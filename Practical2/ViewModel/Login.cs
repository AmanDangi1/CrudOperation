using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Practical2.ViewModel
{
    public class Login
    {
        [Required(ErrorMessage = "Please Enter Your UserName")]
        [Display(Name = "User Name")]
        public string UserName {get; set;}
        


        [Required(ErrorMessage = "Please Enter Pasword")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password {get; set;}
    }
}