using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public class Dice
    {
        private static Random rand = new Random();

        public static DiceRoll RollDice(int count, int faces)
        {
            var rolls = new DiceRoll();

            for (int i = 0; i < count; i++)
            {
                rolls.Add(rand.Next(faces) + 1);
            }

            return rolls;
        }
    }
}
