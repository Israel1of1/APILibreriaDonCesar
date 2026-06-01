using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibreriaDonCesar.Core.Entities;



namespace LibreriaDonCesar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryMovementsController : ControllerBase
    {
        private readonly IInventoryMovementService _inventoryMovementService;

        public InventoryMovementsController(IInventoryMovementService inventoryMovementService)
        {
            _inventoryMovementService = inventoryMovementService;
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var serviceResponse = await _inventoryMovementService.GetAllAsync();

            if (serviceResponse.IsSuccess)
            {
                var movementsDtoCollection = serviceResponse.Data.Select(m => new InventoryMovementDto
                {
                    Id = m.Id,
                    DateTime = m.DateTime,
                    ProductId = m.ProductId,
                    MovementType = m.MovementType,
                    Quiantity = m.Quantity,
                    StockBefore = m.StockBefore,
                    StockAfter = m.StockAfter,
                    Reason = m.Reason
                });

                var apiResponse = new ApiResponse<IEnumerable<InventoryMovementDto>>
                {
                    Data = movementsDtoCollection,
                    Meta = new { totalAmount = movementsDtoCollection.Count(), message = serviceResponse.Message }
                };

                return Ok(apiResponse);
            }

            var unsuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NoData:
                    unsuccessfulResponse.Code = "200";
                    unsuccessfulResponse.Message = "No se encontraron registros";
                    unsuccessfulResponse.Details = new { info = "Temporalmente no hay registros en la BD" };
                    return Ok(unsuccessfulResponse);

                default:
                    unsuccessfulResponse.Code = "500";
                    unsuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unsuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno en la aplicación" };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0 || id == null)
            {
                var response = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }
                };
                return BadRequest(response);
            }

            var serviceResponse = await _inventoryMovementService.GetByIdAsync(id);

            if (serviceResponse.IsSuccess)
            {
                var movementDto = new InventoryMovementDto
                {
                    Id = serviceResponse.Data!.Id,
                    DateTime = serviceResponse.Data!.DateTime,
                    ProductId = serviceResponse.Data!.ProductId,
                    MovementType = serviceResponse.Data!.MovementType,
                    Quiantity = serviceResponse.Data!.Quantity,
                    StockBefore = serviceResponse.Data!.StockBefore,
                    StockAfter = serviceResponse.Data!.StockAfter,
                    Reason = serviceResponse.Data!.Reason
                };

                return Ok(movementDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var unsuccessfulResponse = new UnsuccessfulResponseDto
                    {
                        Code = "404",
                        Message = "No se encontro un movimiento asociado al Id proporcionado",
                        Details = new { info = serviceResponse.Message ?? "No se encontró el recurso solicitado" }
                    };
                    return NotFound(unsuccessfulResponse);

                default:
                    unsuccessfulResponse = new UnsuccessfulResponseDto
                    {
                        Code = "500",
                        Message = "Ocurrió un error",
                        Details = new { info = serviceResponse.Message ?? "Error interno no esperado" }
                    };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateInventoryMovementDto movementDto)
        {
            var serviceResponse = await _inventoryMovementService.CreateAsync(movementDto);

            if (serviceResponse.IsSuccess)
            {
                var newMovementDto = new InventoryMovementDto
                {
                    Id = serviceResponse.Data!.Id,
                    DateTime = serviceResponse.Data!.DateTime,
                    ProductId = serviceResponse.Data!.ProductId,
                    MovementType = serviceResponse.Data!.MovementType,
                    Quiantity = serviceResponse.Data!.Quantity,
                    StockBefore = serviceResponse.Data!.StockBefore,
                    StockAfter = serviceResponse.Data!.StockAfter,
                    Reason = serviceResponse.Data!.Reason
                };

                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = newMovementDto.Id },
                    newMovementDto
                );
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrio un error inesperado";
                    unSuccessfulResponse.Details = new { info = "Error interno inesperado" };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }
    }
}