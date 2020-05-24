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
    public class CategoryService : PostgreService, ICategoryService {
        public string ConnectionString { get; set; }
        public string TableName { get { return tableString; } }

        private readonly string tableString = "category";
        private string productCategoryTable = "product_category";
        public CategoryService () : base () { }
        public CategoryService (string connectionString) {
            ConnectionString = connectionString;
        }

        public async Task<int> Add (CategoryModel category) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {TableName}(name,parent) VALUES(@name,@parent) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", category.Name);
                    cmd.Parameters.AddWithValue ("@parent", category.Parent);
                    try {
                        id = Convert.ToInt32 (await cmd.ExecuteScalarAsync ());
                    } catch (Exception e) {
                        Debug.WriteLine ($"{e.Message} | {e.StackTrace}");
                    }

                }
                await cn.CloseAsync ();
            }
            return id;
        }

        public async Task<List<CategoryModel>> GetList () {
            List<CategoryModel> listCategory = new List<CategoryModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    var reader = await cmd.ExecuteReaderAsync ();
                    while (await reader.ReadAsync ()) {
                        listCategory.Add (new CategoryModel () {
                            ID = Convert.ToInt32 (reader["id"]),
                                Name = Convert.ToString (reader["name"]),
                                Parent = Convert.ToInt32 (reader["parent"])
                        });
                    }
                    await reader.CloseAsync ();
                }
                await cn.CloseAsync ();
            }
            return listCategory;
        }

        public async Task<CategoryModel> Get (int id) {
            CategoryModel category = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @ID";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@ID", id);
                    var reader = await cmd.ExecuteReaderAsync ();
                    if (await reader.ReadAsync ()) {
                        category = new CategoryModel () {
                            ID = Convert.ToInt32 (reader["id"]),
                            Name = Convert.ToString (reader["name"]),
                            Parent = Convert.ToInt32 (reader["parent"])
                        };
                    }
                    await reader.CloseAsync ();
                }
                await cn.CloseAsync ();
            }
            return category;
        }

        public async Task<int> Delete (int id) {
            int rowEffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {TableName} WHERE id = @ID";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@ID", id);
                    rowEffect = Convert.ToInt32 (await cmd.ExecuteNonQueryAsync ());
                }
                await cn.CloseAsync ();
            }
            return rowEffect;
        }

        public async Task<int> Update (CategoryModel category) {
            int rowEffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {TableName} SET name = @name, parent = @parent WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", category.Name);
                    cmd.Parameters.AddWithValue ("@parent", category.Parent);
                    cmd.Parameters.AddWithValue ("@id", category.ID);
                    rowEffect = Convert.ToInt32 (await cmd.ExecuteNonQueryAsync ());
                }
                await cn.CloseAsync ();
            }
            return rowEffect;
        }

        public async Task<List<CategoryModel>> GetCategoriesByProductID (int productID) {
            List<CategoryModel> categories = new List<CategoryModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {TableName} JOIN {productCategoryTable} ON {TableName}.id = {productCategoryTable}.id_category 
                            WHERE {productCategoryTable}.id_product = @idProduct";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idProduct", productID);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            categories.Add (new CategoryModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                    Name = Convert.ToString (reader["name"]),
                                    Parent = Convert.ToInt32 (reader["parent"])
                            });
                        }
                        await reader.CloseAsync ();
                    } catch (Exception e) {
                        throw e;
                    }
                }
                await cn.CloseAsync ();
            }
            return categories;
        }
        public async Task<List<ProductModel>> GetProductByCategoryID (int categoryID) {
            List<ProductModel> products = new List<ProductModel> ();
            string tableProduct = "product";
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $@"SELECT * FROM {tableProduct} JOIN {productCategoryTable} ON {tableProduct}.id = {productCategoryTable}.id_product
                            WHERE {productCategoryTable}.id_category = @idCategory";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@idCategory", categoryID);
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

        public async Task<int> CreateProductCategory (ProductCategoryModel model) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = "";
                for (int i = 0; i < model.IDCategories.Length; i++) {
                    query += $"INSERT INTO {productCategoryTable}(id_product,id_category) VALUES(@product{i},@category{i}); ";
                }
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    for (int i = 0; i < model.IDCategories.Length; i++) {
                        cmd.Parameters.AddWithValue ($"@product{i}", model.IDProduct);
                        cmd.Parameters.AddWithValue ($"@category{i}", model.IDCategories[i]);
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

        public async Task<int> DeleteCategoriesByProductId (int id) {
            int result = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"DELETE FROM {productCategoryTable} WHERE id_product = @id";
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