using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApi.Common.Models
{
    public class TodoItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsComplete { get; set; }
    }
}
