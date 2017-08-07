using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Models
{
    public class Entry
    {
        public int? Id { get; set; }

        [Required]
        [MinLength(length: 3, ErrorMessage = "First Name is too short")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
