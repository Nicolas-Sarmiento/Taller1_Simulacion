using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    /// <summary>
    /// Implementación del algoritmo original XorShift (Generalmente de 64 bits).
    /// Genera números pseudoaleatorios extremadamente rápido utilizando 
    /// operaciones a nivel de bits (XOR y desplazamientos lógicos).
    /// </summary>
    internal class xorShift : IRandomGenerator
    {
        /// <summary>
        /// Estado interno actual del generador.
        /// </summary>
        public ulong x { get; private set; }
        private int shift1;
        private int shift2;
        private int shift3;
        private ulong seed;

        /// <summary>
        /// Inicializa el generador XorShift.
        /// </summary>
        /// <param name="seed">Semilla inicial (no debe ser cero).</param>
        /// <param name="shift1">Primer desplazamiento (usualmente a la izquierda).</param>
        /// <param name="shift2">Segundo desplazamiento (usualmente a la derecha).</param>
        /// <param name="shift3">Tercer desplazamiento.</param>
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

        /// <summary>
        /// Ejecuta los tres desplazamientos (Shifts) y operaciones exclusivas (XOR)
        /// para calcular el siguiente número de la secuencia.
        /// </summary>
        /// <returns>Valor uniforme Un en el rango [0.0, 1.0).</returns>
        public double Next()
        {
            x ^= x << this.shift1;
            x ^= x >> this.shift2;
            x ^= x >> this.shift3;

            return (double) x / ulong.MaxValue;

        }

        /// <summary>
        /// Escala el número uniforme al rango entero solicitado.
        /// </summary>
        public int Next(int maxNum)
        {
            if (maxNum <= 0)
            {
                throw new ArgumentException("maxNum debe ser mayor a 0.");
            }
            return (int)(Next() * maxNum);
        }

    }
}
