﻿
using OxyPlot.Wpf;
using Swiftspear.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Swiftspear
{
    /// <summary>
    /// AnalyzerWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AnalyzerWindow : Window
    {
        public AnalyzerWindow(string fileName)
        {
            InitializeComponent();
            DataContext = new AnalyzerWindowViewModel(fileName);
        }

    }
}
