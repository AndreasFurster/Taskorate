using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickTask.Models
{
    public class QuickTaskModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }

        public QuickTaskModel()
        {
            
        }

        public QuickTaskModel(string name)
        {
            Name = name;
        }
    }
}
