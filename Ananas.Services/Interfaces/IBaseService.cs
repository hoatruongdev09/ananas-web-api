using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ananas.Services.Interfaces {

    public interface IBaseService<T> {
        string ConnectionString { get; set; }
        string TableName { get; }
        Task<T> Get (int id);
        Task<List<T>> GetList ();
        Task<int> Add (T model);
        Task<int> Delete (int id);
        Task<int> Update (T model);
    }
}