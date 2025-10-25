using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamApp.ViewModels
{
    public class LoginModel
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [DataType(DataType.Password)]   
        public string Password { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }    

    }
    public class RegisterModel
    {

        [Required]
        [StringLength(100)]

        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
      
}
      