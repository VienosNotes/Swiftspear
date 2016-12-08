using Livet;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftspear.Models
{
    class AnalyzeModel : ViewModel
    {
        public string FileName { get; private set; }
        private readonly IEnumerable<int> _empty = new List<int>();

        private bool _isPlaying;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            private set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _position;

        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Current));
                }
            }
        }

        /// <summary>
        /// 読み込んでいるファイルのサンプル数を8192（FFTサンプル数）で割ったもの
        /// </summary>
        public int Length
        {
            get
            {
                return (int)(_reader.Length / (2 * 8192));
            }
        }

        private AudioFileReader _reader;


        /// <summary>
        /// ポジション位置の音声データを取得します。
        /// </summary>
        public IEnumerable<int> Current
        {
            get
            {
                if (_reader == null)
                {
                    return _empty;
                }

                var buf = new byte[8192*2];

                _reader.Read(buf, Position * 2 * 8192, 8192 * 2);
                return AnalyzeUtils.AppendBytes(buf);
            }
        }

        /// <summary>
        /// 周波数とその成分量の組を列挙します。
        /// </summary>
        public IEnumerable<Tuple<double, double>> FreqSpectrum
        {
            get
            {
                return AnalyzeUtils.GetFreqSpectrum(Current);
            }
        }

        public AnalyzeModel(string fileName)
        {
            FileName = fileName;
            _reader = new AudioFileReader(fileName);

        }
    }
}
