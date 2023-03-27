using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIKFind.Helpers
{
    class InputHelper
    {
        public static async Task<string> OptimizeInput(string input)
        {
            if (input.Contains("-"))
            {
                int indexOfFirstDash = input.IndexOf("-");
                int additionalDash = input.IndexOf("-", indexOfFirstDash + 1);

                if (additionalDash > -1)
                {
                    input = input.Remove(additionalDash, input.Length - additionalDash);
                }
            }

            if (input.Contains("("))
            {
                int indexOfDash = input.IndexOf("(");

                input.Remove(indexOfDash, input.Length - indexOfDash);
            }

            return input;
        }
    }
}
