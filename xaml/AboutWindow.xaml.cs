using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Niv
{
    public partial class AboutWindow : Window
    {
        private bool allowClose = false;

        public AboutWindow()
        {
            InitializeComponent();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        public void exit()
        {
            allowClose = true;
            this.Close();
        }

        private void linkAddress_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://jarvisniu.com/niv");
        }
        // end of class
    }
}
