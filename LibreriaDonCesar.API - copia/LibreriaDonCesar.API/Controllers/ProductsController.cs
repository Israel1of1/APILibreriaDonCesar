using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.Business.Services;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LibreriaDonCesar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var serviceResponse = await _productService.GetAllAsync();

                if (serviceResponse.IsSuccess)
                {

                    var productsDtoCollection = serviceResponse.Data.Select(p => new ProductDto
                    {
                        Id = p.Id,
                       CategoryId = p.CategoryId,
                       CategoryName = p.CategoryName,
                       PresentationId = p.PresentationId,
                       PresentationName=p.PresentationName,
                       ProductName = p.ProductName,
                       Brand = p.Brand,
                       Color = p.Color,
                       Description = p.Description,


                    });

                    //Preparamos la repuesta ApiResponse
                    var apiResponse = new ApiResponse<IEnumerable<ProductDto>>
                    {
                        Data = productsDtoCollection,
                        Meta = new { totalAmount = productsDtoCollection.Count(), message = serviceResponse.Message }
                    };

                    return Ok(apiResponse);
                }

                var unsuccessfulResponse = new UnsuccessfulResponseDto();

                switch (serviceResponse.MessageCode)
                {
                    case MessageCodes.NoData:
                        unsuccessfulResponse.Code = "200";
                        unsuccessfulResponse.Message = "No se encontraron registros";
                        unsuccessfulResponse.Details = new { info = " Temporalmente no hay registros en la BD" };

                        return Ok(unsuccessfulResponse);

                    default:
                        unsuccessfulResponse.Code = "500";
                        unsuccessfulResponse.Message = "Ocurrio un error inesperado";
                        unsuccessfulResponse.Details = new { info = "Error interno en la aplicacion" };

                        return StatusCode(500, unsuccessfulResponse);
                }
            }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //validar que el id cumpla con el formato esperado
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

            var serviceResponse = await _productService.GetByIdAsync(id);

            if (serviceResponse.IsSuccess)
            {
                //Mapeo de la categoria recibida
                var productDto = new ProductDto
                {
                    Id = serviceResponse.Data!.Id,
                    CategoryId = serviceResponse.Data!.CategoryId,
                    CategoryName = serviceResponse.Data!.CategoryName,
                    PresentationId = serviceResponse.Data!.PresentationId,
                    PresentationName = serviceResponse.Data!.PresentationName,
                    ProductName = serviceResponse.Data!.ProductName,
                    Brand = serviceResponse.Data!.Brand,
                    Color = serviceResponse.Data!.Color,
                    Description = serviceResponse.Data!.Description,

                };

                return Ok(productDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var unsuccessfulResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontro  producto asociado al Id proporcionado",
                        Details = new { info = serviceResponse.Message ?? "No se encontro el recurso solicitado" }
                    };

                    return NotFound(unsuccessfulResponse);

                default:
                    unsuccessfulResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "500",
                        Message = "Ocurrio un error",
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
            if (name.IsNullOrEmpty())
            {
                unSuccessfulResponse.Code = "400";
                unSuccessfulResponse.Message = "El dato proporcionado no es valido";
                unSuccessfulResponse.Details = new { Error = "El name no puede ser nulo o vacio" };

                return BadRequest(unSuccessfulResponse);

            }

            var ServiceResponse = await _productService.GetByNameAsync(name);

            if (ServiceResponse.IsSuccess)
            {
                var ProductDto = new ProductDto()
                {
                    Id = ServiceResponse.Data!.Id,
                    CategoryId = ServiceResponse.Data!.CategoryId,
                    CategoryName = ServiceResponse.Data!.CategoryName,
                    PresentationId = ServiceResponse.Data!.PresentationId,
                    PresentationName = ServiceResponse.Data!.PresentationName,
                    ProductName = ServiceResponse.Data!.ProductName,
                    Brand = ServiceResponse.Data!.Brand,
                    Color = ServiceResponse.Data!.Color,
                    Description = ServiceResponse.Data!.Description,
                };

                return Ok(ProductDto);
            }

            switch (ServiceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "No se encontro categoria  categoria asociada";
                    unSuccessfulResponse.Details = new { Error = "No hay registro asociado al valor name proporcionado" };

                    //no se miro bien en el video duda//
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "Ocurrio un error inesperado";

                    return StatusCode(500, unSuccessfulResponse);
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateProductDto productDto)
        {

            var serviceResponse = await _productService.CreateAsync(productDto);

            if (serviceResponse.IsSuccess)
            {

                var newProductDto = new ProductDto
                {
                    Id = serviceResponse.Data!.Id,
                    CategoryId = serviceResponse.Data!.CategoryId,
                    PresentationId = serviceResponse.Data!.PresentationId,
                    ProductName = serviceResponse.Data!.ProductName,
                    Brand = serviceResponse.Data!.Brand,
                    Color = serviceResponse.Data!.Color,
                    Description = serviceResponse.Data!.Description,

                };


                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = newProductDto.Id },
                   newProductDto
                   );
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.ErrorValidation:
                    unSuccessfulResponse.Code = "400";
                    unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Revisa los campos enviados" };
                    return BadRequest(unSuccessfulResponse);

                /*case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El registro no pudo guardarse por un conflicto";
                    unSuccessfulResponse.Details = new { info = "No se puede duplicar el nombre del producto" };
                    return Conflict(unSuccessfulResponse);*/

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrion un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno inesperado" };

                    return BadRequest(unSuccessfulResponse);



            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dataProduct)
        {


            var serviceResponse = await _productService.UpdateAsync(id, dataProduct);

            if (serviceResponse.IsSuccess)
            {
                var updatedProduct = new ProductDto
                {
                    Id = serviceResponse.Data!.Id,
                    ProductName = serviceResponse.Data!.ProductName,
                    CategoryId = serviceResponse.Data!.CategoryId,
                    PresentationId = serviceResponse.Data!.PresentationId,
                    Brand = serviceResponse.Data!.Brand,
                    Color = serviceResponse.Data!.Color,
                    Description = serviceResponse.Data!.Description

                };

                // En este punto se enviará una respuesta exitosa de la solicitud (registro actualizado)
                return Ok(updatedProduct);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "No se encontró producto con el Id proporcionado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Recurso no encontrado" };
                    return StatusCode(404, unSuccessfulResponse);

                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El registro no pudo guardarse por un conflicto";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Hubo conflicto en la actualización" };
                    return StatusCode(409, unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno inesperado" };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPatch("{id}/state")]
        public async Task<IActionResult> SetState(int id, [FromQuery] bool state)
        {
            var serviceResponse = await _productService.SetStateAsync(id, state);

            if (serviceResponse.IsSuccess)
            {
                return Ok(new
                {
                    Id = serviceResponse.Data!.Id,
                    CategoryId = serviceResponse.Data!.CategoryId,
                    PresentationId = serviceResponse.Data!.PresentationId,
                    ProductName = serviceResponse.Data!.ProductName,
                    Brand = serviceResponse.Data!.Brand,
                    Color = serviceResponse.Data!.Color,
                    Description = serviceResponse.Data!.Description,
                    Message = serviceResponse.Message
                });
            }

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
