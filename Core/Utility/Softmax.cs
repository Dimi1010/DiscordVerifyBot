using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordVerifyBot.Core.Utility
{
    static class Softmax
    {
        public static IEnumerable<double> Compute(IEnumerable<double> scores)
        {
            var scores_exp = scores.Select(Math.Exp);

            var scores_sum_exp = scores_exp.Sum();

            var softmax = scores_exp.Select(i => i / scores_sum_exp);

            return softmax;
        }
    }
}
