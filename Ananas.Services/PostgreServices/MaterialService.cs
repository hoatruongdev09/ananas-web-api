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
    public class MaterialService : PostgreService, IMaterialService {
        private string mainTableName = "material";
        private string productMaterialTable = "product_material";

        public string ConnectionString { get; set; }

        public string TableName { get { return mainTableName; } }
        public MaterialService () : base () {

        }
        public MaterialService (string cn) {
            ConnectionString = cn;
        }
        public async Task<int> Add (MaterialModel model) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {TableName}(material) VALUES(@material) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@material", model.Material);
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

        public async Task<MaterialModel> Get (int id) {
            MaterialModel model = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            model = new MaterialModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                Material = Convert.ToString (reader["material"])
                            };
                        }
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return model;
        }

        public async Task<List<MaterialModel>> GetList () {
            List<MaterialModel> listMaterial = new List<MaterialModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            listMaterial.Add (new MaterialModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Material = Convert.ToString (reader["material"])
                            });
                        }
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return listMaterial;
        }

        public async Task<int> Update (MaterialModel model) {
            int rowAffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {TableName} SET material = @material WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", model.ID);
                    cmd.Parameters.AddWithValue ("@material", model.Material);
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

        public async Task<List<MaterialModel>> GetMaterialsByProductID (int id) {
            List<MaterialModel> collections = new List<MaterialModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {TableName} JOIN {productMaterialTable} ON {TableName}.id = {productMaterialTable}.id_material 
                            WHERE {productMaterialTable}.id_product = @idProduct";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idProduct", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            collections.Add (new MaterialModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Material = Convert.ToString (reader["name"]),
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

        public async Task<List<ProductModel>> GetProductsByMaterialID (int id) {
            List<ProductModel> products = new List<ProductModel> ();
            string tableProduct = "product";
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {tableProduct} JOIN {productMaterialTable} ON {tableProduct}.id = {productMaterialTable}.id_product
                            WHERE {productMaterialTable}.id_material = @idMaterial";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idMaterial", id);
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
        public async Task<int> CreateProductMaterial (ProductMaterialModel model) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = "";
                for (int i = 0; i < model.IDMaterials.Length; i++) {
                    query += $"INSERT INTO {productMaterialTable}(id_product,id_material) VALUES(@product{i},@material{i}); ";
                }
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    for (int i = 0; i < model.IDMaterials.Length; i++) {
                        cmd.Parameters.AddWithValue ($"@product{i}", model.IDProduct);
                        cmd.Parameters.AddWithValue ($"@material{i}", model.IDMaterials[i]);
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

        public async Task<int> DeleteMaterialsByProductId (int id) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {productMaterialTable} WHERE id_product = @id";
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