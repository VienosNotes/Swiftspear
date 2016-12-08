using Livet;
using Livet.Commands;
using Microsoft.Win32;
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
        private readonly string _fileName;
        
        public string Title
        {
            get
            {
                return $"Analyzer - {Path.GetFileName(_fileName)}";
            }
        }

        public AnalyzerWindowViewModel(string fileName)
        {
            _fileName = fileName;               
        }
    }
}
