using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Npgsql;

namespace Ananas.Services.PostgreServices {
    public class GenderService : PostgreService, IGenderService {
        public string ConnectionString { get; set; }

        public string TableName { get { return tableName; } }
        private string tableName = "gender";
        public GenderService (string connectionString) {
            ConnectionString = connectionString;
        }
        public GenderService () : base () { }

        public async Task<int> Add (GenderModel model) {
            int id = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"INSERT INTO {TableName}(name) VALUES(@name) RETURNING id";
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

        public async Task<GenderModel> Get (int id) {
            GenderModel model = null;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName} WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@id", id);
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        if (await reader.ReadAsync ()) {
                            model = new GenderModel () {
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

        public async Task<List<GenderModel>> GetList () {
            List<GenderModel> listGenders = new List<GenderModel> ();
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"SELECT * FROM {TableName}";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    try {
                        var reader = await cmd.ExecuteReaderAsync ();
                        while (await reader.ReadAsync ()) {
                            listGenders.Add (new GenderModel () {
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
            return listGenders;
        }

        public async Task<int> Update (GenderModel model) {
            int rowAffect = -1;
            using (var cn = new NpgsqlConnection (ConnectionString)) {
                await cn.OpenAsync ();
                string query = $"UPDATE {TableName} SET name = @name WHERE id = @id";
                using (var cmd = new NpgsqlCommand (query, cn)) {
                    cmd.Parameters.AddWithValue ("@name", model.Name);
                    cmd.Parameters.AddWithValue ("@id", model.ID);
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