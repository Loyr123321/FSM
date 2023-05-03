using NPOI.SS.Formula.Functions;

namespace BlazorApp1.Models
{
    public class StoreService
    {
        public string ServiceName { get; private set; }
        public double ServicePrice { get; private set; }
        public double StorePrecent { get; private set; }
        public double StoreMoney { get; private set; }
        public double ExecutorAndOurMoney  { get; private set; }
        public Store Store { get; private set; }

        public StoreService(string serviceName, double servicePrice, double storePrecent, double storeMoney, Store store) 
        {
            if (storePrecent>100 || storePrecent < 0) 
            {
                throw new ArgumentException("wrongPercentParams_TEST1");
            }

            this.ServiceName = serviceName;
            this.ServicePrice = servicePrice;
            this.StorePrecent = storePrecent;



            this.StoreMoney = storeMoney;
            this.Store = store;
        }
    }
}
