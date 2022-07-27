using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatabaseLayer.User
{
    public class UserModel
    {
        [Required]
        [RegularExpression("^[A-Z][a-z A-Z 0-9]{3,}$",ErrorMessage ="Minimum 4 characters Required and First Letter In UpperCase")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression("^[A-Z][a-z A-Z 0-9]{3,}$",ErrorMessage = "Minimum 4 characters Required and First Letter In UpperCase")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^[a-z A-Z 0-9 !#$%^&*?]+[@][a-z A-Z]{3,}[.][a-z]{2,5}$",ErrorMessage ="Enter Valid Email")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[A-Z])[A-Z a-z 0-9 @#$%^&*!]{4,}$",ErrorMessage ="Atleast one letter in upper case and minimum 4 characters")]
        public string Password { get; set; }
    }
}
