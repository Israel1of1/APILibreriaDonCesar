using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibreriaDonCesar.Business.DTOs
{
    public class CreatePresentationDto
    {
        [Required]
        [StringLength(60, ErrorMessage = "El nombre de la presentación no debe exceder los 60 caracteres.")]
        public string? PresentationName { get; set; }
        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "La cantidad no es valida.")]
        public decimal Amount { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El Id de la unidad de medida no es valido.")]
        public int UnitMeasureId { get; set; }
        [StringLength(50, ErrorMessage = "El factor de unidad no debe exceder los 50 caracteres.")]
        [Required]
        public string? UnitFactor { get; set; }
    }
}
