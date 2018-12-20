using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EstateMastersInstitute
{
    public partial class EditClientForm : Form
    {
        public EditClientForm()
        {
            InitializeComponent();
        }

        private void EditClientForm_Load(object sender, EventArgs e)
        {
            if(pdfLinkLabel.Text == "No file selected...")
            {
                pdfLinkLabel.LinkColor = Color.Black;
            }
            else
            {
                pdfLinkLabel.LinkColor = Color.Blue;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAndCloseButton_Click(object sender, EventArgs e)
        {
            // Verify the input fields
            // Try to insert a new row in the database
            // If the file already exists, confirm the information hasn't changed much (Same name)
                // If not, go ahead and update the info
            // If it has, the user is trying to overwrite a file number. Warn the user, and if they 
            // want to continue, update the existing values with the new ones.
            this.Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {

        }

        private void createTablesButton_Click(object sender, EventArgs e)
        {

        }
    }
}
