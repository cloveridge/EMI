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
    public partial class EditClientForm : Form
    {
        // The document to attach to the client file
        string attachedDocLink = "";

        // Variable to check for changes
        bool changeOccurred = false;

        public EditClientForm()
        {
            InitializeComponent();
        }

        private void EditClientForm_Load(object sender, EventArgs e)
        {
            //
            if(pdfLinkLabel.Text == "No file selected...")
            {
                pdfLinkLabel.LinkColor = Color.Black;
            }
            else
            {
                pdfLinkLabel.LinkColor = Color.Blue;
            }
        }

        /*****************************************************************
         * Event Handler for cancelButton
         * Description: Closes the form
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If changes have occurred, asks the user if they want to close
            // without saving, otherwise just closes.
            if(!changeOccurred || MessageBox.Show("Do you want to abandon your changes?","Leave Without Saving",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        /*****************************************************************
         * verifyInput method
         * Description: Makes sure a method is not blank if required, and
         *              not longer than the maximum length.
         * Param inputTextBox: A TextBox which will be verified
         * Param maxLenOfInput: An int which represents the max length of the
         *                      associated SQL field in the database
         * Param required: An optional bool, true if the field cannot be blank
         * Returns: bool checksOut - True if no issues, false if out of spec
         *****************************************************************/
        private bool verifyInput(TextBox inputTextBox, int maxLenOfInput, bool required = false)
        {
            bool checksOut = true;

            if(inputTextBox.Text.Length > maxLenOfInput)
            {
                checksOut = false;
                MessageBox.Show("The " + inputTextBox.Name.ToString() + " input field is too long. Please limit to " + maxLenOfInput.ToString() + " characters");
                inputTextBox.Focus();
            }
            else if (required && inputTextBox.Text == "")
            {
                checksOut = false;
                MessageBox.Show("The " + inputTextBox.Name.ToString() + " input field is required. Please enter data, limited to " + maxLenOfInput.ToString() + " characters");
                inputTextBox.Focus();
            }

            return checksOut;
        }

        /*****************************************************************
         * normalizePhone method
         * Description: Removes all characters from given phone number except
         *              numbers, then returns it in "(###) ###-#### x ####"
         *              format (Extension optional). If given a 7-digit number,
         *              adds the 801 area code to the start. If given anything
         *              longer than 10 digits, treats 11+ as an extension. If
         *              given any number less than 10 (But not 7) OR 11, warns the
         *              user that they probably made a typo (But parses it anyway).
         * Params phoneNumber: A string representing a phone number.
         * Returns: phoneNumber - modified and normalized
         *****************************************************************/
        private string normalizePhone(string phoneNumber)
        {
            // Remove all standard phone number formatting
            phoneNumber = phoneNumber.Replace(" ", "");
            phoneNumber = phoneNumber.Replace("(", "");
            phoneNumber = phoneNumber.Replace(")", "");
            phoneNumber = phoneNumber.Replace("e", "");
            phoneNumber = phoneNumber.Replace("x", "");
            phoneNumber = phoneNumber.Replace("t", "");
            phoneNumber = phoneNumber.Replace(".", "");
            phoneNumber = phoneNumber.Replace("-", "");
            phoneNumber = phoneNumber.Replace("+", "");

            if (phoneNumber.Length == 7)
            {
                // Its a 7-digit number, format as "(801) ###-####"
                phoneNumber = "(801) " + phoneNumber.Substring(0, 3) + "-" + phoneNumber.Substring(3, 4);
            }
            else if (phoneNumber.Length == 10)
            {
                // It's a standard 10-digit number, format as "(###) ###-####"
                phoneNumber = "(" + phoneNumber.Substring(0, 3) + ") " + phoneNumber.Substring(3, 3) + "-" + phoneNumber.Substring(6, 4);
            }
            else if (phoneNumber.Length > 10)
            {
                // It's a standard 10-digit number with a possible extension, format as "(###) ###-####"
                phoneNumber = "(" + phoneNumber.Substring(0, 3) + ") " + phoneNumber.Substring(3, 3) + "-" + phoneNumber.Substring(6, 4);
            }

            return phoneNumber;
        }
        
        private void saveAndCloseButton_Click(object sender, EventArgs e)
        {
            // Verify maximum lengths of input fields
            const int MAX_NAME_LEN = 35;
            const int MAX_M_I_LEN = 2;
            const int MAX_ADDRESS_LEN = 35;
            const int MAX_CITY_LEN = 20;
            const int MAX_COUNTY_LEN = 15;
            const int MAX_ZIP_LEN = 5;
            const int MAX_EMAIL_LEN = 40;
            const int MAX_REFERRAL_LEN = 70;

            // Verify the file number is unique
            // TO-DO: QUERY DATABASE TO SEE IF THE NUMBER IS FREE
            // IF IT ISN'T, THEN DISPLAYS BASIC INFO AND ASKS IF THE 
            // USER WANTS TO OVERWRITE THIS INFORMATION

            // Normalize primary phone input data
            if (phoneTextbox.Text != "")
            {
                //Normalize the Primary phone #
                phoneTextbox.Text = normalizePhone(phoneTextbox.Text);
                // Check to see if it was a 7, 10, or 12+ digit phone number
                if (phoneTextbox.Text.Substring(0, 1) != "(")
                {
                    // If not, warn the user; if they want to continue, proceed as normal
                    if (MessageBox.Show("The primary phone number \"+" + phoneTextbox.Text + "\" probably contains a typo. Would you like to double-check before saving?", "Phone Number Typo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // Exit the method so the user can fix the phone
                        return;
                    }
                }
            }

            // Normalize alternate phone 1 input data
            if (altPhone1TextBox.Text != "")
            {
                //Normalize the alternate phone 1 #
                altPhone1TextBox.Text = normalizePhone(altPhone1TextBox.Text);
                // Check to see if it was a 7, 10, or 12+ digit phone number
                if (altPhone1TextBox.Text.Substring(0, 1) != "(")
                {
                    // If not, warn the user; if they want to continue, proceed as normal
                    if (MessageBox.Show("The primary phone number \"+" + altPhone1TextBox.Text + "\" probably contains a typo. Would you like to double-check before saving?", "Phone Number Typo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // Exit the method so the user can fix the phone
                        return;
                    }
                }
            }

            // Normalize alternate phone 2 input data
            if (altPhone2TextBox.Text != "")
            {
                //Normalize the alternate phone 2 #
                altPhone2TextBox.Text = normalizePhone(altPhone2TextBox.Text);
                // Check to see if it was a 7, 10, or 12+ digit phone number
                if (altPhone2TextBox.Text.Substring(0, 1) != "(")
                {
                    // If not, warn the user; if they want to continue, proceed as normal
                    if (MessageBox.Show("The primary phone number \"+" + altPhone2TextBox.Text + "\" probably contains a typo. Would you like to double-check before saving?", "Phone Number Typo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // Exit the method so the user can fix the phone
                        return;
                    }
                }
            }

            // Verify the input fields
            if (!verifyInput(ClientFirstTextbox, MAX_NAME_LEN, true) ||
               !verifyInput(clientLastTextbox, MAX_NAME_LEN, true) ||
               !verifyInput(clientMiddleTextbox, MAX_M_I_LEN) ||
               !verifyInput(spouseFirstTextbox, MAX_NAME_LEN) ||
               !verifyInput(spouseLastTextbox, MAX_NAME_LEN) ||
               !verifyInput(spouseMiddleTextbox, MAX_M_I_LEN) ||
               !verifyInput(addressTextbox, MAX_ADDRESS_LEN, true) ||
               !verifyInput(cityTextbox, MAX_CITY_LEN, true) ||
               !verifyInput(countyTextbox, MAX_COUNTY_LEN, true) ||
               !verifyInput(zipTextbox, MAX_ZIP_LEN, true) ||
               !verifyInput(emailTextBox, MAX_EMAIL_LEN) ||
               !verifyInput(referredByTextbox, MAX_REFERRAL_LEN))
            {
                return;
            }

            // Variables for working with the SQLite3 Database
            SQLiteConnection db_connect;
            SQLiteCommand db_comm;

            // Create connection to database and open it
            db_connect = new SQLiteConnection("Data Source=emi.db;Version=3;");
            db_connect.Open();









            

            // Try to insert a new row in the database
            // If the file already exists, confirm the information hasn't changed much (Same name)
            // If not, go ahead and update the info
            // If it has, the user is trying to overwrite a file number. Warn the user, and if they 
            // want to continue, update the existing values with the new ones.
            this.Close();
        }

        /*****************************************************************
         * browseForFile method
         * Description: Checks to see if the user has already selected a file
         *              to attach to the client. If so, it warns them that they
         *              will override the existing link. If the user has not
         *              already attached a file or wishes to continue, opens
         *              a dialog for the user to select a file, which is then
         *              saved in the global variable "attachedDocLink" in full
         *              filepath form, as well as the user's record.
         * Params: None
         * Returns: None
         *****************************************************************/
        private void browseForFile()
        {
            bool changeFile = true;
            // Unless the user already has a file and doesn't want it replaced,
            // displays a friendly filename in the ink, and updates the 
            // attachedDocLink variable to hold the link location
            if (attachFileDialog.ShowDialog() == DialogResult.OK)
            {
                if(attachedDocLink != attachFileDialog.FileName && attachedDocLink != "")
                {
                    // Warn the user if there's already a file, and it's different
                    // from the one they selected (So they can click "Cancel" OR "OK"
                    //  to undo the file selection in case they clicked "Browse"
                    // by accident).
                    if (MessageBox.Show("Change the attached file for " + clientTitleCombobox.Text + ". " + clientLastTextbox.Text + "?", "Change Attachment", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        changeFile = false;
                    }
                }

                // Go ahead and change the file
                if (changeFile)
                {
                    pdfLinkLabel.Text = attachFileDialog.SafeFileName;
                    attachedDocLink = attachFileDialog.FileName;
                }
            }

        }

        /*****************************************************************
         * Event Handler for browseButton_Click
         * Description: Events handler for the browse button. Calls the 
         *              browseForFile() private method.
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void browseButton_Click(object sender, EventArgs e)
        {
            browseForFile();
        }


        /*****************************************************************
         * Event Handler for createTablesButton_Click
         * Description: A temporary button that will be removed once the
         *               database is set up. Drops all tables (invoices,
         *               clients, services, and invoiceservices) if they
         *               exist, then creates them.
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void createTablesButton_Click(object sender, EventArgs e)
        {
            // Variables for working with the SQLite3 Database
            SQLiteConnection db_connect;
            SQLiteCommand db_comm;

            // Create connection to database and open it
            db_connect = new SQLiteConnection("Data Source=emi.db;Version=3;");
            db_connect.Open();

            // Drop tables
            try
            {
                db_comm = new SQLiteCommand("DROP TABLE Clients;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine(exc.Message);
            }

            try
            {
                db_comm = new SQLiteCommand("DROP TABLE Invoices;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine(exc.Message);
            }

            try
            {
                db_comm = new SQLiteCommand("DROP TABLE Services;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine(exc.Message);
            }

            try
            {
                db_comm = new SQLiteCommand("DROP TABLE InvoiceServices;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine(exc.Message);
            }

            // Create the tables
            try
            {
                db_comm = new SQLiteCommand("CREATE TABLE Clients;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }






            //Close the database connection
            db_connect.Close();
        }

        /*****************************************************************
         * Event Handler for pdfLinkLabel_LinkClicked
         * Description: If a file has already been attached, opens the file
         *              in the user's native PDF reader app. If there is no
         *              attached file, calls the browseForFile method.
         * Param sender: An unused object that sends the event
         * Param e: Unused LinkLabelLinkClickedEventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void pdfLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if(attachedDocLink != "")
            {
                try
                {
                    System.Diagnostics.Process.Start(attachedDocLink);
                }
                catch(Exception exc)
                {
                    // Write error message to console, to show that something happened/was attempted.
                    Console.WriteLine(exc.Message);

                    if (MessageBox.Show("The specified file could not be located. Remove bad link?", "File Not Found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // Remove the path, the display filename, and mark that a change has occurred.
                        attachedDocLink = "";
                        pdfLinkLabel.Text = "No file selected...";
                        changeOccurred = true;
                    }
                }
            }
            else
            {
                browseForFile();
            }
        }

        /*********************************************************************
         *********************************************************************
         *********************************************************************
         * ALL EVENT HANDLERS BELOW SIMPLY REPORT THAT A CHANGE HAS OCCURRED *
         *********************************************************************
         *********************************************************************
         *********************************************************************/
        private void fileNumberTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void clientTitleCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void ClientFirstTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void clientMiddleTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void clientLastTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void spouseTitleCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void spouseFirstTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void spouseMiddleTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void spouseLastTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void addressUnknownCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void addressTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void zipTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void cityTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void countyTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void stateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void phoneTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void altPhone1TextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void altPhone2TextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void notesTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void referredByTextbox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void estatePlannerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }
    }
}
