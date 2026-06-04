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
using Attributes = LibreriaDonCesar.Core.Entities.Attributes;

namespace LibreriaDonCesar.DataAccess.Repositories
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly string _connectionString;

        public AttributeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<IEnumerable<LibreriaDonCesar.Core.Entities.Attributes>>> GetAllAsync()
        {
            var attributes = new List<LibreriaDonCesar.Core.Entities.Attributes>();

            var response = new RepositoryResponse<IEnumerable<LibreriaDonCesar.Core.Entities.Attributes>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAllAttributes", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            attributes.Add(new LibreriaDonCesar.Core.Entities.Attributes
                            {
                                Id = (int)reader["Id"],
                                AttributeName = reader["AttributeName"].ToString()!,
                            });
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = attributes;
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
                return new RepositoryResponse<IEnumerable<LibreriaDonCesar.Core.Entities.Attributes>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            return response;
        }


        public async Task<RepositoryResponse<Attributes>> GetByIdAsync(int id)
        {
            var attributeReturned = new Attribute();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetAttributesById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            attributeReturned.Id = (int)reader["Id"];
                            attributeReturned.AttributeName = (string)reader["AttributeName"];
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Attributes>
                    {
                        Data = attributeReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Attributes>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Attributes>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }

        }

        public async Task<RepositoryResponse<Attributes>> GetByNameAsync(string name)
        {
            var attributes = new Attribute();
            var response = new RepositoryResponse<Attributes>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAttributeByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            attributes.Id = (int)reader["Id"];
                            attributes.AttributeName = reader["AttributeName"].ToString()!;
                        }

                        else
                        {
                            attributes = new Attribute();
                        }
                    }

                    //capturar codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = attributes;
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
                return new RepositoryResponse<Attributes>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }

        }


        public async Task<RepositoryResponse<Attributes>> AddAsync(Attributes attributes)
        {
            var attributeReturned = new Attribute();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_InsertNewAttribute", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AttributeName", attributes.AttributeName);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            attributeReturned.Id = (int)reader["Id"];
                            attributeReturned.AttributeName = (string)reader["AttributeName"];
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Attributes>
                    {
                        Data = attributeReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Attributes>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Attributes>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Attributes>> UpdateAsync(int id, Attributes attributes)
        {
            var attributeUpdated = new Attribute();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateAttribute", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@AttributeName", attributes.AttributeName);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            attributeUpdated.Id = (int)reader["Id"];
                            attributeUpdated.AttributeName = reader["AttributeName"].ToString()!;
                        }
                    }

                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Attributes>
                    {
                        Data = attributeUpdated,
                        OperationStatusCode = retornedValue,
                    };

                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Attributes>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Attributes>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

    }
}