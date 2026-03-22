using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    internal class xorShift
    {
        public ulong x { get; private set; }
        private int shift1;
        private int shift2;
        private int shift3;
        private ulong seed;

        public xorShift(ulong seed, int shift1, int shift2, int shift3)
        {
            if ( seed <= 0 || shift1 <= 0 || shift2 <= 0 || shift3 <= 0)
            {
                throw new ArgumentException("Los parámetros deben ser enteros positivos.");
            }
            this.seed = seed;
            this.shift1 = shift1;
            this.shift2 = shift2;
            this.shift3 = shift3;
            this.x = seed;
        }


        public double Next()
        {
            x ^= x << this.shift1;
            x ^= x >> this.shift2;
            x ^= x >> this.shift3;

            return (double) x / ulong.MaxValue;

        }

    }
}
