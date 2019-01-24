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

        public EditClientForm(string file_num)
        {
            InitializeComponent();

            SQLiteConnection db_connect;
            SQLiteCommand db_comm;
            SQLiteDataReader reader;
            String command_text;

            // Create connection to database and open it
            db_connect = new SQLiteConnection("Data Source=emi.db;Version=3;");
            db_connect.Open();

            try
            {
                // Query the database and populate the fields
                command_text = "SELECT * FROM Clients WHERE client_num = " + file_num.ToString() + ";";
                db_comm = new SQLiteCommand(command_text, db_connect);
                reader = db_comm.ExecuteReader();

                while (reader.Read())
                {
                    fileNumberTextBox.Text = reader["client_num"].ToString();
                    creationDateLabel.Text = reader["created_date"].ToString();
                    modifiedDateLabel.Text = reader["modified_date"].ToString();
                    clientTitleComboBox.Text = reader["title_1"].ToString();
                    spouseTitleComboBox.Text = reader["title_2"].ToString();
                    clientFirstTextBox.Text = reader["first_name_1"].ToString();
                    spouseLastTextBox.Text = reader["last_name_2"].ToString();
                    clientLastTextBox.Text = reader["last_name_1"].ToString();
                    spouseMiddleTextBox.Text = reader["middle_initial_2"].ToString();
                    clientMiddleTextBox.Text = reader["middle_initial_1"].ToString();
                    spouseFirstTextBox.Text = reader["first_name_2"].ToString();
                    addressTextBox.Text = reader["address"].ToString();
                    zipTextBox.Text = reader["zip"].ToString();
                    cityTextBox.Text = reader["city"].ToString();
                    countyTextBox.Text = reader["county"].ToString();
                    stateComboBox.Text = reader["state"].ToString();
                    addressUnknownCheckBox.Checked = reader["client_num"].ToString() == "T" ? true : false;
                    phoneTextBox.Text = reader["main_phone"].ToString();
                    altPhone1TextBox.Text = reader["alt_phone_1"].ToString();
                    altPhone2TextBox.Text = reader["alt_phone_2"].ToString();
                    emailTextBox.Text = reader["email"].ToString();
                    referredByTextBox.Text = reader["referred_by"].ToString();
                    notesTextBox.Text = reader["notes"].ToString();
                    pdfLinkLabel.Text = reader["attached_file"].ToString();
                    estatePlannerCheckBox.Checked = reader["client_num"].ToString() == "T" ? true : false;
                }
                

            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message + " - The file for this client wasn't found in the database, or the database is missing. Please contact an administrator.");
            }

            db_connect.Close();
            clientFirstTextBox.Focus();

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

            creationDateLabel.Text = DateTime.Today.ToString("yyyy-MM-dd");
            modifiedDateLabel.Text = DateTime.Today.ToString("yyyy-MM-dd");

            clientFirstTextBox.Focus();
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
                // It's a standard 10-digit number with a possible extension, format as "(###) ###-#### x ####"
                phoneNumber = "(" + phoneNumber.Substring(0, 3) + ") " + phoneNumber.Substring(3, 3) + "-" + phoneNumber.Substring(6, 4) + " x " + phoneNumber.Substring(10,phoneNumber.Length - 10);
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
            if (phoneTextBox.Text != "")
            {
                //Normalize the Primary phone #
                phoneTextBox.Text = normalizePhone(phoneTextBox.Text);
                // Check to see if it was a 7, 10, or 12+ digit phone number
                if (phoneTextBox.Text.Substring(0, 1) != "(")
                {
                    // If not, warn the user; if they want to continue, proceed as normal
                    if (MessageBox.Show("The primary phone number \"" + phoneTextBox.Text + "\" probably contains a typo. Would you like to double-check before saving?", "Phone Number Typo", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
            if (!verifyInput(clientFirstTextBox, MAX_NAME_LEN, true) ||
               !verifyInput(clientLastTextBox, MAX_NAME_LEN, true) ||
               !verifyInput(clientMiddleTextBox, MAX_M_I_LEN) ||
               !verifyInput(spouseFirstTextBox, MAX_NAME_LEN) ||
               !verifyInput(spouseLastTextBox, MAX_NAME_LEN) ||
               !verifyInput(spouseMiddleTextBox, MAX_M_I_LEN) ||
               !verifyInput(emailTextBox, MAX_EMAIL_LEN) ||
               !verifyInput(referredByTextBox, MAX_REFERRAL_LEN))
            {
                return;
            }

            // If the address is unknown, the addressing fields aren't required
            if(!addressUnknownCheckBox.Checked)
            {
                if(!verifyInput(addressTextBox, MAX_ADDRESS_LEN, true) ||
               !verifyInput(cityTextBox, MAX_CITY_LEN, true) ||
               !verifyInput(countyTextBox, MAX_COUNTY_LEN, true) ||
               !verifyInput(zipTextBox, MAX_ZIP_LEN, true))
                {
                    return;
                }
            }

            // Variables for working with the SQLite3 Database
            SQLiteConnection db_connect;
            SQLiteCommand db_comm;

            // Create connection to database and open it
            string command_text = "";
            db_connect = new SQLiteConnection("Data Source=emi.db;Version=3;");
            db_connect.Open();


            // Try to insert a new row in the database
            try
            {
                command_text = "INSERT INTO Clients(client_num, created_date, modified_date, title_1, first_name_1, middle_initial_1, last_name_1, title_2, " +
                    "first_name_2, middle_initial_2, last_name_2, address_unknown, address, city, county, state, zip, main_phone, alt_phone_1, alt_phone_2, email, " +
                    "referred_by, has_estate_planner, notes, attached_file) " +
                    "VALUES(" + fileNumberTextBox.Text + ", \"" +
                    creationDateLabel.Text + "\", \"" +
                    (changeOccurred ? DateTime.Today.ToString("yyyy-MM-dd") : modifiedDateLabel.Text) + "\", " +
                    "\"" + clientTitleComboBox.Text + "\"" + ", " +
                    "\"" + clientFirstTextBox.Text + "\"" + ", " +
                    (clientMiddleTextBox.Text == "" ? "\"\"" : "\"" + clientMiddleTextBox.Text + "\"") + ", " +
                    "\"" + clientLastTextBox.Text + "\"" + ", " +
                    "\"" + spouseTitleComboBox.Text + "\"" + ", " +
                    (spouseFirstTextBox.Text == "" ? "\"\"" : "\"" + spouseFirstTextBox.Text + "\"") + ", " +
                    (spouseMiddleTextBox.Text == "" ? "\"\"" : "\"" + spouseMiddleTextBox.Text + "\"") + ", " +
                    (spouseLastTextBox.Text == "" ? "\"\"" : "\"" + spouseLastTextBox.Text + "\"") + ", " +
                    (addressUnknownCheckBox.Checked ? "\"T\"" : "\"F\"") + ", " +
                    (addressTextBox.Text == "" ? "\"\"" : "\"" + addressTextBox.Text + "\"") + ", " +
                    (cityTextBox.Text == "" ? "\"\"" : "\"" + cityTextBox.Text + "\"") + ", " +
                    (countyTextBox.Text == "" ? "\"\"" : "\"" + countyTextBox.Text + "\"") + ", " +
                    (stateComboBox.Text == "" ? "\"\"" : "\"" + stateComboBox.Text + "\"") + ", " +
                    (zipTextBox.Text == "" ? "\"\"" : "\"" + zipTextBox.Text + "\"") + ", " +
                    (phoneTextBox.Text == "" ? "\"\"" : "\"" + phoneTextBox.Text + "\"") + ", " +
                    (altPhone1TextBox.Text == "" ? "\"\"" : "\"" + altPhone1TextBox.Text + "\"") + ", " +
                    (altPhone2TextBox.Text == "" ? "\"\"" : "\"" + altPhone2TextBox.Text + "\"") + ", " +
                    (emailTextBox.Text == "" ? "\"\"" : "\"" + emailTextBox.Text + "\"") + ", " +
                    (referredByTextBox.Text == "" ? "\"\"" : "\"" + referredByTextBox.Text + "\"") + ", " +
                    (estatePlannerCheckBox.Checked ? "\"T\"" : "\"F\"") + ", " +
                    (notesTextBox.Text == "" ? "\"\"" : "\"" + notesTextBox.Text + "\"") + ", " +
                    (attachedDocLink == "" ? "\"\"" : "\"" + attachedDocLink + "\"") +
                    ");";

                db_comm = new SQLiteCommand(command_text, db_connect);
                db_comm.ExecuteNonQuery();
            }
            // If it already exists, try to update the record
            catch (Exception exc)
            {
                string client_name = "";
                // If the client already exists, confirm the information hasn't changed much (Same name)
                Console.WriteLine(exc.Message);
                SQLiteDataReader reader;

                try
                {
                    // Query the database to see what's already in the file
                    command_text = "SELECT * FROM Clients WHERE client_num = " + fileNumberTextBox.Text + ";";
                    db_comm = new SQLiteCommand(command_text, db_connect);
                    reader = db_comm.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        client_name = reader["first_name_1"].ToString() + " " + reader["last_name_1"].ToString();
                    }
                    
                    // See if the client's name matches what's in the file number
                    if (client_name != (clientFirstTextBox.Text + " " + clientLastTextBox.Text))
                    {
                        // If so, warn the user that they might be overriding the existing file.
                        DialogResult result = MessageBox.Show("The file number above is already used for your client " + client_name + ", and continuing will overwrite the existing data, and cannot be undone. Continue?", "", MessageBoxButtons.YesNo);
                        if(result == DialogResult.Yes)
                        {
                            // Save the changes and exit
                            command_text = "UPDATE Clients\n" +
                                "SET created_date = \"" + creationDateLabel.Text + "\", " +
                                "modified_date = \"" + DateTime.Today.ToString("yyyy-MM-dd") + "\", " +
                                "title_1 = \"" + clientTitleComboBox.Text + "\", " +
                                "first_name_1 = \"" + clientFirstTextBox.Text + "\", " +
                                "middle_initial_1 = \"" + clientMiddleTextBox.Text + "\", " +
                                "last_name_1 = \"" + clientLastTextBox.Text + "\", " +
                                "title_2 = \"" + spouseTitleComboBox.Text + "\", " +
                                "first_name_2 = \"" + spouseFirstTextBox.Text + "\", " +
                                "middle_initial_2 = \"" + spouseMiddleTextBox.Text + "\", " +
                                "last_name_2 = \"" + spouseLastTextBox.Text + "\", " +
                                "address_unknown = " + (addressUnknownCheckBox.Checked ? "\"T\"" : "\"F\"") + ", " +
                                "address = \"" + addressTextBox.Text + "\", " +
                                "city = \"" + cityTextBox.Text + "\", " +
                                "county = \"" + countyTextBox.Text + "\", " +
                                "state = \"" + stateComboBox.Text + "\", " +
                                "zip = \"" + zipTextBox.Text + "\", " +
                                "main_phone = \"" + phoneTextBox.Text + "\", " +
                                "alt_phone_1 = \"" + altPhone1TextBox.Text + "\", " +
                                "alt_phone_2 = \"" + altPhone2TextBox.Text + "\", " +
                                "email = \"" + emailTextBox.Text + "\", " +
                                "referred_by = \"" + referredByTextBox.Text + "\", " +
                                "has_estate_planner = \"" + (estatePlannerCheckBox.Checked ? "T" : "F") + "\", " +
                                "notes = \"" + notesTextBox.Text + "\", " +
                                "attached_file = \"" + attachedDocLink + "\"\n" +
                                "WHERE client_num = " + fileNumberTextBox.Text + ";";
                            db_comm = new SQLiteCommand(command_text, db_connect);
                            db_comm.ExecuteNonQuery();

                            MessageBox.Show("File number " + fileNumberTextBox.Text + " is now associated with " + client_name);
                            db_connect.Close();
                            this.Close();
                        }
                        
                        // Do nothing so the user can review their changes
                    }
                    // The client's name is the same, but something else has changed. Confirm changes.
                    else if(changeOccurred)
                    {

                        DialogResult result;
                        // Make sure the user wants to save the changes or discard, and whether to close or not.
                        // ("Yes" = save & close, "No" = discard & close, "Cancel" = discard only.
                        result = MessageBox.Show("Would you like to save your changes?", "Update Information", MessageBoxButtons.YesNoCancel);
                        if(result == DialogResult.Yes)
                        {
                            // Save the changes and exit
                            command_text = "UPDATE Clients\n" +
                                "SET created_date = \"" + creationDateLabel.Text + "\", " +
                                "modified_date = \"" + DateTime.Today.ToString("yyyy-MM-dd") + "\", " +
                                "title_1 = \"" + clientTitleComboBox.Text + "\", " +
                                "first_name_1 = \"" + clientFirstTextBox.Text + "\", " +
                                "middle_initial_1 = \"" + clientMiddleTextBox.Text + "\", " +
                                "last_name_1 = \"" + clientLastTextBox.Text + "\", " +
                                "title_2 = \"" + spouseTitleComboBox.Text + "\", " +
                                "first_name_2 = \"" + spouseFirstTextBox.Text + "\", " +
                                "middle_initial_2 = \"" + spouseMiddleTextBox.Text + "\", " +
                                "last_name_2 = \"" + spouseLastTextBox.Text + "\", " +
                                "address_unknown = " + (addressUnknownCheckBox.Checked ? "\"T\"" : "\"F\"") + ", " +
                                "address = \"" + addressTextBox.Text + "\", " +
                                "city = \"" + cityTextBox.Text + "\", " +
                                "county = \"" + countyTextBox.Text + "\", " +
                                "state = \"" + stateComboBox.Text + "\", " +
                                "zip = \"" + zipTextBox.Text + "\", " +
                                "main_phone = \"" + phoneTextBox.Text + "\", " +
                                "alt_phone_1 = \"" + altPhone1TextBox.Text + "\", " +
                                "alt_phone_2 = \"" + altPhone2TextBox.Text + "\", " +
                                "email = \"" + emailTextBox.Text + "\", " +
                                "referred_by = \"" + referredByTextBox.Text + "\", " +
                                "has_estate_planner = \"" + (estatePlannerCheckBox.Checked ? "T" : "F") + "\", " +
                                "notes = \"" + notesTextBox.Text + "\", " +
                                "attached_file = \"" + attachedDocLink + "\"\n" +
                                "WHERE client_num = " + fileNumberTextBox.Text + ";";
                            db_comm = new SQLiteCommand(command_text, db_connect);
                            db_comm.ExecuteNonQuery();

                            MessageBox.Show("The file for " + client_name + " (#" + fileNumberTextBox.Text + ") has been updated!");
                            db_connect.Close();
                            this.Close();
                        }
                        else if(result == DialogResult.No)
                        {
                            // Abandon changes and close the edit form
                            MessageBox.Show("Changes abandoned.");
                            db_connect.Close();
                            this.Close();
                        }
                        else
                        {
                            // Do nothing, so the user can review their changes before saving/aborting
                        }
                    }
                }
                catch (Exception exc2)
                {
                    Console.WriteLine(exc2.Message);
                }
            }
            
            db_connect.Close();
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
                    if (MessageBox.Show("Change the attached file for " + clientTitleComboBox.Text + ". " + clientLastTextBox.Text + "?", "Change Attachment", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        changeFile = false;
                    }
                }

                // Go ahead and change the file
                if (changeFile)
                {
                    // Copy the file to the Local folder
                    System.IO.File.Copy(attachFileDialog.FileName, "Local\\" + attachFileDialog.SafeFileName + "." + attachFileDialog.FileName.Substring(attachFileDialog.FileName.IndexOf(".") + 1));

                    // Display plain file name on the form
                    pdfLinkLabel.Text = attachFileDialog.SafeFileName;

                    // Link to the Local file
                    attachedDocLink = "Local\\" + attachFileDialog.SafeFileName + "." + attachFileDialog.FileName.Substring(attachFileDialog.FileName.IndexOf(".") + 1);
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

        private void phoneTextBox_Leave(object sender, EventArgs e)
        {
            // Normalize primary phone input data
            if (phoneTextBox.Text != "")
            {
                //Normalize the Primary phone #
                phoneTextBox.Text = normalizePhone(phoneTextBox.Text);
                // Check to see if it was a 7, 10, or 12+ digit phone number
                if (phoneTextBox.Text.Substring(0, 1) != "(")
                {
                    // If not, // Turn the text red
                    phoneTextBox.ForeColor = Color.Red;

                }
            }
        }

        private void altPhone1TextBox_Leave(object sender, EventArgs e)
        {
            // Normalize primary phone input data
            if (altPhone1TextBox.Text != "")
            {
                //Normalize the Primary phone #
                altPhone1TextBox.Text = normalizePhone(altPhone1TextBox.Text);
                // Check to see if it was a 7, 10, or 12+ digit phone number
                if (altPhone1TextBox.Text.Substring(0, 1) != "(")
                {
                    // If not, turn the text red
                    altPhone1TextBox.ForeColor = Color.Red;
                }
            }
        }

        private void altPhone2TextBox_Leave(object sender, EventArgs e)
        {
            // Normalize primary phone input data
            if (altPhone2TextBox.Text != "")
            {
                //Normalize the Primary phone #
                altPhone2TextBox.Text = normalizePhone(altPhone2TextBox.Text);
                // Check to see if it was a 7, 10, or 12+ digit phone number
                if (altPhone2TextBox.Text.Substring(0, 1) != "(")
                {
                    // If not, turn the text red
                    altPhone2TextBox.ForeColor = Color.Red;
                }
            }
        }

        private void phoneTextBox_Enter(object sender, EventArgs e)
        {
            // Turn the text black
            phoneTextBox.ForeColor = Color.Black;
        }

        private void altPhone1TextBox_Enter(object sender, EventArgs e)
        {
            // Turn the text black
            altPhone1TextBox.ForeColor = Color.Black;
        }

        private void altPhone2TextBox_Enter(object sender, EventArgs e)
        {
            // Turn the text black
            altPhone2TextBox.ForeColor = Color.Black;
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
            String command_text;

            // Create connection to database and open it
            db_connect = new SQLiteConnection("Data Source=emi.db;Version=3;");
            db_connect.Open();

            // Drop tables
            try
            {
                db_comm = new SQLiteCommand("DROP TABLE IF EXISTS Clients;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine("Could not drop table: " + exc.Message);
            }

            try
            {
                db_comm = new SQLiteCommand("DROP TABLE IF EXISTS Invoices;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine("Could not drop table: " + exc.Message);
            }

            try
            {
                db_comm = new SQLiteCommand("DROP TABLE IF EXISTS Services;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine("Could not drop table: " + exc.Message);
            }

            try
            {
                db_comm = new SQLiteCommand("DROP TABLE IF EXISTS InvoiceServices;", db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                // Write error message to console, to show that something happened/was attempted.
                Console.WriteLine("Could not drop table: " + exc.Message);
            }

            // Create the tables
            try
            {
                command_text = "CREATE TABLE Clients(client_num INT PRIMARY KEY, " +
                    "created_date DATE, " +
                    "modified_date DATE, " +
                    "title_1 VARCHAR(4), " +
                    "first_name_1 VARCHAR(35), " +
                    "middle_initial_1 CHAR(1), " +
                    "last_name_1 VARCHAR(35), " +
                    "title_2 VARCHAR(3), " +
                    "first_name_2 VARCHAR(35), " +
                    "middle_initial_2 CHAR(1), " +
                    "last_name_2 VARCHAR(35), " +
                    "address_unknown CHAR(1), " +
                    "address VARCHAR(35), " +
                    "city VARCHAR(20), " +
                    "county VARCHAR(15), " +
                    "state CHAR(2), " +
                    "zip CHAR(5), " +
                    "main_phone CHAR(23), " +
                    "alt_phone_1 CHAR(23), " +
                    "alt_phone_2 CHAR(23), " +
                    "email VARCHAR(40), " +
                    "referred_by VARCHAR(70), " +
                    "has_estate_planner CHAR(1), " +
                    "notes TEXT, " +
                    "attached_file VARCHAR(255)" +
                    ");";

                db_comm = new SQLiteCommand(command_text, db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Could not create Clients table: " + exc.Message);
            }

            try
            {
                command_text = "CREATE TABLE Invoices(invoice_num INT PRIMARY KEY, " +
                    "invoice_date DATE, " +
                    "file_num INT, " +
                    "due_date DATE, " +
                    "subtotal MONEY, " +
                    "paid_amt MONEY, " +
                    "\nFOREIGN KEY(file_num) REFERENCES Clients(client_num)" +
                    ")";

                db_comm = new SQLiteCommand(command_text, db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Could not create Invoices table: " + exc.Message);
            }

            try
            {
                command_text = "CREATE TABLE Services(service_name TINYTEXT PRIMARY KEY, " +
                    "service_detail TINYTEXT, " +
                    "standard_price MONEY" +
                    ")";

                db_comm = new SQLiteCommand(command_text, db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Could not create Services table: " + exc.Message);
            }

            try
            {
                command_text = "CREATE TABLE InvoiceServices(INVOICEinvoice_num INT, " +
                    "SERVICEservice_name TINYTEXT, " +
                    "service_detail TINYTEXT, " +
                    "actual_price MONEY, " +
                    "\nFOREIGN KEY(INVOICEinvoice_num) REFERENCES Invoices(invoice_num)," +
                    "\nFOREIGN KEY(SERVICEservice_name) REFERENCES Services(service_name)" +
                    ")";

                db_comm = new SQLiteCommand(command_text, db_connect);
                db_comm.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Could not create InvoiceServices table: " + exc.Message);
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
        private void fileNumberTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void clientTitleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void clientFirstTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void clientMiddleTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void clientLastTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void spouseTitleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void spouseFirstTextBox_TextChanged(object sender, EventArgs e)
        {
            if(spouseFirstTextBox.Text != "" && clientLastTextBox.Text != "" && spouseTitleComboBox.Text == "Mrs." &&  spouseLastTextBox.Text == "")
            {
                // Change the spouse's name to match the client's
                spouseLastTextBox.Text = clientLastTextBox.Text;
            }

            changeOccurred = true;
        }

        private void spouseMiddleTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void spouseLastTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void addressUnknownCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            addressTextBox.Enabled = !addressUnknownCheckBox.Checked;
            zipTextBox.Enabled = !addressUnknownCheckBox.Checked;
            cityTextBox.Enabled = !addressUnknownCheckBox.Checked;
            stateComboBox.Enabled = !addressUnknownCheckBox.Checked;
            countyTextBox.Enabled = !addressUnknownCheckBox.Checked;
            
            changeOccurred = true;
        }

        private void addressTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void zipTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void cityTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void countyTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void stateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void phoneTextBox_TextChanged(object sender, EventArgs e)
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

        private void notesTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void referredByTextBox_TextChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }

        private void estatePlannerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            changeOccurred = true;
        }
    }
}
