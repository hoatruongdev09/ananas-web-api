using System;
namespace Ananas.Services {
    public abstract class BaseService {
        public abstract string ConnectionName { get; protected set; }
    }
}