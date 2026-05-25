using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Core.Entities
{
    public class Sale
    {
        public  int Id { get; set; }       
        public int CustomerId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }       
        public int UserId { get; set; }


    }
}
