using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuantumPowerball.Core.Rng.Models
{
    [DataContract]
    public class QrngResponse
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "length")]
        public short Length { get; set; }

        [DataMember(Name = "data")]
        public byte[] Data { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }
    }
}
