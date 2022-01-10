using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        [StringLength(450)]
        [DataType(DataType.EmailAddress)]
        [Index(IsUnique = true)]
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
