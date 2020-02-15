using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfHost;
using WcfClient;

namespace ShortCutSpotify
{
    public partial class EntryForm : Form
    {
        public WcfHost.WcfHost Host { get; set; }
        public EntryForm()
        {
            InitializeComponent();
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (Host != null)
            {
                Host.CloseApplication = true;
            }

            this.Dispose();
            Application.Exit();
        }

        private void MyForm_Disposed(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            var message = string.Empty;

            // Start Application as Server
            Host = new WcfHost.WcfHost();
            ServerForm serverForm = new ServerForm();
            serverForm.Init(WcfHost.WcfHost.ServerVM);

            if (Host.Start(out message))
            {
                this.Hide();
                serverForm.Show();
            }
            else
            {
                MessageBox.Show(message);
            }
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Start Application as Client
            ClientForm myForm = new ClientForm();
            myForm.Show();
            myForm.Disposed += MyForm_Disposed;
        }
    }
}
