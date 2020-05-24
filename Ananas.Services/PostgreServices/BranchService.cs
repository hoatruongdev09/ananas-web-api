using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Ananas.Utility;
using Npgsql;

namespace Ananas.Services.PostgreServices {
    public class BranchService : PostgreService, IBranchService {
        public string ConnectionString { get; set; }

        public string TableName { get { return tableName; } }

        private readonly string tableName = "branch";
        public BranchService () : base () { }
        public BranchService (string connectionString) {
            ConnectionString = connectionString;
        }
        public async Task<int> Add (BranchModel branch) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {TableName}(name,parent) VALUES(@name,@parent) RETURNING id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", branch.Name);
                    cmd.Parameters.AddWithValue ("@parent", branch.Parent);
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

        public async Task<BranchModel> Get (int id) {
            BranchModel model = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            model = new BranchModel () {
                                ID = Convert.ToInt32 (reader["id"]),
                                Name = Convert.ToString (reader["name"]),
                                Parent = Convert.ToInt32 (reader["parent"])
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

        public async Task<List<BranchModel>> GetList () {
            List<BranchModel> branchModels = new List<BranchModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            branchModels.Add (new BranchModel () {
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
            return branchModels;
        }

        public async Task<int> Update (BranchModel branch) {
            int rowAffected = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {TableName} SET name = @name, parent = @parent WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", branch.Name);
                    cmd.Parameters.AddWithValue ("@parent", branch.Parent);
                    cmd.Parameters.AddWithValue ("@id", branch.ID);
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
    }
}