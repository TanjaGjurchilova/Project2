using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string City { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        [ForeignKey("FK_Role")]
        public virtual Role UserRole { get; set; }
        public bool Active { get; set; }
        public bool Appruved { get; set; }
        public bool CompanyUser { get; set; }
        public int RoleId { get; set; }
        public int IndustryId { get; set; }
        public int CompanyId { get; set; }
        public int CountryId { get; set; }
    }
}
