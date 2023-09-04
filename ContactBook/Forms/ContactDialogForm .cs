using ContactBook.DataAccess;
using ContactBook.Models;
using System;
using System.Windows.Forms;

namespace ContactBook.Forms
{
    public partial class ContactDialogForm : Form
    {
        private DataAccessLayer dataAccess;
        private ContactModel contactModel;

        private Label labelFullname;
        private TextBox textBoxFullname;

        private Label labelPhoneNumber;
        private TextBox textBoxPhoneNumber;

        private Label labelDateOfBirth;
        private DateTimePicker dateTimePickerDateOfBirth;

        private Button buttonOK;
        private Button buttonCancel;

        public ContactDialogForm(DataAccessLayer dataAccess, ContactModel contact = null)  {
            InitializeComponent();
            this.dataAccess = dataAccess;
            contactModel = contact;
            InitializeControls();

            StartPosition = FormStartPosition.CenterScreen;

            // Populate the form with existing contact details if in edit mode
            if (contactModel != null) {
                Text = "Edit Contact";
                textBoxFullname.Text = contactModel.Fullname;
                textBoxPhoneNumber.Text = contactModel.PhoneNumber;
                dateTimePickerDateOfBirth.Value = contactModel.DateOfBirth;
            }
        }

        private void InitializeControls() {
            // Set form properties
            Text = contactModel == null ? "Add Contact" : "Edit Contact";
            Size = new System.Drawing.Size(300, 240);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Create and configure controls
            labelFullname = new Label();
            labelFullname.Text = "Full Name:";
            labelFullname.Location = new System.Drawing.Point(20, 20);

            textBoxFullname = new TextBox();
            textBoxFullname.Location = new System.Drawing.Point(120, 20);
            textBoxFullname.Size = new System.Drawing.Size(150, 20);
            textBoxFullname.KeyPress += TextBoxFullname_KeyPress;
            textBoxFullname.TextChanged += TextBoxFullname_TextChanged;

            labelPhoneNumber = new Label();
            labelPhoneNumber.Text = "Phone Number:";
            labelPhoneNumber.Location = new System.Drawing.Point(20, 50);

            textBoxPhoneNumber = new TextBox();
            textBoxPhoneNumber.Location = new System.Drawing.Point(120, 50);
            textBoxPhoneNumber.Size = new System.Drawing.Size(150, 20);
            textBoxPhoneNumber.Text = "+370-6"; // Initial text
            textBoxPhoneNumber.KeyPress += TextBoxPhoneNumber_KeyPress;

            labelDateOfBirth = new Label();
            labelDateOfBirth.Text = "Date of Birth:";
            labelDateOfBirth.Location = new System.Drawing.Point(20, 80);

            dateTimePickerDateOfBirth = new DateTimePicker();
            dateTimePickerDateOfBirth.Location = new System.Drawing.Point(120, 80);
            dateTimePickerDateOfBirth.Width = 150;
            dateTimePickerDateOfBirth.Format = DateTimePickerFormat.Short;

            buttonOK = new Button();
            buttonOK.Text = "OK";
            buttonOK.Location = new System.Drawing.Point(50, 120);
            buttonOK.Click += ButtonOK_Click;

            buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.Location = new System.Drawing.Point(150, 120);
            buttonCancel.Click += ButtonCancel_Click;

            // Improve button styling
            buttonOK.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            buttonOK.ForeColor = System.Drawing.Color.White;
            buttonOK.FlatStyle = FlatStyle.Flat;
            buttonOK.FlatAppearance.BorderSize = 0;
            buttonCancel.BackColor = System.Drawing.Color.FromArgb(200, 0, 0);
            buttonCancel.ForeColor = System.Drawing.Color.White;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.FlatAppearance.BorderSize = 0;

            // Add controls to the form
            Controls.Add(labelFullname);
            Controls.Add(textBoxFullname);
            Controls.Add(labelPhoneNumber);
            Controls.Add(textBoxPhoneNumber);
            Controls.Add(labelDateOfBirth);
            Controls.Add(dateTimePickerDateOfBirth);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
        }
 
        private void TextBoxPhoneNumber_KeyPress(object sender, KeyPressEventArgs e) {
            if (!textBoxPhoneNumber.Text.StartsWith("+370-6")) {
                textBoxPhoneNumber.Text = "+370-6";
            }
            // Check the character count and cancel input if it reaches 12 digits
            if (textBoxPhoneNumber.Text.Replace("-", "").Length >= 12 && !char.IsControl(e.KeyChar)) {
                e.Handled = true;
            }

            // Allow only numeric digits and control keys (backspace)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) {
                e.Handled = true;
            }
            // Move the cursor to the end of the text
            textBoxPhoneNumber.SelectionStart = textBoxPhoneNumber.Text.Length;
        }
        private void ButtonOK_Click(object sender, EventArgs e) {
            ContactModel contactInfo = GetContactInfo();

            if (contactInfo != null) {
                if (contactModel == null) {
                    // Add a new contact
                    dataAccess.SaveContact(contactInfo, true);
                }
                else {
                    // Update an existing contact
                    contactInfo.ContactID = contactModel.ContactID;
                    dataAccess.SaveContact(contactInfo, false);
                }

                MainForm mainForm = Application.OpenForms["MainForm"] as MainForm;
                if (mainForm != null) {
                    mainForm.RefreshDataGridView();
                }

               DialogResult = DialogResult.OK;
               Close();
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e) {
           DialogResult = DialogResult.Cancel;
           Close();
        }
        private void TextBoxFullname_KeyPress(object sender, KeyPressEventArgs e) {
            // Allow only alphabetic characters, space, and backspace
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != (char)Keys.Back) {
                e.Handled = true;
            }
        }
        private void TextBoxFullname_TextChanged(object sender, EventArgs e) {
            // Enforce "Name Surname" format
            string input = textBoxFullname.Text.Trim();
            string[] names = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (names.Length >= 2) {
                textBoxFullname.ForeColor = System.Drawing.Color.Black;
            }  else {
                textBoxFullname.ForeColor = System.Drawing.Color.Red;
            }
        }
        private ContactModel GetContactInfo() {
            // if the name input follows the "Name Surname" format
            string input = textBoxFullname.Text.Trim();
            string[] names = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (names.Length < 2) {
                MessageBox.Show("Please enter a valid full name");
                return null;
            }

            // if the phone number input is valid
            string phoneNumberInput = textBoxPhoneNumber.Text.Trim();
            if (!phoneNumberInput.StartsWith("+370-6") || phoneNumberInput.Length != 13)  {
                MessageBox.Show("Please enter a valid phone number");
                return null;
            }

            // Check if the date of birth is greater than the current date
            DateTime currentDate = DateTime.Now;
            DateTime selectedDate = dateTimePickerDateOfBirth.Value.Date;

            if (selectedDate > currentDate) {
                MessageBox.Show("We do not register time travelers");
                return null;
            }

            ContactModel contactInfo = new ContactModel {
                Fullname = textBoxFullname.Text,
                PhoneNumber = textBoxPhoneNumber.Text, 
                DateOfBirth = dateTimePickerDateOfBirth.Value
            };

            return contactInfo;
        }
    }
}
