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
    public class PresentationRepository : IPresentationRepository
    {
        private readonly string _connectionString;

        public PresentationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<IEnumerable<Presentation>>> GetAllAsync()
        {
            var presentations = new List<Presentation>();

            var response = new RepositoryResponse<IEnumerable<Presentation>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAllPresentations", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            presentations.Add(new Presentation
                            {
                                Id = (int)reader["Id"],
                                PresentationName = reader["PresentationName"].ToString()!,
                                Amount = (decimal)reader["Amount"],
                                UnitMeasureId = (int)reader["UnitMeasureId"],
                                UnitMeasureName = reader["UnitMeasureName"].ToString()!,
                                UnitFactor = reader["UnitFactor"].ToString()!,
                                State = (bool)reader["State"]

                            });
                        }
                    }
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = presentations;
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
                return new RepositoryResponse<IEnumerable<Presentation>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            return response;
        }

        public async Task<RepositoryResponse<Presentation>> GetByIdAsync(int id)
        {
            var presentationReturned = new Presentation();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetPresentationById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            presentationReturned.Id = (int)reader["Id"];
                            presentationReturned.PresentationName = (string)reader["PresentationName"];
                            presentationReturned.Amount = (decimal)reader["Amount"];
                            presentationReturned.UnitMeasureId = (int)reader["UnitMeasureId"];
                            presentationReturned.UnitMeasureName = (string)reader["UnitMeasureName"];
                            presentationReturned.UnitFactor = (string)reader["UnitFactor"];
                            presentationReturned.State = (bool)reader["State"];

                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Presentation>
                    {
                        Data = presentationReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Presentation>> GetByNameAsync(string name)
        {
            var presentation = new Presentation();
            var response = new RepositoryResponse<Presentation>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetPresentationByName", connection);
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PresentationName", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            presentation.Id = (int)reader["Id"];
                            presentation.PresentationName = reader["PresentationName"].ToString()!;
                            presentation.Amount = (decimal)reader["Amount"]; 
                            presentation.UnitMeasureId = (int)reader["UnitMeasureId"];
                            presentation.UnitMeasureName = (string)reader["UnitMeasureName"];
                            presentation.UnitFactor = reader["UnitFactor"].ToString()!;
                            presentation.State = (bool)reader["State"];


                        }

                        else
                        {
                            presentation = new Presentation(); 
                        }
                    }

                    //capturar codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = presentation;
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
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }


        
        }

        public async Task<RepositoryResponse<Presentation>> AddAsync(Presentation presentation)
        {
            var presentationReturned = new Presentation();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_InsertNewPresentation", connection);
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PresentationName", presentation.PresentationName);
                    cmd.Parameters.AddWithValue("@Amount", presentation.Amount);
                    cmd.Parameters.AddWithValue("@UnitMeasureId", presentation.UnitMeasureId);
                    cmd.Parameters.AddWithValue("@UnitFactor", presentation.UnitFactor);
                    cmd.Parameters.AddWithValue("@State", presentation.State);


                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            presentationReturned.Id = (int)reader["Id"];
                            presentationReturned.PresentationName = (string)reader["PresentationName"];
                            presentationReturned.Amount = (decimal)reader["Amount"];
                            presentationReturned.UnitMeasureId = (int)reader["UnitMeasureId"];
                            presentationReturned.UnitFactor = (string)reader["UnitFactor"];
                            presentationReturned.State = (bool)reader["State"];

                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Presentation>
                    {
                        Data = presentationReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Presentation>> UpdateAsync(int id, Presentation presentation)
        {
            var presentationUpdated = new Presentation();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdatePresentation", connection);
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@PresentationName", presentation.PresentationName);
                    cmd.Parameters.AddWithValue("@Amount", presentation.Amount);
                    cmd.Parameters.AddWithValue("@UnitMeasureId", presentation.UnitMeasureId);
                    cmd.Parameters.AddWithValue("@UnitFactor", presentation.UnitFactor);
                    cmd.Parameters.AddWithValue("@State", presentation.State);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            presentationUpdated.Id = (int)reader["Id"];
                            presentationUpdated.PresentationName = (string)reader["PresentationName"].ToString()!;
                            presentationUpdated.Amount = (decimal)reader["Amount"];
                            presentationUpdated.UnitMeasureId = (int)reader["UnitMeasureId"];
                            presentationUpdated.UnitFactor = (string)reader["UnitFactor"].ToString()!;
                            presentationUpdated.State = (bool)reader["State"];

                        }
                    }

                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Presentation>
                    {
                        Data = presentationUpdated,
                        OperationStatusCode = retornedValue,
                    };

                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Presentation>> SetStateAsync(int presentationId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdatePresentationState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", presentationId);
                    cmd.Parameters.AddWithValue("@State", state);

                    Presentation presentationUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            presentationUpdated = new Presentation
                            {
                                Id = (int)reader["Id"],
                                PresentationName = reader["PresentationName"].ToString()!,
                                Amount = (decimal)reader["Amount"],
                                UnitMeasureId = (int)reader["UnitMeasureId"],
                                UnitFactor = reader["UnitFactor"].ToString()!,
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Presentation>
                    {
                        Data = presentationUpdated,
                        OperationStatusCode = presentationUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Presentation>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

    }

}

