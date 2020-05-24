using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Npgsql;
namespace Ananas.Services.PostgreServices {
    public class ShoeSizeService : PostgreService, IShoeSizeService {
        private string mainTableName = "shoe_size";
        private string productShoeSizeTable = "product_shoe_size";

        public string ConnectionString { get; set; }

        public string TableName { get { return mainTableName; } }
        public ShoeSizeService () : base () {

        }
        public ShoeSizeService (string connectionString) {
            this.ConnectionString = connectionString;
        }
        public async Task<int> Add (ShoeSizeModel model) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {TableName}(size) VALUES(@size) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@size", model.Size);
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

        public async Task<ShoeSizeModel> Get (int id) {
            ShoeSizeModel shoeSize = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            shoeSize = new ShoeSizeModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                Size = Convert.ToInt32 (reader["size"])
                            };
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return shoeSize;
        }

        public async Task<List<ShoeSizeModel>> GetList () {
            List<ShoeSizeModel> listSizes = new List<ShoeSizeModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            listSizes.Add (new ShoeSizeModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Size = Convert.ToInt32 (reader["size"])
                            });
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return listSizes;
        }
        public async Task<int> Update (ShoeSizeModel model) {
            int rowAffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {TableName} SET size = @size WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", model.ID);
                    cmd.Parameters.AddWithValue ("@size", model.Size);
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

        public async Task<int> CreateProductShoeSize (ProductShoeSizeModel model) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = "";
                for (int i = 0; i < model.IDShoeSizes.Length; i++) {
                    query += $"INSERT INTO {productShoeSizeTable}(id_product,id_shoe_size) VALUES(@product{i},@shoesize{i}); ";
                }
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    for (int i = 0; i < model.IDShoeSizes.Length; i++) {
                        cmd.Parameters.AddWithValue ($"@product{i}", model.IDProduct);
                        cmd.Parameters.AddWithValue ($"@shoesize{i}", model.IDShoeSizes[i]);
                    }
                    try {
                        result = await cmd.ExecuteNonQueryAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return result;
        }

        public async Task<List<ProductModel>> GetProductsByShoeSizeID (int id) {
            List<ProductModel> productModels = new List<ProductModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM product JOIN {productShoeSizeTable} ON product.id = {productShoeSizeTable}.id_product WHERE id_shoe_size = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        using (var reader = await cmd.ExecuteReaderAsync ()) {
                            while (await reader.ReadAsync ()) {
                                productModels.Add (new ProductModel () {
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
                        }
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return productModels;
        }

        public async Task<List<ShoeSizeModel>> GetShoeSizesByProductID (int id) {
            List<ShoeSizeModel> shoeSizes = new List<ShoeSizeModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} JOIN {productShoeSizeTable} ON {TableName}.id = {productShoeSizeTable}.id_shoe_size WHERE id_product = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        using (var reader = await cmd.ExecuteReaderAsync ()) {
                            while (await reader.ReadAsync ()) {
                                shoeSizes.Add (new ShoeSizeModel () {
                                    ID = Convert.ToInt32 (reader["id"]),
                                        Size = Convert.ToInt32 (reader["size"])
                                });
                            }
                        }
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return shoeSizes;
        }
        public async Task<int> DeleteProductShoeSizeByProductId (int id) {
            int rowAffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {productShoeSizeTable} WHERE id_product = @id";
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
    }
}