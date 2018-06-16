using System;
using CompositeModel;

namespace StockModel {
    public class StockInterceptor : IInterceptor<Composite>, IInterceptorProperties {

        public Composite Composite { get; set; } // Stock contains several values
        public IHandleDb HandleDb { get; set; }

        public Composite Get() {
            return null;
        }

        public void Set(Composite val) {
            // TODO
        }

        public Composite ToComposite(IStock stock) {
            // TODO
            return null;
        }

    }
}
