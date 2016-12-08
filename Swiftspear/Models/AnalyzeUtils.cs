using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Swiftspear.Models
{
    public class AnalyzeUtils
    {
        /// <summary>
        /// 離散的な音声データを受け取り、フーリエ変換した結果を返します。窓関数にはハミング窓を用います。
        /// </summary>
        /// <param name="latestReceived">1要素1サンプルの音声データ。要素数が2の累乗個である必要があります。</param>
        /// <returns>フーリエ変換によって得られた周波数（0.1Hz刻み）とその周波数成分の組を列挙します。</returns>
        public static IEnumerable<Tuple<double, double>> GetFreqSpectrum(IEnumerable<int> latestReceived)
        {
            if (!latestReceived.Any())
            {
                return new List<Tuple<double, double>>();
            }

            var buf = latestReceived.ToArray();
            var window = Window.Hamming(buf.Length);
            var complex = buf.Select((v, i) => new Complex(v * window[i], 0.0)).ToArray();
            Fourier.Forward(complex, FourierOptions.Matlab);
            var points = complex.Take(complex.Length / 2).Select((v, i) =>
                  Tuple.Create<double, double>(i, Math.Sqrt(v.Real * v.Real + v.Imaginary * v.Imaginary))
              );
            return points;
        }

        /// <summary>
        /// byte2つ（16bit）のサンプル列をintの列に変換します。
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static IEnumerable<int> AppendBytes(IEnumerable<byte> bytes)
        {
            if (bytes == null)
            {
                yield break;
            }

            var buf = bytes.ToArray();
            var len = buf.Count();

            for (var index = 0; index < len; index += 2)
            {
                var sample = (short)((buf[index + 1] << 8) | buf[index + 0]);
                yield return sample;
            }
        }
    }
}
