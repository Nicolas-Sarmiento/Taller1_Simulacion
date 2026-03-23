using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    /// <summary>
    /// Implementación del Generador Congruencial Lineal (Linear Congruential Generator).
    /// Utiliza la fórmula matemática: X_{n+1} = (a * X_n + c) mod m
    /// </summary>
    internal class LCG : IRandomGenerator
    {
        private long x, seed, multiplier, additive, mod;
        private long lastXn = 0;
        public long LastXn { get { return lastXn; } }

        /// <summary>
        /// Inicializa un nuevo generador LCG con sus parámetros fundamentales.
        /// </summary>
        /// <param name="seed">X_0 (Semilla inicial).</param>
        /// <param name="multiplier">a (Multiplicador).</param>
        /// <param name="additive">c (Incremento o constante aditiva).</param>
        /// <param name="mod">m (Módulo, define el periodo máximo posible).</param>
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
        /// Calcula el siguiente estado usando la fórmula congruencial y lo normaliza.
        /// </summary>
        /// <returns>Valor uniforme Un en el rango [0.0, 1.0].</returns>
        public double Next()
        {
            x = ((multiplier * x) + additive) % mod;
            lastXn = x;
            return (double) x / ( mod - 1.0 );
        }

        /// <summary>
        /// Escala el número uniforme al rango entero solicitado.
        /// </summary>
        public int Next(int maxNum)
        {
            if (maxNum <= 0) { 
                throw new ArgumentException("maxNum debe ser mayor a 0.");
            }
            return (int)(Next() * maxNum);
        }

    }
}
