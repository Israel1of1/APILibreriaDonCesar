using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.DTOs
{
    public class UpdateCustomerDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "El nombre del cliente debe exceder los 50 caracteres.")]
        public string CustomerName { get; set; }
        [StringLength(50, ErrorMessage = "El tipo de cliente no debe tener mas de 50 caracteres.")]
        public string CustomerType { get; set; }
        public bool State { get; set; }
    }
}
