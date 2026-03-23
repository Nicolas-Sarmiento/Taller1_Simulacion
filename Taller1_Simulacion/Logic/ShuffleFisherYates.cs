
using System;
using System.Collections.Generic;


namespace Taller1_Simulacion
{
    /// <summary>
    /// Implementación del algoritmo de mezcla Fisher-Yates (o Knuth Shuffle).
    /// Permite desordenar aleatoriamente una lista de elementos en tiempo O(N) 
    /// para romper posibles correlaciones seriales en los datos generados.
    /// </summary>
    internal class ShuffleFisherYates
    {
        IRandomGenerator _rng;

        /// <summary>
        /// Inicializa el mezclador inyectando la dependencia del generador aleatorio.
        /// </summary>
        /// <param name="rng">Generador de números (LCG o XorShift) que decidirá los intercambios.</param>
        public ShuffleFisherYates(IRandomGenerator rng)
        {
            _rng = rng;
        }

        /// <summary>
        /// Mezcla (shuffles) los elementos de cualquier lista "In-Place" (modificando la lista original directamente).
        /// Recorre la lista de atrás hacia adelante intercambiando el elemento actual con un índice aleatorio previo.
        /// </summary>
        /// <typeparam name="T">El tipo de dato que contiene la lista (ej. double, int).</typeparam>
        /// <param name="list">La lista que será desordenada.</param>
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = _rng.Next(i);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
