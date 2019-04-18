using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMaker
{
    public class CoffeeMachine
    {
        public double Water;
        public double Sugar;
        public double Coffee;
        public int MoneyInMachine;

        public CoffeeMachine(double Water, double Sugar, double Coffee)
        {
            this.Water = Water;
            this.Sugar = Sugar;
            this.Coffee = Coffee;
        }
    }
}