using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taskorate.Models
{
    public class QuickTask
    {
        
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }

        public QuickTask()
        {
            
        }

        public QuickTask(string name)
        {
            Name = name;
        }
    }
}
