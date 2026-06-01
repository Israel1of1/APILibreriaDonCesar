using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attribute = LibreriaDonCesar.Core.Entities.Attribute;

namespace LibreriaDonCesar.DataAccess.Interfaces
{
    public interface IAttributeRepository
    {
        Task<RepositoryResponse<IEnumerable<Attribute>>> GetAllAsync();
        Task<RepositoryResponse<Attribute>> GetByIdAsync(int id);
        Task<RepositoryResponse<Attribute>> GetByNameAsync(string name);
        Task<RepositoryResponse<Attribute>> AddAsync(Attribute attribute);
        Task<RepositoryResponse<Attribute>> UpdateAsync(int id, Attribute attribute);
    }
}