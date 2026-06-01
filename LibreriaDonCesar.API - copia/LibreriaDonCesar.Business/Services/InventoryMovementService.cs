using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using LibreriaDonCesar.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibreriaDonCesar.DataAccess.Interfaces;
using System.Threading.Tasks;
namespace LibreriaDonCesar.Business.Services
{
    public class InventoryMovementService : IInventoryMovementService
    {
        private readonly IInventoryMovementRepository _inventoryMovementRepository;

        public InventoryMovementService(IInventoryMovementRepository inventoryMovementRepository)
        {
            _inventoryMovementRepository = inventoryMovementRepository;
        }

        public async Task<ServiceResponse<IEnumerable<InventoryMovement>>> GetAllAsync()
        {
            var result = await _inventoryMovementRepository.GetAllAsync();

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<IEnumerable<InventoryMovement>>()
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Operacion exitosa"
                };
            }
            switch (result.OperationStatusCode)
            {
                 case 50118:
                     return new ServiceResponse<IEnumerable<InventoryMovement>>
                     {
                         Data = result.Data,
                         IsSuccess = true,
                         MessageCode = MessageCodes.Success,
                         Message = "No hay registros disponibles"
                     };

                 default:
                     return new ServiceResponse<IEnumerable<InventoryMovement>>
                     {
                         Data = null,
                         IsSuccess = false,
                         MessageCode = MessageCodes.ErrorDataBase,
                         Message = result.Message ?? "Ocurrió un error inesperado"
                     };
            }
        }
        public async Task<ServiceResponse<InventoryMovement>> GetByIdAsync(int id)
        {
            var repoResponse = await _inventoryMovementRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<InventoryMovement>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Success,
                        Message = repoResponse.Message ?? "Operacion exitosa"
                    };
                }
                switch (repoResponse.OperationStatusCode)
                {
                    case 50028:
                        return new ServiceResponse<InventoryMovement>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No existe el movimiento"
                        };

                    default:
                        return new ServiceResponse<InventoryMovement>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = repoResponse.Message ?? "Ocurrio un error inesperado"
                        };
                }
            }
            catch (Exception)
            {
                return new ServiceResponse<InventoryMovement>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"
                };
            }
        }
        public async Task<ServiceResponse<InventoryMovement>> CreateAsync(CreateInventoryMovementDto newMovement)
        {
            try
            {
                // mapeo
                var movement = new InventoryMovement()
                {
                    ProductId = newMovement.ProductId,
                    MovementType = newMovement.MovementType,
                    Quantity = newMovement.Quantity,
                    StockBefore = newMovement.StockBefore,
                    StockAfter = newMovement.StockAfter,
                    Reason = newMovement.Reason
                };

                // llamado al metodo de repo
                var result = await _inventoryMovementRepository.AddAsync(movement);

                return new ServiceResponse<InventoryMovement>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Registro creado con exito"
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<InventoryMovement>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado"
                };
            }
        }





    }
}

