using QuantumPowerball.Core.Rng.Models;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

namespace QuantumPowerball.Core.Rng
{
    public class RngService
    {
        private const string ANU_QRNG_URL_FORMATTER = "https://qrng.anu.edu.au/API/jsonI.php?length={0}&type=uint8";
        private const string RANDOM_ORG_URL_FORMATTER = "https://www.random.org/integers/?num={0}&min=0&max=255&col=1&base=10&format=plain&rnd=new";

        public RngResult GetBytes(short length)
        {
            if (length > 1024)
                throw new ArgumentOutOfRangeException("length", "Maximum length is 1024");

            var data = new byte[length];
            var quality = default(QrngQuality);

            if (TryGetQuantumBytes(data))
                quality = QrngQuality.Quantum;

            else if (TryGetAtmosphericNoiseBytes(data))
                quality = QrngQuality.AtmosphericNoise;

            else
            {
                GetRngCryptoServiceProviderBytes(data);
                quality = QrngQuality.RNGCryptoServiceProvider;
            }

            return new RngResult
            {
                Bytes = data,
                Quality = quality
            };
        }

        private bool TryGetQuantumBytes(byte[] data)
        {
            var url = string.Format(ANU_QRNG_URL_FORMATTER, data.Length);
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server Error (HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
                    var jsonSerializer = new DataContractJsonSerializer(typeof(RngResult));
                    var qrngResponse = jsonSerializer.ReadObject(response.GetResponseStream()) as QrngResponse;
                    if (qrngResponse.Success)
                    {
                        qrngResponse.Data.CopyTo(data, 0);
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private bool TryGetAtmosphericNoiseBytes(byte[] data)
        {
            var url = string.Format(RANDOM_ORG_URL_FORMATTER, data.Length);
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server Error (HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
                    var lines = new StreamReader(response.GetResponseStream()).ReadToEnd().Trim().Split('\n');
                    if(data.Length == lines.Length)
                    {
                        for (var i = 0; i < data.Length; i++)
                        {
                            data[i] = byte.Parse(lines[i].Trim());
                        }
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private void GetRngCryptoServiceProviderBytes(byte[] value)
        {
            var rnd = new RNGCryptoServiceProvider();
            rnd.GetBytes(value);
        }
    }
}
