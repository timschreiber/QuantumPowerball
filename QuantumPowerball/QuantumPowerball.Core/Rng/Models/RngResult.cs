using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumPowerball.Core.Rng.Models
{
    public class RngResult
    {
        public QrngQuality Quality { get; set; }
        public byte[] Bytes { get; set; }
    }

    public enum QrngQuality
    {
        Quantum,
        AtmosphericNoise,
        RNGCryptoServiceProvider
    }
}
