using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    /// <summary>
    /// Motor matemático encargado de ejecutar la simulación de Montecarlo 
    /// para estimar el área de regiones complejas delimitadas por funciones.
    /// </summary>
    internal class Montecarlo
    {
        
        private List<Func<Point,bool>> _constraints;
        private Point _lowerPoint, _upperPoint;

        /// <summary>
        /// Inicializa el simulador definiendo la región de interés y el rectángulo delimitador.
        /// </summary>
        /// <param name="constraint_functions">Lista de expresiones (inecuaciones) que delimitan el área (ej. Y <= Cos(X)).</param>
        /// <param name="lowerPoint">Coordenada mínima (Esquina inferior izquierda) del rectángulo delimitador.</param>
        /// <param name="upperPoint">Coordenada máxima (Esquina superior derecha) del rectángulo delimitador.</param>
        public Montecarlo( List<Func<Point, bool>> constraint_functions, Point lowerPoint, Point upperPoint) { 
           
            _constraints = constraint_functions;
            _lowerPoint = lowerPoint;
            _upperPoint = upperPoint;
        }

        /// <summary>
        /// Estima el área de la región basándose en la proporción de puntos que cayeron dentro.
        /// Fórmula: (Puntos Adentro / Total de Puntos) * Área del Rectángulo.
        /// </summary>
        public double EstimateArea( int totalPoints, int insidePoints ) { 
            double total_area = (_upperPoint.X - _lowerPoint.X) * (_upperPoint.Y - _lowerPoint.Y);
            return ((double)insidePoints / totalPoints) * total_area;
        }

        /// <summary>
        /// Convierte una lista unidimensional de valores aleatorios [0, 1) en coordenadas espaciales 2D,
        /// las escala al rectángulo delimitador y clasifica qué puntos caen dentro o fuera de la región.
        /// </summary>
        /// <param name="randValues">Lista de números pseudoaleatorios generados previamente.</param>
        /// <returns>Una tupla con dos listas separadas: puntos adentro y puntos afuera.</returns>
        public ( List<Point> inside, List<Point> outside) processPoints ( List<double> randValues)
        {
            List<Point> inside = new List<Point>();
            List<Point> outside = new List<Point>();
            for (int i = 0; i < randValues.Count; i += 2)
            {
                Point p = new Point
                {
                    X = _lowerPoint.X + randValues[i] * (_upperPoint.X - _lowerPoint.X),
                    Y = _lowerPoint.Y + randValues[i + 1] * (_upperPoint.Y - _lowerPoint.Y)
                };
                if (isInside(p))
                {
                    inside.Add(p);
                }
                else
                {
                    outside.Add(p);
                }
            }
            return (inside, outside);
        }


        /// <summary>
        /// Evalúa si una coordenada específica cumple con TODAS las restricciones matemáticas definidas.
        /// </summary>
        private bool isInside( Point p ) { 
            foreach ( var constraint in _constraints)
            {
                if (!constraint(p)) return false;
            }
            return true;
        }
    }
}
