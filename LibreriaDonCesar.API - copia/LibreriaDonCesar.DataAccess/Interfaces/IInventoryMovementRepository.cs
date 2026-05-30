using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.DataAccess.Interfaces
{
    public interface IInventoryMovementRepository
    {
        Task<RepositoryResponse<IEnumerable<InventoryMovement>>> GetAllAsync();
        Task<RepositoryResponse<InventoryMovement>> GetByIdAsync(int id);
        Task<RepositoryResponse<InventoryMovement>> AddAsync(InventoryMovement movement);

    }
}
