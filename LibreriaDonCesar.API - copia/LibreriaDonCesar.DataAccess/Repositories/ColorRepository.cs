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
using Color = LibreriaDonCesar.Core.Entities.Color;

namespace LibreriaDonCesar.DataAccess.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly string _connectionString;

        public ColorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<IEnumerable<Color>>> GetAllAsync()
        {
            var colors = new List<Color>();
            var response = new RepositoryResponse<IEnumerable<Color>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAllColors", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            colors.Add(new Color
                            {
                                Id = (int)reader["Id"],
                                ColorName = reader["ColorName"].ToString()!
                            });
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = colors;
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
                return new RepositoryResponse<IEnumerable<Color>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            return response;
        }

        public async Task<RepositoryResponse<Color>> GetByIdAsync(int id)
        {
            var colorReturned = new Color();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetColorsById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            colorReturned.Id = (int)reader["Id"];
                            colorReturned.ColorName = reader["ColorName"].ToString()!;
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Color>
                    {
                        Data = colorReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Color>> GetByNameAsync(string name)
        {
            var colorReturned = new Color();
            var response = new RepositoryResponse<Color>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetColorByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            colorReturned.Id = (int)reader["Id"];
                            colorReturned.ColorName = reader["ColorName"].ToString()!;
                        }
                        else
                        {
                            colorReturned = new Color();
                        }
                    }

                    //capturar codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = colorReturned;
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
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Color>> AddAsync(Color color)
        {
            var colorReturned = new Color();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_InsertNewColor", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ColorName", color.ColorName);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            colorReturned.Id = (int)reader["Id"];
                            colorReturned.ColorName = reader["ColorName"].ToString()!;
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Color>
                    {
                        Data = colorReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Color>> UpdateAsync(int id, Color color)
        {
            var colorUpdated = new Color();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateColor", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@ColorName", color.ColorName);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            colorUpdated.Id = (int)reader["Id"];
                            colorUpdated.ColorName = reader["ColorName"].ToString()!;
                        }
                    }

                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Color>
                    {
                        Data = colorUpdated,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Color>> SetStateAsync(int id, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateColorState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@State", state);

                    Color colorUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            colorUpdated = new Color
                            {
                                Id = (int)reader["Id"],
                                ColorName = reader["ColorName"].ToString()!
                            };
                        }
                    }

                    return new RepositoryResponse<Color>
                    {
                        Data = colorUpdated,
                        OperationStatusCode = colorUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Color>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }
    }
}