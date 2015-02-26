using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class User : Entity
    {
        public User()
        {
            //AddRoles = new List<AppRole>();
            Status = EmployeeStatus.Work;
        }

        public virtual int EmployeeID { get; set; }
        public virtual TitleName Title { get; set; }
        public virtual string FirstNameEN { get; set; }
        public virtual string LastNameEN { get; set; }
        public virtual string FirstNameTH { get; set; }
        public virtual string LastNameTH { get; set; }
        public virtual string Nickname { get; set; }
        public virtual string Email { get; set; }

        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }

        public virtual string Password { get; protected set; }
        public virtual string LastLoginIP { get; set; }
        public virtual string LastLockoutIP { get; set; }
        public virtual DateTime? LastLoginDate { get; set; }
        public virtual DateTime? LastActivityDate { get; set; }
        public virtual DateTime? LastPasswordChangedDate { get; set; }
        public virtual DateTime? LastLockoutDate { get; set; }

        public virtual Department Department { get; set; }

        public virtual Position Position { get; set; }

        public virtual User Lead { get; set; }

        public virtual bool IsLockedOut { get; set; }

        /// <summary>
        /// ทำงานอยู่ (Work or Null) หรือ ลาออก (Resign)
        /// </summary>
        public virtual EmployeeStatus Status { get; set; }

        //public virtual IList<AppRole> AddRoles { get; protected set; }

        //public virtual IList<Project> Projects { get; protected set; }
        public virtual string FullName 
        {
            get
            {
                return string.Format("{0} {1}", FirstNameTH, LastNameTH);
            }
        }

        public virtual void SetPassword(string password)
        {
            using (var md5Hash = SHA256.Create())
            {
                string hash = GetSHA256Hash(md5Hash, password);
                Password = hash;
            }
        }

        string GetSHA256Hash(SHA256 sha256Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        // Verify a hash against a string. 
        public virtual bool VerifyPassword(string input)
        {
            using (var md5Hash = SHA256.Create())
            {
                // Hash the input. 
                string hashOfInput = GetSHA256Hash(md5Hash, input);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, Password))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
