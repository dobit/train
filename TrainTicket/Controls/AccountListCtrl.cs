using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LFNet.TrainTicket.Controls
{
    public partial class AccountListCtrl : UserControl
    {

        public event EventHandler SelectedChanged;
        public AccountListCtrl()
        {
            InitializeComponent();
            Reset();
        }

        public void Reset()
        {
            TreeNode root=new TreeNode("账号列表",3,3);
            List<Account> accounts = AccountManager.AccountList;
            foreach (var account in accounts)
            {
                TreeNode treeNode = new TreeNode(account.AccountInfo.Username);
                treeNode.Tag = account;
                root.Nodes.Add(treeNode);
            }
            this.treeView1.Nodes.Clear();
            this.treeView1.Nodes.Add(root);
            this.treeView1.ExpandAll();
        }

        public Account SelectedValue
        {
            get
            {
                if (treeView1.SelectedNode != null) return treeView1.SelectedNode.Tag as Account;
                else
                {
                    return null;
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(SelectedChanged!=null)
            {
                SelectedChanged(this, e);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Account account = AccountManager.CreateNewAccount();
            TreeNode treeNode = new TreeNode(account.AccountInfo.Username, 2, 2);
            treeNode.Tag = account;
            this.treeView1.Nodes[0].Nodes.Add(treeNode);
            this.treeView1.SelectedNode = treeNode;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Account account = treeView1.SelectedNode.Tag as Account;
            if (account != null) AccountManager.Remove(account.AccountInfo.Username);
            Reset();
        }
    }
}
