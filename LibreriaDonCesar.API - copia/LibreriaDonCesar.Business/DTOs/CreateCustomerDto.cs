using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibreriaDonCesar.Business.DTOs
{
    public class CreateCustomerDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "El nombre del cliente no debe exceder los 50 caracteres.")]
        public string CustomerName { get; set; }
        [Required]

        [StringLength(50, ErrorMessage = "El tipo de cliente no debe exceder los 50 caracteres.")]

        public string CustomerType { get; set; }
    }
}
