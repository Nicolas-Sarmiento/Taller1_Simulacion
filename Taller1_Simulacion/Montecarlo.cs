using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taller1_Simulacion
{
    internal class Montecarlo
    {
        
        private List<Func<Point,bool>> _constraints;
        private Point _lowerPoint, _upperPoint;

        public Montecarlo( List<Func<Point, bool>> constraint_functions, Point lowerPoint, Point upperPoint) { 
           
            _constraints = constraint_functions;
            _lowerPoint = lowerPoint;
            _upperPoint = upperPoint;
        }
        public double EstimateArea( int totalPoints, int insidePoints ) { 
            double total_area = (_upperPoint.X - _lowerPoint.X) * (_upperPoint.Y - _lowerPoint.Y);
            return ((double)insidePoints / totalPoints) * total_area;
        }

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


        private bool isInside( Point p ) { 
            foreach ( var constraint in _constraints)
            {
                if (!constraint(p)) return false;
            }
            return true;
        }
    }
}
