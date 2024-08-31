using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenInnovation_QA_Challenge.Models
{
    internal class ModelVersion
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentModelId { get; set; }
        public string HuggingFaceModel { get; set; }
    }
}
