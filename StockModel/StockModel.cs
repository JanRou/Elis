using DataAccessLayer.Tables;
using StockModel.Model;

namespace StockModel {

    public class StockModel {

        private readonly IHandleDb _handleDb = null;
        private readonly IModel _model;

        public StockModel(IModel model, IHandleDb handleDb) {
            _handleDb = handleDb;
            _model = model;
        }

        public void Init() {
            string name = "Stock";
            string path = "/" + name;
            _model.Set("/", new Composite(name));
            _model.Set(path, new Composite("???"));
            path += "/" + "";
            foreach (var stock in _handleDb.GetStocks()) {
                IComposite dpComposite = CreateStock(stock);
                _model.Set(path, dpComposite);
            }
        }

        public IComposite CreateStock(IStock stock) {
            // TODO
            return null;
        }
    }
}
