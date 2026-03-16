using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    internal class LCG : IRandomGenerator
    {
        private long x, seed, multiplier, additive, mod;
        private long lastXn = 0;
        public long LastXn { get { return lastXn; } }

        public LCG(int seed, int multiplier, int additive, int mod)
        {
            if (mod <= 1) { 
                throw new ArgumentException("Modulo debe ser mayor a 1.");
            }
            this.seed = seed;
            this.multiplier = multiplier;
            this.additive = additive;
            this.mod = mod;
            x = seed;
            lastXn = x;
        }

        public double Next()
        {
            x = ((multiplier * x) + additive) % mod;
            lastXn = x;
            return (double) x / ( mod - 1.0 );
        }

    }
}
