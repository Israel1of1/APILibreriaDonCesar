using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using LibreriaDonCesar.DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LibreriaDonCesar.DataAccess.Repositories
{
   public class RoleRepository : IRoleRepository
    {

        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<IEnumerable<Role>>> GetAllAsync()
        {
            var role = new List<Role>();
            //Instancia de objeto RepositoryResponse que se retornará
            var response = new RepositoryResponse<IEnumerable<Role>>();

            try
            {

                //Conexión a la BD
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAllRole", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            role.Add(new Role
                            {
                                Id = (int)reader["Id"],
                                RoleName = reader["RoleName"].ToString()!,
                                State = reader["State"] != DBNull.Value ? (bool)reader["State"] : false

                            });
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = role;
                    response.OperationStatusCode = returnedValue;
                    response.Message = "Operacion exitosa";

                }
            }

            catch (SqlException ex)
            {
                response.Data = null;
                response.OperationStatusCode = ex.Number;
                response.Message = ex.Message;
            }

            catch (Exception ex)
            {
                return new RepositoryResponse<IEnumerable<Role>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            return response;
        }

        public async Task<RepositoryResponse<Role>> GetByIdAsync(int id)
        {
            var roleReturned = new Role();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetRoleById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            roleReturned.Id = (int)reader["Id"];
                            roleReturned.RoleName = (string)reader["RoleName"];
                            roleReturned.State = (bool)reader["State"];
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Role>
                    {
                        Data = roleReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }

        }

        public async Task<RepositoryResponse<Role>> GetByNameAsync(string name)
        {
            var role = new Role();
            var response = new RepositoryResponse<Role>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetRoleByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            role.Id = (int)reader["Id"];
                            role.RoleName = reader["RoleName"].ToString()!;
                            role.State = (bool)reader["State"];
                        }

                        else
                        {
                            role = new Role(); // Retornar un objeto rol vacío si no se encuentra ninguna coincidencia
                        }
                    }

                    //capturar codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = role;
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
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }


        }

        public async Task<RepositoryResponse<Role>> AddAsync(Role role)
        {
            var roleReturned = new Role();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_InsertNewRole", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            roleReturned.Id = (int)reader["Id"];
                            roleReturned.RoleName = (string)reader["RoleName"];
                            roleReturned.State = (bool)reader["State"];
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Role>
                    {
                        Data = roleReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Role>> UpdateAsync(int id, Role role)
        {
            var roleUpdated = new Role();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateUser", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                    cmd.Parameters.AddWithValue("@State", role.State);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            roleUpdated.Id = (int)reader["Id"];
                            roleUpdated.RoleName = (string)reader["RoleName"].ToString()!;
                            roleUpdated.State = (bool)reader["State"];
                        }
                    }

                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Role>
                    {
                        Data = roleUpdated,
                        OperationStatusCode = retornedValue,
                    };

                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Role>> SetStateAsync(int roleId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateRoleState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", roleId);
                    cmd.Parameters.AddWithValue("@State", state);

                    Role roleUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                          roleUpdated = new Role
                            {
                                Id = (int)reader["Id"],
                                RoleName = reader["RoleName"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Role>
                    {
                        Data = roleUpdated,
                        OperationStatusCode = roleUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }




    }
}




    

