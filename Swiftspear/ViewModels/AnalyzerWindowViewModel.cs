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
            get { return _model.FreqSpectrum?.Select(t => new DataPoint(t.Item1 * 10, t.Item2 / 1000)); }
        }

        public int Position
        {
            get
            {
                return _model.Position;
            }
            set
            {
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
        }
    }
}
