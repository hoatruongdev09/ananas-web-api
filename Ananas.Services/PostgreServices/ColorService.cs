using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Ananas.Utility.Logger;
using Npgsql;
namespace Ananas.Services.PostgreServices {
    public class ColorService : PostgreService, IColorService {

        public string ConnectionString { get; set; }

        public string TableName { get { return mainTableName; } }

        private readonly string mainTableName = "color";
        private string productColorTable = "product_color";
        private ModifiedDebugger debugger;
        public ColorService () : base () { }
        public ColorService (string connectionString) {
            ConnectionString = connectionString;
            debugger = new ModifiedDebugger ();
        }

        public async Task<int> Add (ColorModel color) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {TableName}(name,code) VALUES(@name, @code) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", color.Name);
                    cmd.Parameters.AddWithValue ("@code", color.Code);
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
            int rowEffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        rowEffect = await cmd.ExecuteNonQueryAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return rowEffect;
        }

        public async Task<ColorModel> Get (int id) {
            ColorModel color = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            color = new ColorModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                Name = Convert.ToString (reader["name"]),
                                Code = Convert.ToString (reader["code"])
                            };
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return color;
        }

        public async Task<List<ColorModel>> GetList () {
            List<ColorModel> colorModels = new List<ColorModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            colorModels.Add (new ColorModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Name = Convert.ToString (reader["name"]),
                                    Code = Convert.ToString (reader["code"])
                            });
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return colorModels;
        }

        public async Task<int> Update (ColorModel color) {
            int rowEffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {TableName} SET name = @name, code = @code WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", color.ID);
                    cmd.Parameters.AddWithValue ("@code", color.Code);
                    cmd.Parameters.AddWithValue ("@name", color.Name);
                    try {
                        rowEffect = await cmd.ExecuteNonQueryAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return rowEffect;
        }
        public async Task<int> CreateProductColor (ProductColorModel model) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = "";
                for (int i = 0; i < model.IDColor.Length; i++) {
                    query += $"INSERT INTO {productColorTable}(id_color,id_product) VALUES(@idColor{i},@product{i}); ";
                }
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    for (int i = 0; i < model.IDColor.Length; i++) {
                        cmd.Parameters.AddWithValue ($"@product{i}", model.IDProduct);
                        cmd.Parameters.AddWithValue ($"@idColor{i}", model.IDColor[i]);
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
        public async Task<List<ColorModel>> GetColorByProductID (int idProduct) {
            List<ColorModel> collections = new List<ColorModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {TableName} JOIN {productColorTable} ON {TableName}.id = {productColorTable}.id_color 
                            WHERE {productColorTable}.id_product = @idProduct";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idProduct", idProduct);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            collections.Add (new ColorModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Name = Convert.ToString (reader["name"]),
                                    Code = Convert.ToString (reader["Code"])
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
        public async Task<List<ProductModel>> GetProductByColorID (int idColor) {
            List<ProductModel> products = new List<ProductModel> ();
            string tableProduct = "product";
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {tableProduct} JOIN {productColorTable} ON {tableProduct}.id = {productColorTable}.id_product
                            WHERE {productColorTable}.id_color = @idColor";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idColor", idColor);
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

        public async Task<int> DeleteColorsByProductId (int id) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {productColorTable} WHERE id_product = @id";
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