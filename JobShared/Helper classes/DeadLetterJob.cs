using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobShared.Helper_classes
{
    public class DeadLetterJob
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string Data { get; set; } = "";
        public string Error { get; set; } = "";
        public DateTime FailedAt { get; set; }
    }
}
