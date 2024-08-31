using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenInnovation_QA_Challenge.Models
{
    internal class InferenceArguments
    {
        public string Text { get; set; }
        public bool ApplyTemplate { get; set; } = false;
        public int MaxNewTokens { get; set; } = 256;
        public bool DoSample { get; set; } = true;
        public double Temperature { get; set; } = 0.7;
        public int TopK { get; set; } = 50;
        public double TopP { get; set; } = 0.95;
    }
}
