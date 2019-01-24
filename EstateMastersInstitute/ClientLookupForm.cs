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
                    "UPPER(first_name_1) =\'" + name_chunk_1.ToUpper() + "\' OR " +
                    "UPPER(first_name_2) =\'" + name_chunk_1.ToUpper() + "\' OR " +
                    "UPPER(last_name_1) =\'" + name_chunk_1.ToUpper() + "\' OR " +
                    "UPPER(last_name_2) =\'" + name_chunk_1.ToUpper() + "\'" +
                        (name_chunk_2 == "" ? "" : " OR " +
                        "UPPER(first_name_1) =\'" + name_chunk_2.ToUpper() + "\' OR " +
                        "UPPER(first_name_2) =\'" + name_chunk_2.ToUpper() + "\' OR " +
                        "UPPER(last_name_1) =\'" + name_chunk_2.ToUpper() + "\' OR " +
                        "UPPER(last_name_2) =\'" + name_chunk_2.ToUpper() + "\'") + 
                    ");";
                }
                // If there is a keyword, show only results where the name and keywords match
                // (First OR last OR spouse name) AND (address OR notes OR referral)
                else
                {
                    sql_query = "SELECT * FROM Clients WHERE (" +
                    "UPPER(first_name_1) =\'" + name_chunk_1.ToUpper() + "\' OR " +
                    "UPPER(first_name_2) =\'" + name_chunk_1.ToUpper() + "\' OR " +
                    "UPPER(first_name_2) =\'" + name_chunk_1.ToUpper() + "\' OR " +
                    "UPPER(last_name_1) =\'" + name_chunk_1.ToUpper() + "\' OR " +
                    "UPPER(last_name_2) =\'" + name_chunk_1.ToUpper() + "\'" +
                        (name_chunk_2 == "" ? "" : " OR " +
                        "UPPER(first_name_1) =\'" + name_chunk_2.ToUpper() + "\' OR " +
                        "UPPER(first_name_2) =\'" + name_chunk_2.ToUpper() + "\' OR " +
                        "UPPER(last_name_1) =\'" + name_chunk_2.ToUpper() + "\' OR " +
                        "UPPER(last_name_2) =\'" + name_chunk_2.ToUpper() + "\'") +
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
                    resultsListBox.Items.Add(sql_reader["client_num"].ToString() + " - " + sql_reader["first_name_1"].ToString() + " " + 
                        (sql_reader["first_name_2"].ToString() != "" || sql_reader["last_name_2"].ToString() == sql_reader["last_name_1"].ToString() ? "& " + sql_reader["first_name_2"].ToString() + " " : "") +
                        sql_reader["last_name_1"].ToString());
                }

                // Let the user know there weren't any results
                if(resultsListBox.Items.Count == 0)
                {
                    resultsListBox.Items.Add("No results found");
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

        }

        /*****************************************************************
         * Event Handler for cancelButton
         * Description: Closes the form
         * Param sender: An unused object that sends the event
         * Param e: Unused EventArgs sent to the event
         * Returns: None
         *****************************************************************/
        private void resultsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(resultsListBox.SelectedIndex >= 0)
            {
                groupBox1.Enabled = true;

            }
            else
            {
                groupBox1.Enabled = false;
            }
        }


        private void clearData()
        {
            // Clear all of the data from the summary area
            fileNumberLabel.Text = "";
            createdLabel.Text = "";
            modifiedLabel.Text = "";
            clientNameLabel.Text = "";
            spouseNameLabel.Text = "";
            addressLabel.Text = "";
            cityLabel.Text = "";
            countyLabel.Text = "";
            stateLabel.Text = "";
            zipLabel.Text = "";
            phoneLabel.Text = "";
            altPhone1Label.Text = "";
            altPhone2Label.Text = "";
            emailLabel.Text = "";
            notesLabel.Text = "";
            referredByLabel.Text = "";
            hasPlannerLabel.Text = "";
            linkedFileLinkLabel.Text = "No file attached...";
        }


        private void fillData(string file_num)
        {
            // Run a SQL query, get the data, fill the fields
        }

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
    }
}
