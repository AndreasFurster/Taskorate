using System;
using System.Collections.Generic;
using System.Text;

namespace Taskorate.Models
{
    public class QuickTaskList
    {
        public QuickTaskList(string name)
        {
            Name = name;
        }

        public QuickTaskList()
        {
            
        }

        public Guid? Id { get; set; }
        public string Name { get; set; }
        public List<QuickTask> Tasks { get; set; }
    }
}
