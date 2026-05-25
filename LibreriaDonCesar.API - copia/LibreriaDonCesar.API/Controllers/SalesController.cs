using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaDonCesar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAllSale()
        {
            var serviceResponse = await _saleService.GetAllSaleAsync();

            if (serviceResponse.IsSuccess)
            {
                var purchasesDtoCollection = serviceResponse.Data.Select(pr => new SaleResponseDto
                {
                    Id = pr.Master.Id,
                    CustomerId = pr.Master.CustomerId,
                    UserId = pr.Master.UserId,
                    SaleDate = pr.Master.SaleDate,
                    TotalAmount = pr.Master.TotalAmount,

                    Details = pr.Details.Select(d => new SaleResponseDetailDto
                    {
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        SalePrice = d.SalePrice,
                        LineTotal = d.LineTotal
                    }).ToList()
                }).ToList();

                var apiResponse = new ApiResponse<IEnumerable<SaleResponseDto>>
                {
                    Data = purchasesDtoCollection,
                    Meta = new
                    {
                        totalAmount = purchasesDtoCollection.Count,
                        message = serviceResponse.Message
                    }
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
                    unsuccessfulResponse.Details = new { info = "Error interno en la aplicación" };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var serviceResponse = await _saleService.GetByIdAsync(id);
            if (serviceResponse.IsSuccess)
            {
                var sl = serviceResponse.Data!;
                var saleDto = new SaleResponseDto
                {
                    Id = sl.Master.Id,
                    CustomerId = sl.Master.CustomerId,
                    UserId = sl.Master.UserId,
                    SaleDate = sl.Master.SaleDate,
                    TotalAmount = sl.Master.TotalAmount,
                    Details = sl.Details.Select(d => new SaleResponseDetailDto
                    {
                        ProductId = d.ProductId,
                        ProductName=d.ProductName,
                        Quantity = d.Quantity,
                        SalePrice = d.SalePrice,
                        LineTotal = d.LineTotal
                    }).ToList()
                };
                return Ok(new ApiResponse<SaleResponseDto>
                {
                    Data = saleDto,
                    Meta = new { message = serviceResponse.Message }
                });
            }
            var unsuccessfulResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unsuccessfulResponse.Code = "404";
                    unsuccessfulResponse.Message = serviceResponse.Message ?? "Recurso no encontrado.";
                    unsuccessfulResponse.Details = serviceResponse.Details = new { info = serviceResponse.Message };
                    return NotFound(unsuccessfulResponse);
                default:
                    unsuccessfulResponse.Code = "500";
                    unsuccessfulResponse.Message = "Ocurrió un error inesperado.";
                    return StatusCode(500, unsuccessfulResponse);
            }
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpPost]
        public async Task<IActionResult> Register(CreateSaleDto dto)
        {
            var serviceResponse = await _saleService.InsertAsync(dto);

            if (serviceResponse.IsSuccess)
            {
                var dataResponse = serviceResponse.Data!;

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = dataResponse.Id },
                    dataResponse
                    );

            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case Core.Common.MessageCodes.ErrorValidation:
                    unSuccessfulResponse.Code = "400";
                    unSuccessfulResponse.Message = "Ocurrio un error en la validacion de datos";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };
                    return BadRequest(unSuccessfulResponse);
                case Core.Common.MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "Recurso no encontrado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };
                    return NotFound(unSuccessfulResponse);

                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El registro no pudo guardarse por un conflicto";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message  };
                    return Conflict(unSuccessfulResponse);



                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrio un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };

                    return StatusCode(500, unSuccessfulResponse);



            }
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("details/{saleId}")]
        public async Task<IActionResult> GetDetailById(int saleId)
        {
            if (saleId <= 0)
            {
                var response = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }
                };
                return BadRequest(response);
            }

            var serviceResponse = await _saleService.GetDetailByIdAsync(saleId);

            if (serviceResponse.IsSuccess)
            {
                var saleDetailDto = new List<SaleResponseDetailDto>();
                foreach (var item in serviceResponse.Data)
                {
                    var detailDto = new SaleResponseDetailDto
                    {
                        //Id = item.Id,
                       //SaleId = saleId,
                        ProductId = item.ProductId,
                        ProductName= item.ProductName,
                        Quantity = item.Quantity,
                        SalePrice = item.SalePrice,
                        LineTotal = item.LineTotal
                    };
                    saleDetailDto.Add(detailDto);
                }
                return Ok(saleDetailDto);
            }
            else
            {
                UnsuccessfulResponseDto unsuccessfulResponse;

                switch (serviceResponse.MessageCode)
                {
                    case MessageCodes.NotFound:
                        unsuccessfulResponse = new UnsuccessfulResponseDto
                        {
                            Code = "404",
                            Message = "No se encontro un detalle asociada al Id proporcionado",
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
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("bydaterange")]
        public async Task<IActionResult> GetSaleByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default || endDate < startDate)
            {
                return BadRequest(new
                {
                    Code = "400",
                    Message = "Los parámetros proporcionados no son válidos",
                    Details = new { Error = "startDate y endDate deben ser válidos y endDate >= startDate" }
                });
            }

            var serviceResponse = await _saleService.GetSaleByDateRangeAsync(startDate, endDate);

            if (serviceResponse.IsSuccess)
                return Ok(serviceResponse.Data);

            var unsuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NoData:
                    unsuccessfulResponse.Code = "200";
                    unsuccessfulResponse.Message = "No se encontraron registros";
                    unsuccessfulResponse.Details = new { info = "Temporalmente no hay registros en la BD" };
                    return Ok(unsuccessfulResponse);
                case MessageCodes.NotFound:
                    unsuccessfulResponse.Code = "404";
                    unsuccessfulResponse.Message = serviceResponse.Message ?? "Recurso no encontrado.";
                    unsuccessfulResponse.Details = serviceResponse.Details = new { info = serviceResponse.Message };
                    return NotFound(unsuccessfulResponse);
                case MessageCodes.Conflict:
                    unsuccessfulResponse.Code = "409";
                    unsuccessfulResponse.Message = "Stock insuficiente";
                    unsuccessfulResponse.Details = new { info = "Las productos solicitados superan la cantidad disponible en inventario" };
                    return Conflict(unsuccessfulResponse);


                default:
                    unsuccessfulResponse.Code = "500";
                    unsuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unsuccessfulResponse.Details = new { info = "Error interno en la aplicación" };
                    return StatusCode(500, unsuccessfulResponse);

                
            }
        }




    }
}

    

