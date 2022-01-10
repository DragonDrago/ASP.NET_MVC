using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DAL.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductDescription { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }

        
        public virtual Category Category { get; set; }

    }

    
}
