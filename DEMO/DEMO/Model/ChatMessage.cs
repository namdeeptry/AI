using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEMO.Model
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsUser { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
