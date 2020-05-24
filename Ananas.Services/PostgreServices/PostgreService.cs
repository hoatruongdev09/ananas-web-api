namespace Ananas.Services.PostgreServices {
    public class PostgreService : BaseService {
        public override string ConnectionName { get; protected set; }
        public PostgreService () {
            ConnectionName = "PostgreSQL";
        }
    }
}