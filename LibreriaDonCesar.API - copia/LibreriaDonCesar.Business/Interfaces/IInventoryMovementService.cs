using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreriaDonCesar.Core.Entities;



namespace LibreriaDonCesar.Business.Interfaces
{
    public interface IInventoryMovementService
    {
        Task<ServiceResponse<IEnumerable<InventoryMovement>>> GetAllAsync();
        Task<ServiceResponse<InventoryMovement>> GetByIdAsync(int  id);
        Task<ServiceResponse<InventoryMovement>> CreateAsync(CreateInventoryMovementDto newMovement);
    }
}
