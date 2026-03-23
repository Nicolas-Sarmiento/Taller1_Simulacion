using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    /// <summary>
    /// Interfaz que define el contrato estándar para todos los generadores 
    /// de números pseudoaleatorios utilizados en la simulación.
    /// Permite intercambiar generadores (Polimorfismo) sin alterar el motor principal.
    /// </summary>
    internal interface IRandomGenerator
    {
        /// <summary>
        /// Genera el siguiente número pseudoaleatorio en una distribución uniforme continua.
        /// </summary>
        /// <returns>Un valor de punto flotante en el rango [0.0, 1.0).</returns>
        double Next();

        /// <summary>
        /// Genera el siguiente número pseudoaleatorio entero dentro de un rango específico.
        /// </summary>
        /// <param name="maxNum">El límite superior exclusivo (el número generado será menor a este valor).</param>
        /// <returns>Un entero en el rango [0, maxNum).</returns>
        int Next(int maxNum);
    }
}
