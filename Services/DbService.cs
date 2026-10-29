using trpo8.Models;

namespace trpo8.Services
{
    public class DbService
    {
        private VoroshilovTrpo8DbContext _context;
        public VoroshilovTrpo8DbContext Context => _context;

        private static DbService instance;
        public static DbService Instance
        {
            get
            {
                if (instance == null) instance = new DbService();
                return instance;
            }
        }
        private DbService()
        {
            _context = new VoroshilovTrpo8DbContext();
        }
    }
}
