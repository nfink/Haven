using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public class DiceRoll : List<int>
    {
        public int Sum
        {
            get
            {
                return this.Sum();
            }
        }

        public bool Doubles
        {
            get
            {
                return this.Distinct().Count() < this.Count;
            }
        }
    }
}
