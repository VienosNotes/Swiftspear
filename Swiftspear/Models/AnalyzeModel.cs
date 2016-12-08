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

        public AnalyzeModel(string fileName)
        {
            FileName = fileName;
            _reader = new AudioFileReader(fileName);

        }
    }
}
