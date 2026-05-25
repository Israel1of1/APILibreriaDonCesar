using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using LibreriaDonCesar.DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.DataAccess.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<IEnumerable<Customer>>> GetAllAsync()
        {
            var customers = new List<Customer>();

            var response = new RepositoryResponse<IEnumerable<Customer>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAllCustomers", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new Customer
                            {
                                Id = (int)reader["Id"],
                                CustomerName = reader["CustomerName"].ToString()!,
                                CustomerType = reader["CustomerType"].ToString()!,
                                State = reader["State"] != DBNull.Value ? (bool)reader["State"] : false


                            });
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = customers;
                    response.OperationStatusCode = returnedValue;
                    response.Message = "Operacion exitosa";


                }
            }
            catch (SqlException ex)
            {
                response.Data = null;
                response.OperationStatusCode = ex.Number;

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<IEnumerable<Customer>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            return response;
        }

        public async Task<RepositoryResponse<Customer>> GetByIdAsync(int id)
        {
            var customerReturned = new Customer();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetCustomerById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerReturned.Id = (int)reader["Id"];
                            customerReturned.CustomerName = (string)reader["CustomerName"];
                            customerReturned.CustomerType = (string)reader["CustomerType"];
                            customerReturned.State = (bool)reader["State"];
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }

        }

        public async Task<RepositoryResponse<Customer>> GetByNameAsync(string name)
        {
            var customer = new Customer();
            var response = new RepositoryResponse<Customer>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetCustomerByName", connection);
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customer.Id = (int)reader["Id"];
                            customer.CustomerName = reader["CustomerName"].ToString()!;
                            customer.CustomerType = reader["CustomerType"].ToString()!;
                            customer.State = (bool)reader["State"];
                        }

                        else
                        {
                            customer = new Customer(); // Retornar un objeto Category vacío si no se encuentra ninguna coincidencia
                        }
                    }

                    //capturar codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = customer;
                    response.OperationStatusCode = returnedValue;

                    return response;
                }
            }
            catch (SqlException ex)
            {
                response.Data = null;
                response.OperationStatusCode = ex.Number;

                return response;

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }


        }

        public async Task<RepositoryResponse<Customer>> AddAsync(Customer customer)
        {
            var customerReturned = new Customer();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_InsertNewCustomer", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@CustomerType", customer.CustomerType);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerReturned.Id = (int)reader["Id"];
                            customerReturned.CustomerName = reader["CustomerName"].ToString()!;
                            customerReturned.CustomerType = reader["CustomerType"].ToString()!;
                            customerReturned.State = (bool)reader["State"];
                        }
                    }

                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Customer>> UpdateAsync(int id, Customer customer)
        {
            var customerUpdated = new Customer();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateCustomer", connection);
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@CustomerType", customer.CustomerType);
                    cmd.Parameters.AddWithValue("@State", customer.State);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerUpdated.Id = (int)reader["Id"];
                            customerUpdated.CustomerName = (string)reader["CustomerName"].ToString()!;
                            customerUpdated.CustomerType = (string)reader["CustomerType"].ToString()!;
                            customerUpdated.State = (bool)reader["State"];
                        }
                    }

                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerUpdated,
                        OperationStatusCode = retornedValue,
                    };

                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Customer>> SetStateAsync(int customerId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateCustomerState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    cmd.Parameters.AddWithValue("@State", state);

                    Customer customerUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerUpdated = new Customer
                            {
                                Id = (int)reader["Id"],
                                CustomerName = reader["CustomerName"].ToString(),
                                CustomerType = reader["CustomerType"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerUpdated,
                        OperationStatusCode = customerUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

    }
}
