using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using ContactBook.Models;

namespace ContactBook.DataAccess
{
    public class DataAccessLayer {
        string filePath = @"C:\Users\Hackerman\source\repos\ContactBook\ContactBook\SQL\procedures.sql";
        private string connectionString;

        public DataAccessLayer(string connectionString) {
            this.connectionString = connectionString;
        }

        public void SaveContact(ContactModel contact, bool isInsert) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                SqlCommand command = new SqlCommand(isInsert ? "InsertContact" : "UpdateContact", connection);
                command.CommandType = CommandType.StoredProcedure;

                if (!isInsert) {
                    command.Parameters.AddWithValue("@ContactID", contact.ContactID); 
                }

                command.Parameters.AddWithValue("@FullName", contact.Fullname);
                command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                command.Parameters.AddWithValue("@DateOfBirth", contact.DateOfBirth);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteContact(int contactID) {
            try {
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DeleteContact", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ContactID", contactID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Error while deleting contact: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public List<ContactModel> GetContacts() {
            List<ContactModel> contacts = new List<ContactModel>();

            try {
                string sqlScript = File.ReadAllText(filePath);
                string[] sqlStatements = sqlScript.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    connection.Open();

                    foreach (string sqlStatement in sqlStatements) {
                        SqlCommand command = new SqlCommand(sqlStatement, connection);
                        command.ExecuteNonQuery();
                    }

                    SqlCommand selectCommand = new SqlCommand("SELECT ContactID, FullName, PhoneNumber, DateOfBirth FROM dbo.Contacts", connection);
                    using (SqlDataReader reader = selectCommand.ExecuteReader()) {
                        while (reader.Read()) {
                            ContactModel contact = new ContactModel {
                                ContactID = (int)reader["ContactID"],
                                Fullname = reader["FullName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                DateOfBirth = (DateTime)reader["DateOfBirth"]
                            };

                            contacts.Add(contact);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error while retrieving contacts: " + ex.Message);
            }

            return contacts;
        }
    }
}
