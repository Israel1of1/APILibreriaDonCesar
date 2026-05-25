using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.DTOs
{
    public class CreateSaleDto
    {
        [Required(ErrorMessage = " El id del cliente es abligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = " El id del cliente no es valido")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "El Id del usuario es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del Usuario no es valido.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = " La venta debe incluir un detalle de compra")]
        [MinLength(1, ErrorMessage = "El detalle debe incluir el menos un producto")]
        public List<CreateSaleDetailDto> Details { get; set; }


    }
}
