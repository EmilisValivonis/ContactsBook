using System;

namespace ContactBook.Models
{
    public class ContactModel
    {
        public int ContactID { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; } 
    }
}
