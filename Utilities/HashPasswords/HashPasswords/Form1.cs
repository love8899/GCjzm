using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using Wfm.Services.Security;
using Wfm.Core.Domain.Security;
using System.Text;

namespace HashPasswords
{
    public partial class Form1 : Form
    {
        private string _connectionStr;
        private IEncryptionService _encryptionService;
        public Form1()
        {
            InitializeComponent();
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            txtUser.Enabled = rdbSqlAuth.Checked;
            txtPassword.Enabled = rdbSqlAuth.Checked;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (TestConnection())
                MessageBox.Show("Connection string is valid", "Success");
            else
                MessageBox.Show("Connection string is invalid!", "Failed");
        }

        private bool TestConnection()
        {
            _connectionStr = this.BuildConnectionString();
            if (_connectionStr == null)
                return false;

            using (SqlConnection connection = new SqlConnection(_connectionStr))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        private string BuildConnectionString()
        {
            if (String.IsNullOrWhiteSpace(txtServer.Text) || String.IsNullOrWhiteSpace(txtDB.Text))
                return null;

            if (rdbSqlAuth.Checked && (String.IsNullOrWhiteSpace(txtUser.Text) || String.IsNullOrWhiteSpace(txtPassword.Text))   )
                return null;

            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

            builder["Data Source"] = txtServer.Text.Trim();
            builder["Initial Catalog"] = txtDB.Text.Trim();
            builder["integrated Security"] = !rdbSqlAuth.Checked;

            if (rdbSqlAuth.Checked)
            {
                builder.UserID = txtUser.Text.Trim();
                builder.Password = txtPassword.Text.Trim();

            }

            return builder.ConnectionString;
        }

        private void btnHash_Click(object sender, EventArgs e)
        {
            StringBuilder query = new StringBuilder(@"Select Id, Password 
                             From Candidate
                             Where UserName is not Null 
                               and Password is not Null 
                               and PasswordFormatId = 0
                             ");
            if (rdbOneCandidate.Checked && !String.IsNullOrWhiteSpace(txtCandidateId.Text) )
            {
                query.Append(String.Format(" and Id = {0}", txtCandidateId.Text.Trim()));
            }

            const string updateSql = @"Update Candidate
                                       Set PasswordFormatId = 1, 
                                           PasswordSalt = @salt,
                                           Password = @newPassword
                                      Where Id = @Id";

            string salt;  string newPassword;

            if (TestConnection())
            {
                _encryptionService = new EncryptionService(new SecuritySettings());
                int counter = 0;
                int failed = 0;

                using (SqlConnection connection = new SqlConnection(_connectionStr))
                {
                    SqlCommand readCmd = new SqlCommand(query.ToString(), connection);

                    using (SqlConnection connection2 = new SqlConnection(_connectionStr))
                    {
                        SqlCommand updateCmd = new SqlCommand(updateSql, connection2);
                        updateCmd.Parameters.Add(new SqlParameter("Id", 0));
                        updateCmd.Parameters.Add(new SqlParameter("salt", String.Empty));
                        updateCmd.Parameters.Add(new SqlParameter("newPassword", String.Empty));

                        connection.Open();
                        connection2.Open();

                        SqlDataReader reader = readCmd.ExecuteReader();

                        // Call Read before accessing data.
                        while (reader.Read())
                        {
                            int _id  = Convert.ToInt32(reader["Id"]);
                            
                            HashPasswordForOneCandidate((IDataRecord)reader, out salt, out newPassword);

                            // Update the row
                            updateCmd.Parameters["Id"].Value = _id;
                            updateCmd.Parameters["salt"].Value = salt;
                            updateCmd.Parameters["newPassword"].Value = newPassword;

                            if (updateCmd.ExecuteNonQuery() == 1)
                            {
                                counter++;
                                lblProcess.Text = String.Format("Updated CandidateId {0}. Row count = {1}", _id, counter);
                                lblProcess.Refresh();
                            }
                            else
                                failed++;
                        }

                        // Call Close when done reading.
                        reader.Close();
                    }

                }

                MessageBox.Show(String.Format("{0} records updated successfuly. {1} records were failed to update.", counter, failed), "Operation Result");

            }
            else
                MessageBox.Show("Connection string is invalid!", "Failed");
        }

        private void HashPasswordForOneCandidate(IDataRecord record, out string salt, out string newPassword)
        {
            int _id = Convert.ToInt32(record["Id"]);
            string _pw = record["Password"].ToString();

            salt = _encryptionService.CreateSaltKey(5);
            newPassword = _encryptionService.CreatePasswordHash(_pw, salt);
        }

        private void rdbOneCandidate_CheckedChanged(object sender, EventArgs e)
        {
            txtCandidateId.Enabled = rdbOneCandidate.Checked;
        }


    }
}
