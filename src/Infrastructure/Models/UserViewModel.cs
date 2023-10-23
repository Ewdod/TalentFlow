using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class UserViewModel
    {
        // TODO: burasi bugli burasi duzeltilecek

        //[FileExtensions(Extensions = ".jpg,.jpeg,.png", ErrorMessage = "We only accept .jpg, .png and .jpeg formats")]
        public IFormFile? Photo { get; set; }

        [RegularExpression(@"^\+90\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number in the format +90xxxxxxxxxx.")]
        [MaxLength(13)]
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
