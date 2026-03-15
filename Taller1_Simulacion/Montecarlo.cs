using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    internal class Montecarlo
    {
        private IRandomGenerator _rgn;
        private List<Func<Point,bool>> _constraints;
        private Point _lowerPoint, _upperPoint;

        public Montecarlo(IRandomGenerator rng, List<Func<Point, bool>> constraint_functions, Point lowerPoint, Point upperPoint) { 
            _rgn = rng;
            _constraints = constraint_functions;
            _lowerPoint = lowerPoint;
            _upperPoint = upperPoint;
        }
        public double EstimateArea( int iterations ) { 
            int inside_count = 0;
            for (int i = 0; i < iterations; i++)
            {
                var p = new Point(
                    _lowerPoint.X + _rgn.Next() * (_upperPoint.X - _lowerPoint.X),
                    _lowerPoint.Y + _rgn.Next() * (_upperPoint.Y - _lowerPoint.Y)
                );
                if (isInside(p)) inside_count++;
            }
            double total_area = (_upperPoint.X - _lowerPoint.X) * (_upperPoint.Y - _lowerPoint.Y);
            return ((double)inside_count / iterations) * total_area;
        }
        private bool isInside( Point p ) { 
            foreach ( var constraint in _constraints)
            {
                if (!constraint(p)) return false;
            }
            return true;
        }
    }
}
