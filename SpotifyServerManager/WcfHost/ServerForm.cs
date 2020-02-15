using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfHost.Models;

namespace WcfHost
{
    public partial class ServerForm : Form
    {
        public ServerVM ServerVM { get; set; }
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
            this.bindingSourceConnectedMember.DataSource = serverVM.ConnectedMembers;
            //this.bindingSourceConnectedMember.

            this.ServerVM.LogHasChanged += LogHasChanged;

            this.ServerVM.MemberHasChanged += new ServerVM.MemberHasChangedDelegate(MemberHasChanged);

            // Anonymus Event Delegates
            //bs.AddingNew += (s, ev) => Debug.WriteLine("AddingNew");
        }

        private void MemberHasChanged()
        {
            InvokeIfRequired(this, (MethodInvoker)delegate ()
            {
                //this.listBoxUsers.Items.Add(member);
                this.bindingSourceConnectedMember.ResetBindings(false);
                this.Refresh();
            });
        }  

        private void LogHasChanged(string msg)
        {
            InvokeIfRequired(this, (MethodInvoker)delegate ()
            {
                this.textBoxLog.Text +=  $"<{DateTime.Now}>: " + msg + Environment.NewLine;
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
