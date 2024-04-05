using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace ServerUpload
{
    public partial class Form1 : Form
    {

        struct ftpSetting
        {
            public string fileName { get; set; }
            public string server { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string fullName { get; set; }
        }

        ftpSetting inputParameter;

        public Form1()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblUpload.Text = "Upload Complete";
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblUpload.Text = $"Uploaded {e.ProgressPercentage}%";
            progressBar.Value = e.ProgressPercentage;
            progressBar.Update();
        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = ((ftpSetting)e.Argument).fileName;
            string fullName = ((ftpSetting)e.Argument).fullName;
            string username = ((ftpSetting)e.Argument).username;
            string password = ((ftpSetting)e.Argument).password;
            string server = ((ftpSetting)e.Argument).server;

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", server, fileName)));
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(username, password);
        Stream ftpStream = request.GetRequestStream();
        FileStream fs = File.OpenRead(fullName);
        byte[] buffer = new byte[1024];
        double total = (double)fs.Length;
        int byteRead = 0;
        double read = 0;
        do
        {
            if (!backgroundWorker.CancellationPending)
            {
                byteRead = fs.Read(buffer, 0, 1024);
                ftpStream.Write(buffer, 0, byteRead);
                read += (double)byteRead;
                double percentage = read / total * 100;
                backgroundWorker.ReportProgress((int)percentage);
            }
        }

        while (byteRead != 0);
        fs.Close();
        ftpStream.Close();
    }
    private void uploadOnServer()
    {
        // Start the file upload process by calling RunWorkerAsync and passing in the input parameter
        backgroundWorker.RunWorkerAsync(inputParameter);

    }

    private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "All files]|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileInfo file = new FileInfo(ofd.FileName);
                    inputParameter.username = txtUsername.Text;
                    inputParameter.password = txtPassword.Text;
                    inputParameter.server = txtServer.Text;
                    inputParameter.fileName = file.Name;
                    inputParameter.fullName = file.FullName;
                }
            }
            uploadOnServer();
        }

    }
}
