using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Taller1_Simulacion
{
    public partial class Form1 : Form
    {
        IRandomGenerator myRng = new LCG(12345, 1664525, 1013904223, int.MaxValue);
        // LCG rgn1 = new LCG(12345, 1664525, 1013904223, int.MaxValue); -> como crear un nuevo generador de numeros aleatorios con diferentes parametros
        LCG rgn10, rgn100;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {


            /*
             * TESTING CODE... THIS CODE IS JUST TO TEST THE MONTECARLO CLASS, IT CAN BE REMOVED OR COMMENTED OUT
             */

            //    var conditions = new List<Func<Point, bool>>
            //    {
            //        (p) => p.Y <= Math.Pow(p.X,2),  
            //        (p) => p.Y <= Math.Cos(p.X), 
            //        (p) => p.Y >= Math.Pow(p.X, 3)                  
            //    };


            
            //    Montecarlo mc = new Montecarlo(myRng, conditions, new Point(0, 0), new Point(0.865474033101613, 0.679194068181104));

            //    int iterations = 10_000_000;
            //    double estimatedArea = mc.EstimateArea(iterations);
            //    Console.WriteLine($"Estimated Area: {estimatedArea}");


            //    lblResult.Text = $"Estimated Area: {estimatedArea:F6}";

            rgn10 = new LCG(54321, 1103515245, 1, 10);

            // Limpiar Datagrids
            // Limpiar charts, las series.
            // Crear las columnas ( iteracion, valor generado, número aleatorio generado)

            for ( int i = 0; i < 10; i++)
            {
                // iteracion = i
                // valor_generado = rgn10.Next() -> esto devuelve un numero en [0,1], multiplicar por (mod - 1) 
                // 
                // insertar fila ( i, valor_generado * (9), valor_generado )
            }

            // Para las pruebas: 
            // 1. Frecuencia ( grafico )
            // 2. Distribución por rangos ( grafico ) 
            // 3. Tendencia ( grafico linea) 
            // 4. Correlación un cálculo ( label ) 
            // 5. Kolmogorov-Smirnov ( label )
            //&:h

        }

    }
}
