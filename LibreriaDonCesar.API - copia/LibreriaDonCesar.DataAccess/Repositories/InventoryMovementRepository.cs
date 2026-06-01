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
    public class InventoryMovementRepository : IInventoryMovementRepository
    {
        private readonly string _connectionString;

        public InventoryMovementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<IEnumerable<InventoryMovement>>> GetAllAsync()
        {
            var movements = new List<InventoryMovement>();

            var response = new RepositoryResponse<IEnumerable<InventoryMovement>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAllMovements", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            movements.Add(new InventoryMovement
                            {
                                Id = (int)reader["Id"],
                                DateTime = (DateTime)reader["DateTime"],
                                ProductId = (int)reader["ProductId"],
                                MovementType = reader["MovementType"].ToString()!,
                                Quantity = (int)reader["Quantity"],
                                StockBefore = (int)reader["StockBefore"],
                                StockAfter = (int)reader["StockAfter"],
                                Reason = reader["Reason"] == DBNull.Value ? null : reader["Reason"].ToString()
                            });
                        }
                    }
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = movements;
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
                return new RepositoryResponse<IEnumerable<InventoryMovement>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            return response;
        }

        public async Task<RepositoryResponse<InventoryMovement>> GetByIdAsync(int id)
        {
            var movementReturned = new InventoryMovement();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetMovementsById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            movementReturned.Id = (int)reader["Id"];
                            movementReturned.DateTime = (DateTime)reader["DateTime"];
                            movementReturned.ProductId = (int)reader["ProductId"];
                            movementReturned.MovementType = reader["MovementType"].ToString()!;
                            movementReturned.Quantity = (int)reader["Quantity"];
                            movementReturned.StockBefore = (int)reader["StockBefore"];
                            movementReturned.StockAfter = (int)reader["StockAfter"];
                            movementReturned.Reason = reader["Reason"] == DBNull.Value ? null : reader["Reason"].ToString();
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<InventoryMovement>
                    {
                        Data = movementReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<InventoryMovement>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<InventoryMovement>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<InventoryMovement>> AddAsync(InventoryMovement movements)
        {
            var movementReturned = new InventoryMovement();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_InsertNewMovement", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductId", movements.ProductId);
                    cmd.Parameters.AddWithValue("@MovementType", movements.MovementType);
                    cmd.Parameters.AddWithValue("@Quantity", movements.Quantity);
                    cmd.Parameters.AddWithValue("@StockBefore", movements.StockBefore);
                    cmd.Parameters.AddWithValue("@StockAfter", movements.StockAfter);
                    cmd.Parameters.AddWithValue("@Reason", (object?)movements.Reason ?? DBNull.Value);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            movementReturned.Id = (int)reader["Id"];
                            movementReturned.DateTime = (DateTime)reader["DateTime"];
                            movementReturned.ProductId = (int)reader["ProductId"];
                            movementReturned.MovementType = reader["MovementType"].ToString()!;
                            movementReturned.Quantity = (int)reader["Quantity"];
                            movementReturned.StockBefore = (int)reader["StockBefore"];
                            movementReturned.StockAfter = (int)reader["StockAfter"];
                            movementReturned.Reason = reader["Reason"] == DBNull.Value ? null : reader["Reason"].ToString();
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<InventoryMovement>
                    {
                        Data = movementReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<InventoryMovement>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<InventoryMovement>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

    }

}

