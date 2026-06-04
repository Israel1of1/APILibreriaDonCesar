using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using LibreriaDonCesar.DataAccess.Contexto;
using LibreriaDonCesar.DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.DataAccess.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly string _connectionString;
        private readonly ContextoBd _context;

        public BrandRepository(IConfiguration configuration, ContextoBd bd)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _context = bd;
        }

        public async Task<PaginationList<Brand>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            var totalCount = await _context.Brands.CountAsync();
            var list = await _context.Brands
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationList<Brand>(pageIndex, pageSize, totalCount, list);

        }
        public async Task<RepositoryResponse<Brand>> GetByIdAsync(int id)
        {
            var brandReturned = new Brand();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetBrandsById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            brandReturned.Id = (int)reader["Id"];
                            brandReturned.BrandName = reader["BrandName"].ToString()!;
                            brandReturned.State = (bool)reader["State"];
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Brand>
                    {
                        Data = brandReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Brand>> GetByNameAsync(string name)
        {
            var brandReturned = new Brand();
            var response = new RepositoryResponse<Brand>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetBrandByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            brandReturned.Id = (int)reader["Id"];
                            brandReturned.BrandName = reader["BrandName"].ToString()!;
                            brandReturned.State = (bool)reader["State"];
                        }
                        else
                        {
                            brandReturned = new Brand();
                        }
                    }

                    //capturar codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = brandReturned;
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
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Brand>> AddAsync(Brand brand)
        {
            var brandReturned = new Brand();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_InsertNewBrand", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BrandName", brand.BrandName);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            brandReturned.Id = (int)reader["Id"];
                            brandReturned.BrandName = reader["BrandName"].ToString()!;
                            brandReturned.State = (bool)reader["State"];
                        }
                    }

                    //capturar codigo de retorno
                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Brand>
                    {
                        Data = brandReturned,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Brand>> UpdateAsync(int id, Brand brand)
        {
            var brandUpdated = new Brand();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateBrand", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@BrandName", brand.BrandName);
                    cmd.Parameters.AddWithValue("@State", brand.State);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            brandUpdated.Id = (int)reader["Id"];
                            brandUpdated.BrandName = reader["BrandName"].ToString()!;
                            brandUpdated.State = (bool)reader["State"];
                        }
                    }

                    var retornedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Brand>
                    {
                        Data = brandUpdated,
                        OperationStatusCode = retornedValue,
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Brand>> SetStateAsync(int id, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateBrandState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@State", state);

                    Brand brandUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            brandUpdated = new Brand
                            {
                                Id = (int)reader["Id"],
                                BrandName = reader["BrandName"].ToString()!,
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Brand>
                    {
                        Data = brandUpdated,
                        OperationStatusCode = brandUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Brand>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }
    }
}