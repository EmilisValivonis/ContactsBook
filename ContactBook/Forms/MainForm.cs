using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ContactBook.Forms;
using ContactBook.DataAccess;
using ContactBook.Models;
using System.Drawing;

namespace ContactBook {
    public partial class MainForm : Form {
        //Constants
        private const string DataSource = "DESKTOP-5TKTFP1\\SQLEXPRESS";
        private const string InitialCatalog = "ContactsDB";
        private const string IntegratedSecurity = "True";

        private const int EditColumnIndex = 0;
        private const int DeleteColumnIndex = 1;

        private static readonly string connectionString =
            $"Data Source={DataSource};Initial Catalog={InitialCatalog};Integrated Security={IntegratedSecurity}";

        private DataAccessLayer dataAccess;
        private DataGridView dataGridView;

        public MainForm() {
            InitializeComponent();

            dataAccess = new DataAccessLayer(connectionString);

            InitializeFormControls();

            Text = "Contacts Book";

            Size = new Size(800, 600);

            WindowState = FormWindowState.Maximized;

            BackColor = Color.LightBlue;
        }

        private void InitializeFormControls() {
            StartPosition = FormStartPosition.CenterScreen;

            dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.White;

            Controls.Add(dataGridView);

            DataGridViewTextBoxColumn fullNameColumn = new DataGridViewTextBoxColumn();
            fullNameColumn.DataPropertyName = "Fullname"; 
            fullNameColumn.HeaderText = "Full Name";
            dataGridView.Columns.Add(fullNameColumn);

            DataGridViewTextBoxColumn phoneNumberColumn = new DataGridViewTextBoxColumn();
            phoneNumberColumn.DataPropertyName = "PhoneNumber";
            phoneNumberColumn.HeaderText = "Phone Number";
            dataGridView.Columns.Add(phoneNumberColumn);

            DataGridViewTextBoxColumn dateOfBirthColumn = new DataGridViewTextBoxColumn();
            dateOfBirthColumn.DataPropertyName = "DateOfBirth"; 
            dateOfBirthColumn.HeaderText = "Date of Birth";
            dataGridView.Columns.Add(dateOfBirthColumn);

            //Create the Add new contact button
            Button addButton = new Button();
            addButton.Text = "Add new contact";
            addButton.Click += AddButton_Click;
            addButton.FlatStyle = FlatStyle.Flat;
            addButton.BackColor = Color.FromArgb(0, 123, 255);
            addButton.ForeColor = Color.White;
            addButton.Width = 200;

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();

            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.AutoSize = true;

            buttonPanel.Controls.Add(addButton);
            Controls.Add(buttonPanel);

            // Create the "Edit" button column
            DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
            editButtonColumn.Text = "Edit";
            editButtonColumn.UseColumnTextForButtonValue = true;
            editButtonColumn.FlatStyle = FlatStyle.Flat;
            editButtonColumn.DefaultCellStyle.BackColor = Color.Green;
            editButtonColumn.DefaultCellStyle.ForeColor = Color.White;
            editButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(editButtonColumn);

            // Create the "Delete" button column
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            deleteButtonColumn.FlatStyle = FlatStyle.Flat;
            deleteButtonColumn.DefaultCellStyle.BackColor = Color.FromArgb(200, 0, 0);
            deleteButtonColumn.DefaultCellStyle.ForeColor = Color.White;
            dataGridView.Columns.Add(deleteButtonColumn);

            dataGridView.ReadOnly = true;

            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.CellContentClick += DataGridView_CellClick;
            dataGridView.ReadOnly = true;
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e) {
            int rowIndex = e.RowIndex;
            List<object> rowData = GetContactByRowIndex(rowIndex);

            object contactID = rowData[2];
            object name = rowData[3];
            object phoneNumber = rowData[4];
            object dateOfBirth = rowData[5];

            ContactModel selectedContact = new ContactModel
            {
                ContactID = Convert.ToInt32(contactID),
                Fullname = Convert.ToString(name),
                PhoneNumber = Convert.ToString(phoneNumber),
                DateOfBirth = Convert.ToDateTime(dateOfBirth)
            };

            switch (e.ColumnIndex) {
                case EditColumnIndex:
                    EditContact(selectedContact);
                    break;
                case DeleteColumnIndex:
                    DeleteContact(selectedContact);
                    break;
            }
         }
      
        private void EditContact(ContactModel selectedContact) {
            ContactDialogForm editForm = new ContactDialogForm(dataAccess, selectedContact);
            if (editForm.ShowDialog() == DialogResult.OK) {
                RefreshDataGridView();
            }
        }

        private void DeleteContact(ContactModel selectedContact) {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this contact?", "Delete Contact",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try {
                    dataAccess.DeleteContact(selectedContact.ContactID);
                    RefreshDataGridView();

                }  catch (Exception ex) {
                    MessageBox.Show("Error deleting contact: " + ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private List<object> GetContactByRowIndex(int rowIndex) {
            if (rowIndex >= 0 && rowIndex < dataGridView.Rows.Count) {
                DataGridViewRow selectedRow = dataGridView.Rows[rowIndex];
                List<object> rowData = new List<object>();

                foreach (DataGridViewCell cell in selectedRow.Cells)
                {
                    rowData.Add(cell.Value);
                }

                return rowData;
            }

            return null;
        }

        private void AddButton_Click(object sender, EventArgs e) {
            ContactDialogForm contactDialog = new ContactDialogForm(dataAccess);

            if (contactDialog.ShowDialog() == DialogResult.OK)
            {
                RefreshDataGridView();
            }
        }
        public void RefreshDataGridView() {
            // Populate the DataGridView with updated data
            List<ContactModel> contacts = dataAccess.GetContacts();
            dataGridView.DataSource = contacts;
        }

        private void MainForm_Load(object sender, EventArgs e) {
                try {
                    dataAccess = new DataAccessLayer(connectionString);
                List<ContactModel> contacts = dataAccess.GetContacts();
                dataGridView.DataSource = contacts;

                // hide the "ContactID" column since we dont need it
                dataGridView.Columns["ContactID"].Visible = false;
                RefreshDataGridView();
             

            }  catch (Exception ex) {
                    MessageBox.Show("Error initializing DataAccessLayer: " + ex.Message, "Initialization Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }  
        }
    }
}
