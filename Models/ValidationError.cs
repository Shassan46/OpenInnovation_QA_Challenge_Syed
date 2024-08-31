using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using NUnit.Framework;
using RestSharp.Serializers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenInnovation_QA_Challenge.Models
{
    public class ValidationError
    {
        public List<ValidationDetail> Detail { get; set; }
    }
    public class ValidationDetail
    {
        public string Type { get; set; }
        public List<string> Loc { get; set; }
        public string Msg { get; set; }
        public object Input { get; set; }
    } 
}
