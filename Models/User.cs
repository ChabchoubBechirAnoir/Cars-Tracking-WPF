using System;

namespace MapFollow.Models
{
    public class User
    {
        User () { }
        public User (string mail, string password)
        {
            this.Mail = mail;
            this.Password = password;
        }

        public User(string firstName, string lastName, string mail, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Mail = mail;
            Password = password;
            UserGroupId = 1;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public int UserGroupId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
    }
}
