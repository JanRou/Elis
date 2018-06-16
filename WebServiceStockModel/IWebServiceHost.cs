namespace WebServiceStockModel {
    public interface IWebServiceHost {
        void Start();
        void Stop();
        void DebugRun();
        void DebugStop();
    }
}