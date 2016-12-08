using Livet.Commands;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Swiftspear.Commands
{
    class OpenAnalyzerWindowCommand : Command, ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "WAVファイル (*.wav)|*.wav",
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.RecordsDir),
                DefaultExt = "*.wav"
            };

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                var window = new AnalyzerWindow(dlg.FileName);
                window.Show();
            }
        }
    }
}
