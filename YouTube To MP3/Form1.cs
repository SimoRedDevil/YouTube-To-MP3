using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace YouTube_To_MP3
{
    public partial class Form1 : Form
    {
        public WebBrowser webRequest = new WebBrowser();
        public string mainURL = "https://youtubetomp3music.com/en7/download?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3D";
        public int MP3 = 0;
        public string per = "";
        public string mp3Title = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webRequest.ScriptErrorsSuppressed = true;
            webRequest.Navigate(mainURL);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Status : -"; 
            lblStatus.Text = "Duration : -";
            progressBar1.Value = 0;

            string[] urlSplit = textBox1.Text.Split(new char[] { '=' }, StringSplitOptions.None);
            string fullURL = mainURL + urlSplit[1];
            if(comboBox1.Text == "MP3 64kbps")
            {
                MP3 = 0;
            }
            else if(comboBox1.Text == "MP3 128kbps")
            {
                MP3 = 1;
            }
            else if (comboBox1.Text == "MP3 192kbps")
            {
                MP3 = 2;
            }
            else if (comboBox1.Text == "MP3 256kbps")
            {
                MP3 = 3;
            }
            else if(comboBox1.Text == "MP3 320kbps")
            {
                MP3 = 4;
            }
            
            webRequest.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted);
            webRequest.Navigate(fullURL);
            lblStatus.Text = "Status : Starting...";
        }

        public void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webRequest.Document.GetElementById("formatselect").GetElementsByTagName("optgroup")[1].GetElementsByTagName("option")[MP3].SetAttribute("selected", "selected");
            webRequest.Document.GetElementById("cvt-btn").InvokeMember("click");
            string startConverting = webRequest.Document.GetElementById("cvt-btn").InnerHtml;
            mp3Title = webRequest.Document.GetElementsByTagName("h3")[0].InnerHtml;
            lblDuration.Text = webRequest.Document.GetElementsByTagName("p")[0].InnerHtml.Replace(": ", " : ");

            if (startConverting == "Starting...")
            {
                lblStatus.Text = "Status : Converting...";
                timer1.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string downloadLink = webRequest.Document.GetElementById("mp3-dl-btn").GetAttribute("href");
            WebClient WC = new WebClient();
            WC.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
            WC.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCompleted);
            WC.DownloadDataAsync(new Uri(downloadLink));
        }

        public void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        public void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();       
            saveFile.Filter = "MP3 File | *.mp3";
            saveFile.FileName = mp3Title + ".mp3";
            saveFile.ShowDialog();
            byte[] downloadedBytes = e.Result;
            System.IO.File.WriteAllBytes(saveFile.FileName, downloadedBytes);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            per =  webRequest.Document.GetElementById("cvt-process").InnerHtml;
            string btnConvert = webRequest.Document.GetElementById("cvt-btn").InnerHtml;
            if (btnConvert == "Convert")
            {
                timer1.Stop();
                lblStatus.Text = "Status : Converted Successfully !";
            }
        }
    }
}
