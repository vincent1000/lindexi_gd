﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace principalComputer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        viewModel view;
        public MainWindow()
        {
            view = new viewModel();
            InitializeComponent();
            this.DataContext = view;
        }

        private void principalComputer(object sender , RoutedEventArgs e)
        {
            view.principal();
        }

        private void slaveComputer(object sender , RoutedEventArgs e)
        {
            view.slave();
        }

        private void send(object sender , RoutedEventArgs e)
        {
            view.send();
        }
    }
}
