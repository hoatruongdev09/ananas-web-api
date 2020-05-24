using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Npgsql;

namespace Ananas.Services.PostgreServices {
    public class ProductService : PostgreService, IProductService {
        private string tableName = "product";

        public string ConnectionString { get; set; }

        public string TableName { get { return tableName; } }

        public async Task<int> Add (ProductModel model) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"INSERT INTO {TableName}
                (name, code, description, infomation, price, branch, status, image, gender)
	            VALUES (@name, @code, @description, @infomation, @price, @branch, @status, @image, @gender) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", model.Name);
                    cmd.Parameters.AddWithValue ("@code", model.Code);
                    cmd.Parameters.AddWithValue ("@description", model.Description);
                    cmd.Parameters.AddWithValue ("@infomation", model.Infomation);
                    cmd.Parameters.AddWithValue ("@price", model.Price);
                    cmd.Parameters.AddWithValue ("@branch", model.Branch);
                    cmd.Parameters.AddWithValue ("@image", model.Image);
                    cmd.Parameters.AddWithValue ("@id", model.ID);
                    cmd.Parameters.AddWithValue ("@status", model.Status);
                    cmd.Parameters.AddWithValue ("@gender", model.Gender);
                    // cmd.Parameters.AddWithValue ("@category", model.Category);
                    // cmd.Parameters.AddWithValue ("@color", model.Color);
                    // cmd.Parameters.AddWithValue ("@collection", model.Collection);
                    // cmd.Parameters.AddWithValue ("@form", model.Form);
                    // cmd.Parameters.AddWithValue ("@material", model.Material);
                    // cmd.Parameters.AddWithValue ("@shoe_size", model.ShoeSize);
                    // cmd.Parameters.AddWithValue ("@size", model.Size);
                    try {
                        id = Convert.ToInt32 (await cmd.ExecuteScalarAsync ());
                    } catch (Exception e) {
                        throw e;
                    }

                }
                await cn.CloseAsync ();
            }
            return id;
        }

        public async Task<int> Delete (int id) {
            int rowAffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        rowAffect = await cmd.ExecuteNonQueryAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return rowAffect;
        }

        public async Task<ProductModel> Get (int id) {
            ProductModel model = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            model = new ProductModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                Name = Convert.ToString (reader["name"]),
                                Code = Convert.ToString (reader["code"]),
                                Description = Convert.ToString (reader["description"]),
                                Infomation = Convert.ToString (reader["infomation"]),
                                Price = Convert.ToInt32 (reader["price"]),
                                Branch = Convert.ToInt32 (reader["branch"]),
                                Status = Convert.ToInt32 (reader["status"]),
                                Image = Convert.ToString (reader["image"]),
                                Gender = Convert.ToInt32 (reader["gender"]),
                                // Color = Convert.ToInt32 (reader["color"]),
                                // Category = Convert.ToInt32 (reader["category"]),
                                // Collection = Convert.ToInt32 (reader["collection"]),
                                // Form = Convert.ToInt32 (reader["form"]),
                                // Material = Convert.ToInt32 (reader["material"]),
                                // ShoeSize = Convert.ToInt32 (reader["shoe_size"]),
                                // Size = Convert.ToInt32 (reader["size"]),
                            };
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return model;
        }

        public async Task<List<ProductModel>> GetList () {
            List<ProductModel> models = new List<ProductModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            models.Add (new ProductModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Name = Convert.ToString (reader["name"]),
                                    Code = Convert.ToString (reader["code"]),
                                    Description = Convert.ToString (reader["description"]),
                                    Infomation = Convert.ToString (reader["infomation"]),
                                    Price = Convert.ToInt32 (reader["price"]),
                                    Branch = Convert.ToInt32 (reader["branch"]),
                                    Status = Convert.ToInt32 (reader["status"]),
                                    Image = Convert.ToString (reader["image"]),
                                    Gender = Convert.ToInt32 (reader["gender"]),
                                    // Color = Convert.ToInt32 (reader["color"]),
                                    // Category = Convert.ToInt32 (reader["category"]),
                                    // Collection = Convert.ToInt32 (reader["collection"]),
                                    // Form = Convert.ToInt32 (reader["form"]),
                                    // Material = Convert.ToInt32 (reader["material"]),
                                    // ShoeSize = Convert.ToInt32 (reader["shoe_size"]),
                                    // Size = Convert.ToInt32 (reader["size"]),
                            });
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return models;
        }

        public async Task<int> Update (ProductModel model) {
            int rowAffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"UPDATE {TableName} SET name=@name, code=@code, description=@description, infomation=@infomation, price=@price, branch=@branch, status=@status, image=@image, gender=@gender WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", model.Name);
                    cmd.Parameters.AddWithValue ("@code", model.Code);
                    cmd.Parameters.AddWithValue ("@description", model.Description);
                    cmd.Parameters.AddWithValue ("@infomation", model.Infomation);
                    cmd.Parameters.AddWithValue ("@price", model.Price);
                    cmd.Parameters.AddWithValue ("@branch", model.Branch);
                    cmd.Parameters.AddWithValue ("@image", model.Image);
                    cmd.Parameters.AddWithValue ("@id", model.ID);
                    cmd.Parameters.AddWithValue ("@status", model.Status);
                    cmd.Parameters.AddWithValue ("@gender", model.Gender);
                    // cmd.Parameters.AddWithValue ("@category", model.Category);
                    // cmd.Parameters.AddWithValue ("@color", model.Color);
                    // cmd.Parameters.AddWithValue ("@collection", model.Collection);
                    // cmd.Parameters.AddWithValue ("@form", model.Form);
                    // cmd.Parameters.AddWithValue ("@material", model.Material);
                    // cmd.Parameters.AddWithValue ("@shoe_size", model.ShoeSize);
                    // cmd.Parameters.AddWithValue ("@size", model.Size);
                    try {
                        rowAffect = await cmd.ExecuteNonQueryAsync ();
                    } catch (Exception e) {
                        throw e;
                    }

                }
                await cn.CloseAsync ();
            }
            return rowAffect;
        }
    }
}