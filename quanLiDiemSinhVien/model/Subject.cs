using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class Subject
    {
        public string Id { get; private set; } // Chỉ đọc
        public string Name { get; set; }       // Đọc/Ghi
        public int Credits { get; set; }       // Đọc/Ghi

        public Subject(string id, string name, int credits)
        {
            Id = id.Trim();
            Name = name.Trim();
            Credits = credits;
        }

        public override string ToString()
        {
            // Dùng String Interpolation ($) dễ đọc hơn String.Format
            return $"{Id} - {Name} ({Credits} tín chỉ)";
        }
    }
}
