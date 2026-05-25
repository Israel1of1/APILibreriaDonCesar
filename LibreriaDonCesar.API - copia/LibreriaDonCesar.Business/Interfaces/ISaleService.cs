using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Entities;

namespace LibreriaDonCesar.Business.Interfaces
{
    public interface ISaleService
    {

        Task<ServiceResponse<IEnumerable<SaleTransaction>>> GetAllSaleAsync();
        Task<ServiceResponse<SaleTransaction>> GetByIdAsync(int id);
        Task<ServiceResponse<SaleResponseDto>> InsertAsync(CreateSaleDto dto);
        Task<ServiceResponse<List<SaleResponseDto>>> GetSaleByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ServiceResponse<IEnumerable<SaleResponseDetailDto>>> GetDetailByIdAsync(int saleId);


    }
}
