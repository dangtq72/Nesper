using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvs_Nesper
{
    public class OrderEvent
    {
        private String itemName;
        private double price;

        public OrderEvent(string itemName, double price)
        {
            this.itemName = itemName;
            this.price = price;
        }

        public String getItemName()
        {
            return itemName;
        }

        public double getPrice()
        {
            return price;
        }
    }
}
