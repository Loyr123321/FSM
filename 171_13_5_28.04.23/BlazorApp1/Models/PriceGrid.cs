using System.Globalization;

namespace BlazorApp1.Models
{
    public class BasicServices
    {
        public decimal RealPrice { get; set; }
        public decimal ClientPrice { get; set; }
        public decimal ExecutorPrice { get; set; } //цена монтажника установлена заранее на каждом монтажнике
        public decimal ShopProfit { get; set; }
        public decimal Profit { get; set; }

        public decimal Discount { get; set; } //скидка
        public decimal DiscountProcent { get; set; } //скидка в процентах

        public int Procent { get; set; } //Наш процент от профита магаза

        public bool Validation()
        {
            return false;
        } 

        public void CalculatePrice()
        {
            //clear
            ShopProfit = 0;
            ClientPrice = 0;
            Profit = 0;

            //цена клиента
            ClientPrice = RealPrice - Discount;
            decimal oneProcentClientPrice = ClientPrice / 100;
            ClientPrice = ClientPrice - (oneProcentClientPrice * DiscountProcent);

            ShopProfit = ClientPrice - ExecutorPrice;

            if (ShopProfit > 0)
            {
                decimal oneProcent = ShopProfit / 100;
                Profit = oneProcent * Procent;

                ShopProfit = ShopProfit - Profit;
            }

        }
    }

    public class AdditionalServices
    {

    }

    public class PriceGrid
    {
       
    }
}
