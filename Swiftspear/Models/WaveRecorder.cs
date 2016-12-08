using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftspear.Models
{
    /// <summary>
    /// 録音機能を提供します。
    /// コンストラクタで保存先とフォーマットを指定したあと、このクラスの　OnDataReceived メソッドを WaveIn クラスの DataAvailable イベントにハンドラとして登録すれば音声データが保存されます。
    /// 録音を停止する際は StopRecording メソッドを呼んでください。
    /// </summary>
    class WaveRecorder : IDisposable
    {
        private WaveFormat _format;
        private string _path;
        private WaveFileWriter _writer;
        private bool _isRecordingFinished;
        private object _locker = new object();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">音声データを書き込むファイル名のフルパス。</param>
        /// <param name="format">音声フォーマット。</param>
        public WaveRecorder(string path, WaveFormat format)
        {
            _path = path;
            _format = format;
            _writer = new WaveFileWriter(path, format);
        }

        /// <summary>
        /// 録音を終了し、バッファの残りをファイルに書き込みます。
        /// </summary>
        public void StopRecording()
        {
            lock (_locker)
            {
                if (!_isRecordingFinished)
                {
                    _isRecordingFinished = true;
                    _writer.Flush();
                    _writer.Dispose();
                }
            }
        }

        /// <summary>
        /// WaveIn オブジェクトの DataAvailable イベントのハンドラ。
        /// このハンドラが呼び出されるたびに音声データがバッファおよびファイルに書き込みます。
        /// </summary>
        /// <param name="sender">WaveInオブジェクト。</param>
        /// <param name="e">イベント情報。</param>
        public virtual void OnDataReceived(object sender, WaveInEventArgs e)
        {
            if (_isRecordingFinished)
            {
                return;
            }

            var len = e.BytesRecorded;
            var buf = e.Buffer.Take(len).ToArray();

            lock (_locker)
            {
                _writer.Write(buf, 0, len);
            }
        }

        /// <summary>
        /// このオブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            StopRecording();
        }
    }
}
