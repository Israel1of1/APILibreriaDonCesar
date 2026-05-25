using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(60, ErrorMessage = "El nombre del producto no debe exceder los 50 caracteres.")]
        public string? ProductName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El Id de la categoria no es valido.")]
        [DefaultValue(0)]
        public int CategoryId { get; set; } 

        [Range(1, int.MaxValue, ErrorMessage = "El Id de la presentacion.")]
        [DefaultValue(0)]
        public int PresentationId { get; set; } = 0;

        [StringLength(60, ErrorMessage = "El nombre de la marca no debe exceder los 50 caracteres.")]
        public string Brand { get; set; }
        [StringLength(60, ErrorMessage = "El nombre del color no debe exceder los 50 caracteres.")]
        public string Color { get; set; }
        [StringLength(200, ErrorMessage = "La descripción no debe exceder los 100 caracteres.")]
        public string? Description { get; set; }
        public bool State { get; set; }
    }
}
