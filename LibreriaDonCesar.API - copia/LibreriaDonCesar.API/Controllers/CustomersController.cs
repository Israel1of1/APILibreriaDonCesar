using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.Business.Services;
using LibreriaDonCesar.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaDonCesar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        //Endpoints
        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var serviceResponse = await _customerService.GetAllAsync();

            if (serviceResponse.IsSuccess)
            {
                //Mapeo de los datos recibidos a la estructura del Dto a enviar
                //En este caso mapear la estructura Category a CategoryDto usando LINQ
                var customersDtoCollection = serviceResponse.Data.Select(cs => new CustomerDto
                {
                    Id = cs.Id,
                    CustomerName=cs.CustomerName,
                    CustomerType=cs.CustomerType,

                });

                //Preparamos la repuesta ApiResponse
                var apiResponse = new ApiResponse<IEnumerable<CustomerDto>>
                {
                    Data = customersDtoCollection,
                    Meta = new { totalAmount = customersDtoCollection.Count(), message = serviceResponse.Message }
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
            //validar que el id cumple con el formato esperado
            if (id<=0 || id==null)
            {
                var response = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }
                };
                return BadRequest(response);
            }
            var serviceResponse = await _customerService.GetByIdAsync(id);

            if (serviceResponse.IsSuccess)
            {

                var customerDto = new CustomerDto
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerName = serviceResponse.Data!.CustomerName,
                    CustomerType = serviceResponse.Data!.CustomerType,
                };

                return Ok(customerDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var unsuccessfulResponse = new UnsuccessfulResponseDto
                    {
                        Code = "404",
                        Message = "No se encontro un cliente asociada al Id proporcionado",
                        Details = new { info = serviceResponse.Message ?? "No se encontró el recurso solicitado" }
                    };

                    return NotFound(unsuccessfulResponse);

                default:
                    unsuccessfulResponse = new UnsuccessfulResponseDto
                    {
                        Code = "500",
                        Message =  "Ocurrió un error",
                        Details = new { info = serviceResponse.Message ?? "Error interno no esperado" }
                    };
                    return StatusCode(500, unsuccessfulResponse);

            }
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("byname/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            if (string.IsNullOrWhiteSpace(name))
            {
                unSuccessfulResponse.Code = "400";
                unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                unSuccessfulResponse.Details = new { Error = "El Name no puede ser nulo o vacío" };
                return BadRequest(unSuccessfulResponse);
            }

            var serviceResponse = await _customerService.GetByNameAsync(name);

            if (serviceResponse.IsSuccess)
            {
                var customerDto = new CustomerDto
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerName = serviceResponse.Data.CustomerName,
                    CustomerType = serviceResponse.Data.CustomerType
                };

                return Ok(customerDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "No se encontró el cliente asociado al valor de Name proporcionado";
                    //agregar
                    unSuccessfulResponse.Details = serviceResponse.Details = new { Error = "No hay registros asociados al valor de Name proporcionado" };
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "Ocurrió un error inesperado";
                    //quité el de detalles
                    return StatusCode(500, unSuccessfulResponse);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCustomerDto customerDto)
        {
            var serviceResponse = await _customerService.CreateAsync(customerDto);

            if (serviceResponse.IsSuccess)
            {
                var newCustomerDto = new CustomerDto
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerName = serviceResponse.Data!.CustomerName,
                    CustomerType = serviceResponse.Data!.CustomerType
                };

                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = newCustomerDto.Id },
                    newCustomerDto
                );
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El nombre del cliente ya existe";
                    unSuccessfulResponse.Details = new { info = "No se puede duplicar un nombre del cliente" };

                    return Conflict(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrio un error inesperado";
                    unSuccessfulResponse.Details = new { info = "Error interno inesperado" };

                    return StatusCode(500, unSuccessfulResponse);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto customerDto)
        {

            var serviceResponse = await _customerService.UpdateAsync(id, customerDto);
            if (serviceResponse.IsSuccess)
            {
                var updatedCustomerDto = new CustomerDto
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerName = serviceResponse.Data!.CustomerName,
                    CustomerType = serviceResponse.Data!.CustomerType
                };
                return Ok(updatedCustomerDto);
            }
            var unSuccessfulResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "No se encontro cliente con el Id proporcionado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Recurso no encontrado" };
                    return NotFound(unSuccessfulResponse);
                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El registro no pudo guardarse por un conflicto";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Hubo conflicto con el nombre del cliente, no debe duplicarse" };
                    return Conflict(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrio un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno inesperado" };
                    return StatusCode(500, unSuccessfulResponse);
            }

        }

        [Authorize(Roles = "Administrador")]
        [HttpPatch("{id}/state")]
        public async Task<IActionResult> SetState(int id, [FromQuery] bool state)
        {
            // Llama al servicio que maneja la lógica de activación/desactivación
            var serviceResponse = await _customerService.SetStateAsync(id, state);

            if (serviceResponse.IsSuccess)
            {
                // Devuelve la categoría actualizada con mensaje
                return Ok(new
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerName = serviceResponse.Data.CustomerName,
                    State = serviceResponse.Data.State,
                    Message = serviceResponse.Message
                });
            }

            // Devuelve error si no se encontró la categoría o hubo problema
            var unSuccessfulResponse = new UnsuccessfulResponseDto
            {
                Code = serviceResponse.MessageCode == MessageCodes.ErrorValidation ? "400" : "500",
                Message = serviceResponse.Message,
                Details = new { info = "Error al cambiar el estado" }
            };

            return BadRequest(unSuccessfulResponse);
        }

    }
}
