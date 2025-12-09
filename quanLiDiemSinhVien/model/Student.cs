using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class Student : IComparable<Student>
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Student(string id, string name, string email)
        {
            Id = id.Trim();
            Name = name.Trim();
            Email = email.Trim();
        }

        // Implement hàm CompareTo của interface
        public int CompareTo(Student other)
        {
            if (other == null) return 1;

            // So sánh chuỗi không phân biệt hoa thường (IgnoreCase) chuẩn C#
            return string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return $"{Id} - {Name} - {Email}";
        }
    }
}
