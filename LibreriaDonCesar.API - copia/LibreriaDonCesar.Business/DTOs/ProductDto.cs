using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string CategoryName { get; set; }
        public int PresentationId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PresentationName { get; set; }
        public string? ProductName { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string? Description { get; set; }
        //public bool State { get; set; }
    }
}
