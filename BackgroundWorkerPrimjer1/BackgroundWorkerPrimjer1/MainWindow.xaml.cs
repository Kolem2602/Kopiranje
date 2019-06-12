using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;

namespace BackgroundWorkerPrimjer1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // https://msdn.microsoft.com/en-us/library/system.componentmodel.backgroundworker(v=vs.110).aspx

        private BackgroundWorker radnik1 = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            radnik1.WorkerReportsProgress = true;
            radnik1.WorkerSupportsCancellation = true;
          
            radnik1.DoWork += new DoWorkEventHandler(radnik_DoWork);
            radnik1.ProgressChanged += new ProgressChangedEventHandler(radnik_ProgressChanged);
            radnik1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(radnik_RunWorkerCompleted);
        }

        private void pokreniTipka_Click(object sender, RoutedEventArgs e)
        {
            List<object> argumenti = new List<object>();
            argumenti.Add(datoteka.Text);
            argumenti.Add(putanja.Text);
            if (radnik1.IsBusy != true)
            {
                // Metodom RunWorkerAsync se pokreće izvođenje operacije u pozadiniputs
                
                radnik1.RunWorkerAsync(argumenti);
            }
        }

       

        /* DoWork
        /* https://msdn.microsoft.com/en-us/library/system.componentmodel.backgroundworker.dowork.aspx 
         * -->
         * Your code in the DoWork event handler should periodically check the CancellationPending 
         * property value and abort the operation if it is true. When this occurs, you can set the 
         * Cancel flag of System.ComponentModel.DoWorkEventArgs to true, and the Cancelled flag of 
         * System.ComponentModel.RunWorkerCompletedEventArgs in your RunWorkerCompleted event handler 
         * will be set to true.
         */
        
      
        

        private void radnik_DoWork(object sender, DoWorkEventArgs e)
        {
           
            List<object> lista = e.Argument as List<object>;
            var datoteka = lista[0].ToString();
            var putanja = lista[1].ToString();
            int bufferSize = 1024 * 1024;
            using (FileStream kopiram = new FileStream(datoteka, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (FileStream kamkopiram = new FileStream(putanja, FileMode.OpenOrCreate, FileAccess.Write))
            {
                int bytesRead = -1;
                var totalReads = 0;
                var totalBytes = kopiram.Length;
                byte[] bytes = new byte[bufferSize];
                int prevPercent = 0;

                while ((bytesRead = kopiram.Read(bytes, 0, bufferSize)) > 0)
                {
                    kamkopiram.Write(bytes, 0, bytesRead);
                    totalReads += bytesRead;
                    int percent = System.Convert.ToInt32(((decimal)totalReads / (decimal)totalBytes) * 100);
                    if (percent != prevPercent)
                    {
                        radnik1.ReportProgress(percent);
                        prevPercent = percent;
                    }
                }
            }
           

        }

        private void radnik_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");

            napredakTekst.Text = e.ProgressPercentage.ToString();
            napredakTraka.Value = e.ProgressPercentage;
        }

        private void radnik_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            if ((e.Cancelled == true))
            {
                napredakTekst.Text = "Otkazano!";
            }

            else if (!(e.Error == null))
            {
                napredakTekst.Text = ("Greška: " + e.Error.Message);
            }
            else
            {
                napredakTekst.Text = "Gotovo!";
            }
        }

        /* https://msdn.microsoft.com/en-us/library/system.componentmodel.backgroundworker.cancelasync.aspx
         *
         * CancelAsync submits a request to terminate the pending background operation and sets the CancellationPending 
         *
         * property to true. When you call CancelAsync, your worker method has an opportunity to stop its execution and exit. 
         * The worker code should periodically check the CancellationPending property to see if it has been set to true.
         */

        private void zaustaviTipka_Click(object sender, RoutedEventArgs e)
        {
            radnik1.CancelAsync();
        }

        private void Odaberi_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Get the selected file name and display in a TextBox 
            if (dlg.ShowDialog() == true)
            {
                // Open document 
                string filename = dlg.FileName;
                datoteka.Text = filename;
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            
            string nastavak;
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = putanja.Text; 
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                nastavak = Path.GetExtension(datoteka.Text);
                putanja.Text = path+nastavak;
            }
        }
    }
}
