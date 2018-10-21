using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Card(int id, string name)
        {
            Id = id;
            Name = name;
        }

    }
}
