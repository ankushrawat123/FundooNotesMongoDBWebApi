using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatabaseLayer.User
{
    public class PasswordPostModel
    {
        [Required]
        [RegularExpression("^(?=.*[A-Z])[A-Z a-z 0-9 $#@!&*?|]{5,}$", ErrorMessage = "Password Have minimum 5 Characters, Should have at least 1 Upper Case and Should have numeric number and Has Special Character")]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^(?=.*[A-Z])[A-Z a-z 0-9 $#@!&*?|]{5,}$", ErrorMessage = "Password Have minimum 5 Characters, Should have at least 1 Upper Case and Should have numeric number and Has Special Character")]
        public string ConfirmPassword { get; set; }
    }
}
