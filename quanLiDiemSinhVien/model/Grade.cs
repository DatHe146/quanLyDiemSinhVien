using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class Grade
    {
        // Auto-implemented properties
        public string StudentId { get; private set; }
        public string SubjectId { get; private set; }
        public double Score { get; set; }

        public Grade(string studentId, string subjectId, double score)
        {
            // Trong C#, hàm Trim() viết hoa chữ T
            StudentId = studentId.Trim();
            SubjectId = subjectId.Trim();
            Score = score;
        }

        public override string ToString()
        {
            return $"{StudentId},{SubjectId},{Score}";
        }
    }
}
