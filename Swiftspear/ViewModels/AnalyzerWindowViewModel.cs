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
        private readonly AnalyzeModel _model = new AnalyzeModel();

        private readonly string _fileName;
        
        public string Title
        {
            get
            {
                return $"Analyzer - {Path.GetFileName(_fileName)}";
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
            get { return null; } // _model.LatestReceived?.Select((p, idx) => new DataPoint(idx, p)); }
        }

        public IEnumerable<DataPoint> FrequencyDomain
        {
            get { return null; } // _model.FreqSpectrum.Select(t => new DataPoint(t.Item1 * 10, t.Item2 / 1000)); }
        }

        public AnalyzerWindowViewModel(string fileName)
        {
            _fileName = fileName;               
        }


    }
}
