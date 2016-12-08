using Livet;
using Livet.Commands;
using Microsoft.Win32;
using OxyPlot;
using Swiftspear.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Swiftspear.ViewModels
{
    class AnalyzerWindowViewModel : ViewModel
    {
        private readonly AnalyzeModel _model;
        
        public string Title
        {
            get
            {
                return $"Analyzer - {Path.GetFileName(_model.FileName)}";
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _model.IsPlaying;
            }
        }

        public IEnumerable<DataPoint> TimeDomain
        {
            get { return _model.Current?.Select((p, idx) => new DataPoint(idx, p)); }
        }

        public IEnumerable<DataPoint> FrequencyDomain
        {
            get { return _model.FreqSpectrum?.Select(t => new DataPoint(t.Item1, t.Item2 / 10000)); }
        }

        public int Position
        {
            get
            {
                return _model.Position;
            }
            set
            {
                Console.WriteLine(value);
                _model.Position = value;
            }
        }

        public int MaxPosition
        {
            get { return _model.Length; }
        }

        public AnalyzerWindowViewModel(string fileName)
        {
            _model = new AnalyzeModel(fileName);
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
                case nameof(_model.Position):
                    RaisePropertyChanged(nameof(Position));
                    RaisePropertyChanged(nameof(TimeDomain));
                    RaisePropertyChanged(nameof(FrequencyDomain));
                    break;
                case nameof(_model.IsPlaying):
                    RaisePropertyChanged(nameof(IsPlaying));
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
