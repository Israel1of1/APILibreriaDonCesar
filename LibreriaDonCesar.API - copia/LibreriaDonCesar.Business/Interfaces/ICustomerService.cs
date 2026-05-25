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
    public interface ICustomerService
    {
        Task<ServiceResponse<IEnumerable<Customer>>> GetAllAsync();
        Task<ServiceResponse<Customer>> GetByIdAsync(int id);
        Task<ServiceResponse<Customer>> GetByNameAsync(string name);
        Task<ServiceResponse<Customer>> CreateAsync(CreateCustomerDto newCustomer);
        Task<ServiceResponse<Customer>> UpdateAsync(int id, UpdateCustomerDto customer);
        Task<ServiceResponse<Customer>> SetStateAsync(int customerId, bool state);
    }
}
