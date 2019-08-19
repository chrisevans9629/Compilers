using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Minesweeper.Test;
using Pascal.Ide.Wpf.ViewModels;
using Unity;

namespace Pascal.Ide.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;
        public MainWindow(IUnityContainer container)
        {
            var ser = container.Resolve<IDocumentService>();
            InitializeComponent();
            ser.Initialize(FastColoredTextBox);
           
        }
    }
}
