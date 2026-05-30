using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.Interfaces
{
    public interface IBrandService
    {
        Task<ServiceResponse<IEnumerable<Brand>>> GetAllAsync();
        Task<ServiceResponse<Brand>> GetByIdAsync(int id);
        Task<ServiceResponse<Brand>> GetByNameAsync(string name);
        Task<ServiceResponse<Brand>> CreateAsync(CreateBrandDto newBrand);
        Task<ServiceResponse<Brand>> UpdateAsync(int id, UpdateBrandDto brand);
        Task<ServiceResponse<Brand>> SetStateAsync(int brandId, bool state);
    }
}
