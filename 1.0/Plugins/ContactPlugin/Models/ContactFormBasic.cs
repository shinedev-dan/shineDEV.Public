using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContactPlugin.Models
{
    public class ContactFormBasic
    {
        [Required]
        public String Name { get; set; }
        [Display(Name = "Phone Number")]
        [RequiredIf("EmailAddress == null", ErrorMessage ="The Phone Number is required if no Email Address is provided")]
        [DataType(DataType.PhoneNumber)]
        public String PhoneNumber { get; set; }
        [RequiredIf("PhoneNumber == null", ErrorMessage = "The Email Address is required if no Phone Number is provided")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public String EmailAddress { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public String Message { get; set; }
    }
}