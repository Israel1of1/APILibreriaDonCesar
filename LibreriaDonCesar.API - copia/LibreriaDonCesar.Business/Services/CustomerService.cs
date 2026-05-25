using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using LibreriaDonCesar.DataAccess.Interfaces;
using LibreriaDonCesar.DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        //Constructor del Servicio 
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        //Programar la logica /reglas del negocio relacionadas a Categorias
        public async Task<ServiceResponse<IEnumerable<Customer>>> GetAllAsync()
        {
            var result = await _customerRepository.GetAllAsync();

            if (result.OperationStatusCode==0)
            {
                return new ServiceResponse<IEnumerable<Customer>>()
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode =MessageCodes.Success,
                    Message = "Operacion exitosa"
                };
            }


            switch (result.OperationStatusCode)
            {
                case 0:
                    return new ServiceResponse<IEnumerable<Customer>>
                    {
                        Data = result.Data,
                        IsSuccess = true,
                        MessageCode =MessageCodes.Success,
                        Message = "Éxito"
                    };

                case 50037:
                    return new ServiceResponse<IEnumerable<Customer>>
                    {
                        Data = result.Data,
                        IsSuccess = true,
                        MessageCode =MessageCodes.Success,
                        Message = "No hay registros disponibles"
                    };

                default:
                    return new ServiceResponse<IEnumerable<Customer>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = result.Message ?? "Ocurrió un error inesperado"
                    };


            }


        }

        public async Task<ServiceResponse<Customer>> GetByIdAsync(int id)
        {
            var repoResponse = await _customerRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode==0)
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Success,
                        Message = repoResponse.Message ?? "Operacion exitosa"
                    };
                }
                switch (repoResponse.OperationStatusCode)
                {
                    case 50050:
                        return new ServiceResponse<Customer>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontro registro al Id proporcionado"
                        };

                    default:
                        return new ServiceResponse<Customer>
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
                return new ServiceResponse<Customer>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"
                };

            }
        }

        public async Task<ServiceResponse<Customer>> GetByNameAsync(string name)
        {
            var result = await _customerRepository.GetByNameAsync(name);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<Customer>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Operacion exitosa"
                };
            }

            var messageCode = new MessageCodes();
            var message = string.Empty;


            switch (result.OperationStatusCode)
            {
                case 50051:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontro registro con el Name proporcionado";
                    break;

                default:

                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el cliente.";
                    break;


            }

            return new ServiceResponse<Customer>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }

        public async Task<ServiceResponse<Customer>> CreateAsync(CreateCustomerDto newCustomer)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByNameAsync(newCustomer.CustomerName);


                if (existingCustomer.Data!.Id != 0 && !existingCustomer.Data.CustomerName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Conflict,
                        Message = "Existe un registro con el nombre proporcionado"
                    };
                }

                //mapeo
                var category = new Customer()
                {
                    CustomerName = newCustomer.CustomerName,
                    CustomerType = newCustomer.CustomerType,
                };

                //llamado al metodo de repo
                var result = await _customerRepository.AddAsync(category);

                return new ServiceResponse<Customer>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Registro creado con exito",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<Customer>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",

                };

            }
        }

        public async Task<ServiceResponse<Customer>> UpdateAsync(int id, UpdateCustomerDto customer)
        {

            try
            {
                var existingIdCustomer = await _customerRepository.GetByIdAsync(id);

                if (existingIdCustomer.Data!.Id == 0 && existingIdCustomer.Data.CustomerName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No existe cliente con el Id proporcionado"
                    };
                }

                //validar que el nombre de la categoria no coincida con otro nombre existente
                var existingNameCustomer = await _customerRepository.GetByNameAsync(customer.CustomerName);
                if (existingNameCustomer.Data!.CustomerName != null && existingNameCustomer.Data.Id != id)
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Conflict,
                        Message = "Ya existe un cliente con el nombre proporcionado. No se debe duplicar el nombre"
                    };
                }

                //mapeo
                var dataCustomer = new Customer()
                {
                    CustomerName = customer.CustomerName,
                    CustomerType = customer.CustomerType,
                    State = customer.State,
                };

                //llamado al metodo de repo
                var result = await _customerRepository.UpdateAsync(id, dataCustomer);

                return new ServiceResponse<Customer>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Registro actualizado con exito"
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<Customer>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }

        }

        public async Task<ServiceResponse<Customer>> SetStateAsync(int customerId, bool state)
        {
            var response = new ServiceResponse<Customer>();

            var existingCustomer = await _customerRepository.GetByIdAsync(customerId);
            if (existingCustomer == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "El cliente no existe";
                return response;
            }
            if (existingCustomer.Data.State == state)
            {
                response.Data = existingCustomer.Data;
                response.IsSuccess = true;
                response.MessageCode = MessageCodes.Success;
                response.Message = state ? "El cliente ya está activado" : "El cliente ya está desactivado";
                return response;
            }

            var repoResponse = await _customerRepository.SetStateAsync(customerId, state);

            if (repoResponse.Data == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "No se pudo actualizar el estado de el cliente";
                return response;
            }

            // Construir la respuesta exitosa
            response.Data = repoResponse.Data;
            response.IsSuccess = true;
            response.MessageCode = MessageCodes.Success;
            response.Message = state ? "Cliente activado" : "Cliente desactivado";

            return response;
        }


    }
}
