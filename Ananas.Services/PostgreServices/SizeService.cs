using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Npgsql;
namespace Ananas.Services.PostgreServices {
    public class SizeService : PostgreService, ISizeService {
        private string mainTableName = "size";
        private string productSizeTable = "product_size";

        public string ConnectionString { get; set; }

        public string TableName { get { return mainTableName; } }
        public SizeService () : base () { }
        public SizeService (string cn) {
            ConnectionString = cn;
        }
        public async Task<int> Add (SizeModel model) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {TableName}(tag) VALUES(@tag) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@tag", model.Tag);
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
            int rowAffected = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        rowAffected = await cmd.ExecuteNonQueryAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return rowAffected;
        }

        public async Task<SizeModel> Get (int id) {
            SizeModel size = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            size = new SizeModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                Tag = Convert.ToString (reader["tag"])
                            };
                        }
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return size;
        }

        public async Task<List<SizeModel>> GetListAll () {
            List<SizeModel> listSizes = new List<SizeModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            listSizes.Add (new SizeModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Tag = Convert.ToString (reader["tag"])
                            });
                        }
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return listSizes;
        }

        public async Task<int> Update (SizeModel model) {
            int rowAffected = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {TableName} SET tag = @tag WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", model.ID);
                    cmd.Parameters.AddWithValue ("@tag", model.Tag);
                    try {
                        rowAffected = await cmd.ExecuteNonQueryAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return rowAffected;
        }
        public async Task<int> CreateProductSize (ProductSizeModel model) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = "";
                for (int i = 0; i < model.IDSizes.Length; i++) {
                    query += $"INSERT INTO {productSizeTable}(id_product,id_size) VALUES(@product{i},@size{i}); ";
                }
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    for (int i = 0; i < model.IDSizes.Length; i++) {
                        cmd.Parameters.AddWithValue ($"@product{i}", model.IDProduct);
                        cmd.Parameters.AddWithValue ($"@size{i}", model.IDSizes[i]);
                    }
                    result = await cmd.ExecuteNonQueryAsync ();
                }
                await cn.CloseAsync ();
            }
            return result;
        }
        public async Task<List<ProductModel>> GetProductsBySizeID (int id) {
            List<ProductModel> products = new List<ProductModel> ();
            string tableProduct = "product";
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {tableProduct} JOIN {productSizeTable} ON {tableProduct}.id = {productSizeTable}.id_product
                            WHERE {productSizeTable}.id_size = @idSize";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idSize", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            products.Add (new ProductModel () {
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
                                    Category = Convert.ToInt32 (reader["category"])
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
            return products;
        }

        public async Task<List<SizeModel>> GetSizesByProductID (int id) {
            List<SizeModel> collections = new List<SizeModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {TableName} JOIN {productSizeTable} ON {TableName}.id = {productSizeTable}.id_size 
                            WHERE {productSizeTable}.id_product = @idProduct";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idProduct", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            collections.Add (new SizeModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Tag = Convert.ToString (reader["tag"])
                            });
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return collections;
        }

        public async Task<int> DeleteSizesByProductId (int id) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {productSizeTable} WHERE id_product = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    result = await cmd.ExecuteNonQueryAsync ();
                }
                await cn.CloseAsync ();
            }
            return result;
        }

        public Task<List<SizeModel>> GetList (int pageIndex = 0, int pageCount = 10) {
            throw new NotImplementedException ();
        }
    }
}