using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Npgsql;
namespace Ananas.Services.PostgreServices {
    public class ProductImageService : PostgreService, IProductImageService, IBaseService<ProductImageModel> {
        public string ConnectionString { get; set; }
        public string TableName => "product_image";

        public ProductImageService () : base () { }
        public ProductImageService (string connectionString) {
            ConnectionString = connectionString;
        }

        public async Task<int> CreateProductImage (ProductImageModel model) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = "";
                for (int i = 0; i < model.Image.Length; i++) {
                    query += $"INSERT INTO {TableName}(id_product,image) VALUES(@product{i},@image{i}); ";
                }
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    for (int i = 0; i < model.Image.Length; i++) {
                        cmd.Parameters.AddWithValue ($"@product{i}", model.IDProduct);
                        cmd.Parameters.AddWithValue ($"@image{i}", model.Image[i]);
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
        public async Task<int> RemoveImageByProductId (int id) {
            int rowAffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {TableName} WHERE id_product = @id";
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
        public async Task<List<string>> GetImageByProductId (int id) {
            List<string> images = new List<string> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT image FROM {TableName} WHERE id_product = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        using (var reader = await cmd.ExecuteReaderAsync ()) {
                            while (await reader.ReadAsync ()) {
                                images.Add (Convert.ToString (reader["image"]));
                            }
                            await reader.CloseAsync ();
                        }
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return images;
        }

        public Task<int> Add (ProductImageModel model) {
            throw new NotImplementedException ();
        }

        public Task<int> Delete (int id) {
            throw new NotImplementedException ();
        }

        public Task<ProductImageModel> Get (int id) {
            throw new NotImplementedException ();
        }

        public Task<List<ProductImageModel>> GetListAll () {
            throw new NotImplementedException ();
        }

        public Task<int> Update (ProductImageModel model) {
            throw new NotImplementedException ();
        }

        public Task<List<ProductImageModel>> GetList (int pageIndex = 0, int pageCount = 10) {
            throw new NotImplementedException ();
        }
    }
}