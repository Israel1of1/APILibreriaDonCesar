using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attributes = LibreriaDonCesar.Core.Entities.Attributes;

namespace LibreriaDonCesar.Business.Interfaces
{
    public interface IAttributeService
    {
        Task<ServiceResponse<IEnumerable<Attributes>>> GetAllAsync();
        Task<ServiceResponse<Attributes>> GetByIdAsync(int id);
        Task<ServiceResponse<Attributes>> GetByNameAsync(string name);
        Task<ServiceResponse<Attributes>> CreateAsync(CreateAttributeDto newAttribute);
        Task<ServiceResponse<Attributes>> UpdateAsync(int id, UpdateAttributeDto attribute);
    }
}
