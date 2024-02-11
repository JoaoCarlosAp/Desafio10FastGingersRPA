using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desafio10FastGingersRPA.Models
{
    public class ResultModel
    {
        public string Wpm { get; set; } = string.Empty;
        public string Keystrokes { get; set; } = string.Empty;
        public string Accuracy { get; set; } = string.Empty;
        public string CorrectWords { get; set; } = string.Empty;
        public string WrongWords { get; set; } = string.Empty;
    }
}
