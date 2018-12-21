using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EstateMastersInstitute
{
    public partial class mainMenuForm : Form
    {
        public mainMenuForm()
        {
            InitializeComponent();
        }

        private void EstateMastersInstitute_Load(object sender, EventArgs e)
        {
           
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            ExitProgram();
        }

        private void exitLabel_Click(object sender, EventArgs e)
        {
            ExitProgram();
        }

        private void EditClientButton_Click(object sender, EventArgs e)
        {
            EditClient();
        }

        private void EditClientLabel_Click(object sender, EventArgs e)
        {
            EditClient();
        }

        private void clientLookupButton_Click(object sender, EventArgs e)
        {
            ClientLookup();
        }

        private void clientLookupLabel_Click(object sender, EventArgs e)
        {
            ClientLookup();
        }

        private void invoicingButton_Click(object sender, EventArgs e)
        {
            InvoicingOptions();
        }

        private void invoicingLabel_Click(object sender, EventArgs e)
        {
            InvoicingOptions();
        }

        private void mailingButton_Click(object sender, EventArgs e)
        {
            MailingOptions();
        }

        private void mailingLabel_Click(object sender, EventArgs e)
        {
            MailingOptions();
        }

        private void EditClient()
        {
            EditClientForm editing_form = new EditClientForm();
            editing_form.ShowDialog();
        }

        private void ClientLookup()
        {
            ClientLookupForm lookup_form = new ClientLookupForm();
            lookup_form.ShowDialog();
        }

        private void InvoicingOptions()
        {
            InvoicingForm invoicing_form = new InvoicingForm();
            invoicing_form.ShowDialog();
        }

        private void MailingOptions()
        {
            MailOptionsForm mailing_form = new MailOptionsForm();
            mailing_form.ShowDialog();
        }

        private void ExitProgram()
        {
            this.Close();
        }

        private void developerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:christian.loveridge@gmail.com");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Please contact Christian Loveridge at christian.loveridge@gmail.com for questions, comments, or help.","Contact Developer");
            }
        }
    }
}
