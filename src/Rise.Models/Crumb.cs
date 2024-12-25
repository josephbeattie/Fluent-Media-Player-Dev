using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Models
{
    public struct Crumb
    {
        public string Title { get; set; }
        public Type Type { get; private set; }

        public Crumb(string title, Type type)
        {
            Title = title;
            Type = type;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
