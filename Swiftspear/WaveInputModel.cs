﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using NAudio.Wave;
using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;
using System.IO;

namespace Swiftspear
{
    /// <summary>
    /// 音声の取得、解析、録音などの機能を提供します。
    /// </summary>
    class WaveInputModel : ViewModel
    {
        private ObservableSynchronizedCollection<string> _inputDevices = new ObservableSynchronizedCollection<string>();

        /// <summary>
        /// インプットデバイスの一覧を取得します。
        /// </summary>
        public ObservableSynchronizedCollection<string> InputDevices
        {
            get
            {
                return _inputDevices;
            }
            set
            {
                if (value != _inputDevices)
                {
                    _inputDevices = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _currentDeviceId = 0;

        /// <summary>
        /// 録音中のデバイスIDを取得または設定します。
        /// </summary>
        public int CurrentDeviceId
        {
            get { return _currentDeviceId; }
            set
            {
                if (value != _currentDeviceId)
                {
                    _currentDeviceId = value;
                    SetWaveIn(value);
                    RaisePropertyChanged();
                }
            }
        }

        private WaveIn _currentWaveIn;

        /// <summary>
        /// コンストラクタ。
        /// このオブジェクトが生成されると、自動的に音声データの取得を始めます。入力デバイスはデバイスIDの一番小さなものを既定で使用します。
        /// 音声入力可能なデバイスがない場合は例外を投げます。
        /// </summary>
        /// <exception cref="InvalidOperationException">利用可能な音声入力デバイスが見つかりません。</exception>
        public WaveInputModel()
        {
            if (WaveIn.DeviceCount == 0)
            {
                throw new InvalidOperationException("利用可能な音声入力デバイスが見つかりません。");
            }

            foreach (var i in Enumerable.Range(0, WaveIn.DeviceCount))
            {
                var dev = WaveIn.GetCapabilities(i);
                _inputDevices.Add(String.Format("{0} ,{1} channels", dev.ProductName, dev.Channels));
            }

            CurrentDeviceId = 0;
            SetWaveIn(0);
        }

        /// <summary>
        /// 現在録音中のインプットデバイスの音量を取得します。
        /// </summary>
        public double Volume
        {
            get
            {
                if (LatestReceived == null)
                {
                    return 0;
                }
                return LatestReceived.Where(v => v < 1 << 15)
                                     .Select(v => (double)v)
                                     .ToList()
                                     .Mean() * 2;
            }
        }

        /// <summary>
        /// 最新のサンプリングデータを取得します。
        /// </summary>
        public IEnumerable<int> LatestReceived
        {
            get
            {
                if (_latestReceivedRaw == null)
                {
                    yield break;
                }

                var len = _latestReceivedRaw.Count();
                var buf = _latestReceivedRaw.ToArray();

                for (var index = 0; index < len; index += 2)
                {
                    var sample = (short)((buf[index + 1] << 8) | buf[index + 0]);
                    yield return sample;
                }
            }
        }

        /// <summary>
        /// 周波数とその成分量の組を列挙します。
        /// </summary>
        public IEnumerable<Tuple<double, double>> FreqSpectrum
        {
            get
            {
                return GetFreqSpectrum(LatestReceived);
            }
        }

        /// <summary>
        /// 離散的な音声データを受け取り、フーリエ変換した結果を返します。窓関数にはハミング窓を用います。
        /// </summary>
        /// <param name="latestReceived">1要素1サンプルの音声データ。要素数が2の累乗個である必要があります。</param>
        /// <returns>フーリエ変換によって得られた周波数（0.1Hz刻み）とその周波数成分の組を列挙します。</returns>
        private static IEnumerable<Tuple<double, double>> GetFreqSpectrum(IEnumerable<int> latestReceived)
        {
            if (!latestReceived.Any())
            {
                return new List<Tuple<double, double>>();
            }

            var buf = latestReceived.ToArray();
            var window = Window.Hamming(buf.Length);
            var complex = buf.Select((v, i) => new Complex(v * window[i], 0.0)).ToArray();
            Fourier.Forward(complex, FourierOptions.Matlab);
            var points = complex.Take(complex.Length / 2).Select((v, i) =>
                  Tuple.Create<double, double>(i, Math.Sqrt(v.Real * v.Real + v.Imaginary * v.Imaginary))
              );
            return points;
        }

        private IEnumerable<byte> _latestReceivedRaw;

        /// <summary>
        /// インプットデバイスを設定します。すでに設定されている場合は、既存のインプットデバイスは `Dispose` されます。
        /// </summary>
        /// <param name="id">インプットデバイスID。</param>
        /// <param name="freqency">サンプリング周波数（Hz）。既定は8000Hzです。</param>
        /// <param name="bits">サンプリングビット数。既定は16bitです。</param>
        /// <param name="channels">チャンネル数。既定は1（モノラル）です。</param>
        protected virtual void SetWaveIn(int id, int freqency = 8192, int bits = 16, int channels = 1)
        {
            _currentWaveIn?.StopRecording();
            _currentWaveIn?.Dispose();

            _currentWaveIn = new WaveIn
            {
                DeviceNumber = id,
                WaveFormat = new WaveFormat(freqency, bits, channels)
            };

            _currentWaveIn.DataAvailable += OnDataAvailable;
            _currentWaveIn.StartRecording();
        }

        protected virtual void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            _latestReceivedRaw = e.Buffer.Take(e.BytesRecorded);
            RaisePropertyChanged(nameof(LatestReceived));
        }

        public new void Dispose()
        {
            base.Dispose();
            _currentWaveIn.StopRecording();
            _currentWaveIn.Dispose();
        }

        private bool _isRecording;

        /// <summary>
        /// 録音状態を取得します。録音が進行中の場合は true、そうでない場合は false を返します。
        /// </summary>
        public bool IsRecording
        {
            get
            {
                return _isRecording;
            }
            set
            {
                if (_isRecording != value)
                {
                    _isRecording = value;
                    RaisePropertyChanged();
                }
            }
        }


        private WaveRecorder _recorder;

        /// <summary>
        /// 録音を開始します。すでに録音が進行中の場合はそれを停止し、新たに録音を開始します。
        /// </summary>
        public virtual void StartRecordingToFile()
        {
            StopRecordingToFile();
            IsRecording = true;

            _recorder = new WaveRecorder(GetSavingPath(), _currentWaveIn.WaveFormat);
            _currentWaveIn.DataAvailable += _recorder.OnDataReceived;            
        }

        /// <summary>
        /// 録音を終了します。
        /// </summary>
        public virtual void StopRecordingToFile()
        {
            IsRecording = false;
            if (_currentWaveIn != null && _recorder != null)
            {
                _currentWaveIn.DataAvailable -= _recorder.OnDataReceived;
            }
            _recorder?.StopRecording();
            _recorder?.Dispose();
        }

        /// <summary>
        /// 保存先のファイル名をフルパスで返します。
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSavingPath()
        {
            var targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "record");
            Directory.CreateDirectory(targetDir);
            return Path.Combine(targetDir, DateTime.Now.ToString("yyyyMMddhhmmssff.wav"));
        }
    }
}
