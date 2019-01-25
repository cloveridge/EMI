using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EstateMastersInstitute
{
    public partial class ClientLookupForm : Form
    {
        string clientFile = "";

        public ClientLookupForm()
        {
            InitializeComponent();
            clearData();
            groupBox1.Enabled = false;
            numberSearchTextBox.Focus();
        }

        /*****************************************************************
         * Event Handler for closeButton
         * Description: Closes the form
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        /*****************************************************************
         * Event Handler for searchButton
         * Description: Searches database by criteria in the 3 search fields
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void searchButton_Click(object sender, EventArgs e)
        {
            // Filter if the search criteria is blank
            if (numberSearchTextBox.Text == "" && nameSearchTextBox.Text == "" && keywordSearchTextBox.Text == "")
            {
                MessageBox.Show("Please enter a number, name, or a keyword to search for.");
                return;
            }

            string sql_query = "";
            string name_chunk_1 = "";
            string name_chunk_2 = "";
            SQLiteConnection db_connect;
            SQLiteCommand db_comm;
            db_connect = new SQLiteConnection("Data Source=emi.db;Version=3;");
            db_connect.Open();

            // Clear any existing results out of the search results listbox
            resultsListBox.Items.Clear();

            // Otherwise, Launch the SQL search:
            // If the number field is used, just search for that.
            if (numberSearchTextBox.Text != "")
            {
                sql_query = "SELECT * FROM Clients WHERE client_num = " + numberSearchTextBox.Text + ";";
            }
            // Else, If the name field is used, first separate the names for searching.
            else if (nameSearchTextBox.Text != "")
            {
                // Break the nameBox up into chunks
                name_chunk_1 = "";
                name_chunk_2 = "";
                if(nameSearchTextBox.Text.IndexOf(" ") == -1)
                {
                    name_chunk_1 = nameSearchTextBox.Text;
                }
                else
                {
                    name_chunk_1 = nameSearchTextBox.Text.Substring(0, nameSearchTextBox.Text.IndexOf(" "));
                    name_chunk_2 = nameSearchTextBox.Text.Substring(nameSearchTextBox.Text.IndexOf(" ") + 1, nameSearchTextBox.Text.Length-(nameSearchTextBox.Text.IndexOf(" ") + 1));
                }

                // Then, see if they are also slicing by keyword. If not, search just by name.
                if(keywordSearchTextBox.Text == "")
                {
                    sql_query = "SELECT * FROM Clients WHERE (" +
                    "UPPER(first_name_1) LIKE \'%" + name_chunk_1.ToUpper() + "%\' OR " +
                    "UPPER(first_name_2) LIKE \'%" + name_chunk_1.ToUpper() + "%\' OR " +
                    "UPPER(last_name_1) LIKE \'%" + name_chunk_1.ToUpper() + "%\' OR " +
                    "UPPER(last_name_2) LIKE \'%" + name_chunk_1.ToUpper() + "%\'" +
                        (name_chunk_2 == "" ? "" : " OR " +
                        "UPPER(first_name_1) LIKE \'%" + name_chunk_2.ToUpper() + "%\' OR " +
                        "UPPER(first_name_2) LIKE \'%" + name_chunk_2.ToUpper() + "%\' OR " +
                        "UPPER(last_name_1) LIKE \'%" + name_chunk_2.ToUpper() + "%\' OR " +
                        "UPPER(last_name_2) LIKE \'%" + name_chunk_2.ToUpper() + "%\'") + 
                    ");";
                }
                // If there is a keyword, show only results where the name and keywords match
                // (First OR last OR spouse name) AND (address OR notes OR referral)
                else
                {
                    sql_query = "SELECT * FROM Clients WHERE (" +
                    "UPPER(first_name_1) LIKE \'%" + name_chunk_1.ToUpper() + "%\' OR " +
                    "UPPER(first_name_2) LIKE \'%" + name_chunk_1.ToUpper() + "%\' OR " +
                    "UPPER(first_name_2) LIKE \'%" + name_chunk_1.ToUpper() + "%\' OR " +
                    "UPPER(last_name_1) LIKE \'%" + name_chunk_1.ToUpper() + "%\' OR " +
                    "UPPER(last_name_2) LIKE \'%" + name_chunk_1.ToUpper() + "%\'" +
                        (name_chunk_2 == "" ? "" : " OR " +
                        "UPPER(first_name_1) LIKE \'%" + name_chunk_2.ToUpper() + "%\' OR " +
                        "UPPER(first_name_2) LIKE \'%" + name_chunk_2.ToUpper() + "%\' OR " +
                        "UPPER(last_name_1) LIKE \'%" + name_chunk_2.ToUpper() + "%\' OR " +
                        "UPPER(last_name_2) LIKE \'%" + name_chunk_2.ToUpper() + "%\'") +
                    ") AND (UPPER(notes) LIKE \'%" + keywordSearchTextBox.Text.ToUpper() + "%\' OR " +
                    "UPPER(address) LIKE \'%" + keywordSearchTextBox.Text.ToUpper() + "%\' OR " +
                    "UPPER(referred_by) LIKE \'%" + keywordSearchTextBox.Text.ToUpper() + "%\');";
                }
            }

            // Else, just search for the keyword in the notes, referral, and address. (OR)
            else
            {
                sql_query = "SELECT * FROM Clients WHERE (UPPER(notes) LIKE \'%" + keywordSearchTextBox.Text.ToUpper() + "%\' OR " +
                    "UPPER(address) LIKE \'%" + keywordSearchTextBox.Text.ToUpper() + "%\' OR " +
                    "UPPER(referred_by) LIKE \'%" + keywordSearchTextBox.Text.ToUpper() + "%\');";
            }

            // Run the search, and add the results to the list
            try
            {
                db_comm = new SQLiteCommand(sql_query, db_connect);
                SQLiteDataReader sql_reader = db_comm.ExecuteReader();
                while(sql_reader.Read())
                {
                    // Save results in the listbox as FileNumber & client and spouse name.
                    // 1000 - Testy & Testee McTesterson
                    resultsListBox.Items.Add(string.Format("{0:D4}", int.Parse(sql_reader["client_num"].ToString())) + " - " + sql_reader["first_name_1"].ToString() + " " + 
                        (sql_reader["first_name_2"].ToString() != "" || sql_reader["last_name_2"].ToString() == sql_reader["last_name_1"].ToString() ? "& " + sql_reader["first_name_2"].ToString() + " " : "") +
                        sql_reader["last_name_1"].ToString());
                }

                // Let the user know there weren't any results
                if(resultsListBox.Items.Count == 0)
                {
                    resultsListBox.Items.Add("No results found");
                }
                else
                {
                    // Select top result
                    resultsListBox.SelectedIndex = 0;
                }
                
            }
            catch(Exception exc)
            {
                // Write error to console
                Console.WriteLine(exc);
            }

            db_connect.Close();

        }

        /*****************************************************************
         * Event Handler for clearButton
         * Description: Clears the search fields
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void clearButton_Click(object sender, EventArgs e)
        {
            keywordSearchTextBox.Text = "";
            numberSearchTextBox.Text = "";
            nameSearchTextBox.Text = "";
            resultsListBox.Items.Clear();
            groupBox1.Enabled = false;
        }

        /*****************************************************************
         * Event Handler for cancelButton
         * Description: Closes the form
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void editClientButton_Click(object sender, EventArgs e)
        {
            EditClientForm editing_form = new EditClientForm(resultsListBox.SelectedItem.ToString().Substring(0, resultsListBox.SelectedItem.ToString().IndexOf(" ")));
            editing_form.StartPosition = FormStartPosition.CenterParent;
            editing_form.ShowDialog();

            fillData(resultsListBox.SelectedItem.ToString().Substring(0, resultsListBox.SelectedItem.ToString().IndexOf(" ")));
        }

        /*****************************************************************

         * Event Handler for resultsListBox
         * Description: When the user selects an item from the list, runs
         *              a SQL query to pull the data into the preview

         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void resultsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(resultsListBox.SelectedIndex >= 0 && resultsListBox.SelectedItem.ToString() != "No results found")
            {
                groupBox1.Enabled = true;

                fillData(resultsListBox.SelectedItem.ToString().Substring(0, resultsListBox.SelectedItem.ToString().IndexOf(" ")));
            }
            else
            {
                groupBox1.Enabled = false;
            }
        }


        /*****************************************************************
         * clearData method
         * Description: Resets the text in the summary fields
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void clearData()
        {
            // Clear all of the data from the summary area
            fileNumberLabel.Text = "";
            createdLabel.Text = "";
            modifiedLabel.Text = "";
            clientNameLabel.Text = "";
            spouseNameLabel.Text = "";
            addressLabel.Text = "";
            phoneLabel.Text = "";
            altPhone1Label.Text = "";
            altPhone2Label.Text = "";
            emailLabel.Text = "";
            notesLabel.Text = "";
            referredByLabel.Text = "";
            hasPlannerLabel.Text = "";
            linkedFileLinkLabel.Text = "No file attached...";
        }


        /*****************************************************************
         * fillData method
         * Description: When the user selects an item from the list, runs
         *              a SQL query to pull the data into the preview
         * Param file_num: The file number to search for
         * Returns: None
         *****************************************************************/
        private void fillData(string file_num)
        {
            // Run a SQL query, get the data, fill the fields
            SQLiteConnection db_connect;
            SQLiteCommand db_comm;
            db_connect = new SQLiteConnection("Data Source=emi.db;Version=3;");
            db_connect.Open();

            string sql_query = "SELECT * FROM Clients WHERE client_num = " + file_num + ";";
            //string sql_query = "SELECT * FROM Clients WHERE client_num = " + numberSearchTextBox.Text + ";";

            db_comm = new SQLiteCommand(sql_query, db_connect);
            SQLiteDataReader sql_reader = db_comm.ExecuteReader();
            while (sql_reader.Read())
            {
                fileNumberLabel.Text = string.Format("{0:0000}", int.Parse(file_num));
                createdLabel.Text = sql_reader["created_date"].ToString().Substring(0,10);
                modifiedLabel.Text = sql_reader["modified_date"].ToString().Substring(0, 10);
                clientNameLabel.Text = sql_reader["title_1"].ToString() + " " + sql_reader["first_name_1"].ToString() + " " + sql_reader["last_name_1"].ToString();
                spouseNameLabel.Text = (sql_reader["first_name_2"].ToString() == "" ? "" : sql_reader["title_2"].ToString() + " " + sql_reader["first_name_2"].ToString() + " " + sql_reader["last_name_2"].ToString());
                addressLabel.Text = sql_reader["address"].ToString() + " \n" + sql_reader["city"].ToString() + ", " + sql_reader["state"].ToString() + " " + sql_reader["zip"].ToString() + " \n" + sql_reader["county"].ToString() + " County";
                phoneLabel.Text = sql_reader["main_phone"].ToString();
                altPhone1Label.Text = sql_reader["alt_phone_1"].ToString();
                altPhone2Label.Text = sql_reader["alt_phone_2"].ToString();
                emailLabel.Text = sql_reader["email"].ToString();
                notesLabel.Text = sql_reader["notes"].ToString();
                referredByLabel.Text = sql_reader["referred_by"].ToString();
                hasPlannerLabel.Text = (sql_reader["has_estate_planner"].ToString() == "T" ? "Yes" : "No");
                linkedFileLinkLabel.Text = sql_reader["attached_file"].ToString();
            }
            
            db_connect.Close();
        }

        /*****************************************************************
         * Event Handler for linkLabel
         * Description: Opens the client's linked file, if exists.
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void linkedFileLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (clientFile != "")
            {
                try
                {
                    // open the linked file
                    System.Diagnostics.Process.Start(clientFile);
                }
                catch (Exception exc)
                {
                    MessageBox.Show("There was an error because the file could not be found. Please ensure the file exists, and re-attach it in the edit screen.");
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete the selected file? This operation cannot be undone.","Delete Client",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
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
                    command_text = "DELETE FROM Clients WHERE client_num = " + resultsListBox.SelectedItem.ToString().Substring(0, resultsListBox.SelectedItem.ToString().IndexOf(" ")) + ";";

                    db_comm = new SQLiteCommand(command_text, db_connect);
                    db_comm.ExecuteNonQuery();
                    db_connect.Close();

                    resultsListBox.Items.RemoveAt(resultsListBox.SelectedIndex);

                }
                catch(Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
                db_connect.Close();
            }
        }

        private void newClientButton_Click(object sender, EventArgs e)
        {
            EditClientForm editing_form = new EditClientForm();
            editing_form.StartPosition = FormStartPosition.CenterParent;
            editing_form.ShowDialog();
        }
    }
}
