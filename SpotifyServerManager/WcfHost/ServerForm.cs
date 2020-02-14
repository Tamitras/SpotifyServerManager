using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WcfHost
{
    public partial class ServerForm : Form
    {
        ServerVM ServerVM { get; set; }
        public ServerForm()
        {
            InitializeComponent();
            Application.ApplicationExit += Application_ApplicationExit;
            this.Disposed += ServerForm_Disposed;
        }

        private void ServerForm_Disposed(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Application.Exit();
        }


        public void Init(ServerVM serverVM)
        {
            ServerVM = serverVM;

            this.ServerVM.UserHasConnected += new ServerVM.UserHasConnectedDelegate(UserHasConnected);
            this.ServerVM.UserHasDisconected += new ServerVM.UserHasConnectedDelegate(UserHasDisconected);
            this.ServerVM.LogHasChanged += ServerVM_LogHasChanged;
        }

        private void ServerVM_LogHasChanged(string msg)
        {
            InvokeIfRequired(this, (MethodInvoker)delegate ()
            {
                this.textBoxLog.Text +=  $"<{DateTime.Now}>: " + msg + Environment.NewLine;
                this.Refresh();
            });
        }

        private void UserHasDisconected(string user)
        {
            InvokeIfRequired(this, (MethodInvoker)delegate ()
            {
                this.listBoxUsers.Items.Clear();
                this.listBoxUsers.Items.AddRange(this.ServerVM.ConnectedUsers.ToArray());
                this.Refresh();
            });
        }

        public void UserHasConnected(string user)
        {
            InvokeIfRequired(this, (MethodInvoker)delegate ()
            {
                this.listBoxUsers.Items.Add(user);
                this.Refresh();
            });
        }

        /// <summary>
        /// Mit Hilfe von InvokeRequired wird geprüft ob der Aufruf direkt an die UI gehen kann oder
        /// ob ein Invokeing hier von Nöten ist
        /// </summary>
        /// <param name="target"></param>
        /// <param name="methodToInvoke"></param>
        private void InvokeIfRequired(Control target, Delegate methodToInvoke)
        {
            if (target.InvokeRequired)
            {
                // Das Control muss per Invoke geändert werden, weil der aufruf aus einem Backgroundthread kommt
                target.Invoke(methodToInvoke);
            }
            else
            {
                // Die Änderung an der UI kann direkt aufgerufen werden.
                methodToInvoke.DynamicInvoke();
            }
        }
    }
}
