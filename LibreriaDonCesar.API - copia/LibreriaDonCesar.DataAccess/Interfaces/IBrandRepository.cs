using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.DataAccess.Interfaces
{
    public interface IBrandRepository
    {
        Task<PaginationList<Brand>> GetAllAsync(int pageIndex =1 , int pageSize = 10);
        Task<RepositoryResponse<Brand>> GetByIdAsync(int id);
        Task<RepositoryResponse<Brand>> GetByNameAsync(string name);
        Task<RepositoryResponse<Brand>> AddAsync(Brand brand);
        Task<RepositoryResponse<Brand>> UpdateAsync(int id, Brand brand);
        Task<RepositoryResponse<Brand>> SetStateAsync(int id, bool state);
    }
}