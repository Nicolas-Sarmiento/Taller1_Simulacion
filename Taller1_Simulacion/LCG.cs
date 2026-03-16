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

        public LCG(long seed, long multiplier, long additive, long mod)
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

        /// <summary>
        /// Valida que los parámetros cumplan las condiciones de Hull-Dobell
        /// para garantizar un período completo de m.
        /// </summary>
        public static bool ValidateHullDobell(long a, long c, long m)
        {
            // Condición 1: gcd(c, m) = 1
            if (GCD(c, m) != 1) return false;

            // Condición 2: a - 1 es divisible por todos los factores primos de m
            long am1 = a - 1;
            long temp = m;

            // Factorizar m
            for (long p = 2; p * p <= temp; p++)
            {
                if (temp % p == 0)
                {
                    if (am1 % p != 0) return false;
                    while (temp % p == 0) temp /= p;
                }
            }
            if (temp > 1 && am1 % temp != 0) return false;

            // Condición 3: Si m es divisible por 4, entonces a - 1 también
            if (m % 4 == 0 && am1 % 4 != 0) return false;

            return true;
        }

        private static long GCD(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public double Next()
        {
            x = ((multiplier * x) + additive) % mod;
            lastXn = x;
            return (double) x / ( mod - 1.0 );
        }

    }
}
