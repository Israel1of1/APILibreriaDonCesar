using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.DataAccess.Interfaces
{
    public interface ICustomerRepository
    {
        Task<RepositoryResponse<IEnumerable<Customer>>> GetAllAsync();
        Task<RepositoryResponse<Customer>> GetByIdAsync(int id);
        Task<RepositoryResponse<Customer>> GetByNameAsync(string name);
        Task<RepositoryResponse<Customer>> AddAsync(Customer customer);
        Task<RepositoryResponse<Customer>> UpdateAsync(int id, Customer customer);
        Task<RepositoryResponse<Customer>> SetStateAsync(int customerId, bool state);
    }
}
