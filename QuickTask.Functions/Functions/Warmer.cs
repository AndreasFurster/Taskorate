using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs;

namespace QuickTask.Functions.Functions
{
    public static class Warmer
    {
        // Keep stuff warm :)
        [FunctionName("Warmer")]
        public static void Run([TimerTrigger("0 */15 * * * *")] TimerInfo timer)
        {
            // Do nothing
        }
    }
}
