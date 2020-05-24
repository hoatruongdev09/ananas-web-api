using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Npgsql;
namespace Ananas.Services.PostgreServices {
    public class CollectionService : PostgreService, ICollectionService {
        public string ConnectionString { get; set; }

        public string TableName { get { return tableName; } }
        private string tableName = "collection";

        private string productCollectionTable = "product_collection";
        public CollectionService () : base () { }
        public CollectionService (string connectionString) {
            ConnectionString = connectionString;
        }
        public async Task<int> Add (CollectionModel model) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {tableName}(name) VALUES(@name) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", model.Name);
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
                string query = $"DELETE FROM {tableName} WHERE id = @id";
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

        public async Task<CollectionModel> Get (int id) {
            CollectionModel model = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {tableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            model = new CollectionModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                Name = Convert.ToString (reader["name"])
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

        public async Task<List<CollectionModel>> GetList () {
            List<CollectionModel> listModels = new List<CollectionModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {tableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            listModels.Add (new CollectionModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Name = Convert.ToString (reader["name"])
                            });
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return listModels;
        }

        public async Task<int> Update (CollectionModel model) {
            int rowAffected = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {tableName} SET name = @name WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", model.Name);
                    cmd.Parameters.AddWithValue ("@id", model.ID);
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
        public async Task<int> CreateProductCollection (ProductCollectionModel model) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = "";
                for (int i = 0; i < model.IDCollections.Length; i++) {
                    query += $"INSERT INTO {productCollectionTable}(id_collection,id_product) VALUES(@collection{i},@product{i}); ";
                }
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    for (int i = 0; i < model.IDCollections.Length; i++) {
                        cmd.Parameters.AddWithValue ($"@product{i}", model.IDProduct);
                        cmd.Parameters.AddWithValue ($"@collection{i}", model.IDCollections[i]);
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

        public async Task<List<CollectionModel>> GetCollectionsByProductID (int productID) {
            List<CollectionModel> collections = new List<CollectionModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {TableName} JOIN {productCollectionTable} ON {TableName}.id = {productCollectionTable}.id_collection 
                            WHERE {productCollectionTable}.id_product = @idProduct";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idProduct", productID);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            collections.Add (new CollectionModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Name = Convert.ToString (reader["name"]),
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

        public async Task<List<ProductModel>> GetProductsByCollectionID (int collectionID) {
            List<ProductModel> products = new List<ProductModel> ();
            string tableProduct = "product";
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {tableProduct} JOIN {productCollectionTable} ON {tableProduct}.id = {productCollectionTable}.id_product
                            WHERE {productCollectionTable}.id_collection = @idCollection";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idCollection", collectionID);
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

        public async Task<int> DeleteCollectionsByProductId (int id) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {productCollectionTable} WHERE id_product = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
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
    }
}