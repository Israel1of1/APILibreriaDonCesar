using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attributes = LibreriaDonCesar.Core.Entities.Attributes;

namespace LibreriaDonCesar.DataAccess.Interfaces
{
    public interface IAttributeRepository
    {
        Task<RepositoryResponse<IEnumerable<Attributes>>> GetAllAsync();
        Task<RepositoryResponse<Attributes>> GetByIdAsync(int id);
        Task<RepositoryResponse<Attributes>> GetByNameAsync(string name);
        Task<RepositoryResponse<Attributes>> AddAsync(Attributes attribute);
        Task<RepositoryResponse<Attributes>> UpdateAsync(int id, Attributes attribute);
    }
}