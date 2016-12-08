using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using NAudio.Wave;
using OxyPlot.Wpf;
using OxyPlot;
using System.Windows;
using System.Windows.Input;
using Livet.Commands;
using Swiftspear.Commands;
using Swiftspear.Models;

namespace Swiftspear.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private WaveInputModel _model = new WaveInputModel();

        /// <summary>
        /// インプットデバイスの一覧を取得します。
        /// </summary>
        public ObservableSynchronizedCollection<string> InputDevices
        {
            get { return _model.InputDevices; }
        }

        /// <summary>
        /// 録音中のデバイスIDを取得または設定します。
        /// </summary>
        public int CurrentDeviceId
        {
            get { return _model.CurrentDeviceId; }
            set { _model.CurrentDeviceId = value; }
        }

        /// <summary>
        /// 現在録音中のインプットデバイスの音量を取得します。
        /// </summary>
        public double Volume
        {
            get { return _model.Volume; }
        }

        public bool IsRecording
        {
            get { return _model.IsRecording; }
        }

        public bool CanChangeDevice
        {
            get { return !IsRecording; }
        }
        /// <summary>
        /// 現在録音中の時間領域データを取得します。
        /// </summary>
        public IEnumerable<DataPoint> TimeDomain
        {
            get { return _model.LatestReceived?.Select((p, idx) => new DataPoint(idx, p)); }
        }

        /// <summary>
        /// 現在録音中の時間領域データを取得します。
        /// </summary>
        public IEnumerable<DataPoint> FrequencyDomain
        {
            get { return _model.FreqSpectrum.Select(t => new DataPoint(t.Item1 * 10, t.Item2 / 1000)); }
        }

        public MainWindowViewModel()
        {
            try
            {
                _model = new WaveInputModel();
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show($"音声入力デバイスの初期化に失敗しました。{e.Message}",
                                "デバイスエラー",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(1);
            }

            _model.PropertyChanged += OnModelPropertyChanged;
        }

        /// <summary>
        /// Modelの変更通知イベントを処理します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_model.CurrentDeviceId):
                    RaisePropertyChanged(nameof(CurrentDeviceId));
                    break;
                case nameof(_model.LatestReceived):
                    RaisePropertyChanged(nameof(Volume));
                    RaisePropertyChanged(nameof(TimeDomain));
                    RaisePropertyChanged(nameof(FrequencyDomain));
                    break;
                case nameof(_model.IsRecording):
                    RaisePropertyChanged(nameof(IsRecording));
                    RaisePropertyChanged(nameof(CanChangeDevice));
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private ICommand _toggleRecordingStateCommand;
        public ICommand ToggleRecodingStateCommand
        {
            get
            {
                if (_toggleRecordingStateCommand == null)
                {
                    _toggleRecordingStateCommand = new ViewModelCommand(ToggleRecodingStateCommandImpl);
                }

                return _toggleRecordingStateCommand;
            }
        }

        private void ToggleRecodingStateCommandImpl()
        {
            if (_model.IsRecording)
            {
                _model.StopRecordingToFile();
            }
            else
            {
                _model.StartRecordingToFile();
            }
        }

        private ICommand _openAnalyzeWindowCommand;
        public ICommand OpenAnalyzeWindowCommand
        {
            get
            {
                if (_openAnalyzeWindowCommand == null)
                {
                    _openAnalyzeWindowCommand = new OpenAnalyzerWindowCommand();
                }

                return _openAnalyzeWindowCommand;
            }
        }

        /// <summary>
        /// このオブジェクトが Dispose されるときの処理です。
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            _model?.Dispose();
        }
    }
}
