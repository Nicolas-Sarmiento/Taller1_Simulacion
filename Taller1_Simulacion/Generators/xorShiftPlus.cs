using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    /// <summary>
    /// Implementación avanzada del algoritmo XorShift+ de 128 bits.
    /// Diseñado específicamente para superar pruebas estadísticas rigurosas (BigCrush) 
    /// y optimizado para la generación de números de punto flotante de doble precisión.
    /// </summary>
    internal class xorShiftPlus : IRandomGenerator
    {
        ulong seed0, seed1;
        int shift1, shift2, shift3;
        ulong[] state;

        /// <summary>
        /// Inicializa el generador. Usa el reloj del sistema como semilla de alta entropía.
        /// </summary>
        /// <param name="shift1">Desplazamiento a para el algoritmo (ej. 23).</param>
        /// <param name="shift2">Desplazamiento b para el algoritmo (ej. 17).</param>
        /// <param name="shift3">Desplazamiento c para el algoritmo (ej. 26).</param>
        public xorShiftPlus( int shift1, int shift2, int shift3)
        {
            if ( shift1 <= 0 || shift2 <= 0 || shift3 <= 0)
            {
                throw new ArgumentException("Los parámetros deben ser enteros positivos.");
            }
           
            this.shift1 = shift1;
            this.shift2 = shift2;
            this.shift3 = shift3;

            ulong seed = (ulong)DateTime.UtcNow.Ticks;

            seed0 = SplitMix64(ref seed);
            seed1 = SplitMix64(ref seed);

            if (seed0 == 0 && seed1 == 0) seed0 = 0x9E3779B97F4A7C15;


            state = new ulong[] { seed0, seed1 };

        }

        /// <summary>
        /// Función mezcladora (Mixer) que asegura que semillas similares (como Ticks consecutivos)
        /// produzcan estados iniciales drásticamente diferentes.
        /// </summary>
        
        private ulong SplitMix64(ref ulong state)
        {
            ulong z = (state += 0x9E3779B97F4A7C15);
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            return z ^ (z >> 31);
        }

        /// <summary>
        /// Calcula el siguiente estado usando XorShift y la suma '+' final.
        /// Convierte el resultado a un double usando un desplazamiento de 53 bits 
        /// para obtener precisión matemática perfecta en el estándar IEEE 754.
        /// </summary>
        public double Next()
        {
            ulong x = state[0];
            ulong y = state[1];
            state[0] = y;

            x ^= x << shift1;
            state[1] = x ^ y ^ (x >> shift2) ^ (y >> shift3);
            ulong result = state[1] + y;

            return (result >> 11) * (1.0 / (1UL << 53));
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
