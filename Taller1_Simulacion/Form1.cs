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
        private DataGridView[] dataGrids;
        private System.Windows.Forms.DataVisualization.Charting.Chart[] charts;
        private readonly int[] modulos = { 10, 100, 1000, 5000, 10000 };

        public Form1()
        {
            InitializeComponent();
            // Inicializar DataGridViews y Charts para cada tabPage
            dataGrids = new DataGridView[5];
            charts = new System.Windows.Forms.DataVisualization.Charting.Chart[5];
            for (int i = 0; i < 5; i++)
            {
                dataGrids[i] = new DataGridView
                {
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    Size = new System.Drawing.Size(313, 454),
                    Location = new System.Drawing.Point(12, 6),
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
                };
                charts[i] = new System.Windows.Forms.DataVisualization.Charting.Chart
                {
                    Size = new System.Drawing.Size(559, 183),
                    Location = new System.Drawing.Point(577, 22)
                };
                charts[i].ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("ChartArea1"));
                charts[i].Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend("Legend1"));
            }
            tabPage3.Controls.Add(dataGrids[0]);
            tabPage3.Controls.Add(charts[0]);
            tabPage4.Controls.Add(dataGrids[1]);
            tabPage4.Controls.Add(charts[1]);
            tabPage5.Controls.Add(dataGrids[2]);
            tabPage5.Controls.Add(charts[2]);
            tabPage6.Controls.Add(dataGrids[3]);
            tabPage6.Controls.Add(charts[3]);
            tabPage7.Controls.Add(dataGrids[4]);
            tabPage7.Controls.Add(charts[4]);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            for (int idx = 0; idx < modulos.Length; idx++)
            {
                int modulo = modulos[idx];
                var grid = dataGrids[idx];
                var chart = charts[idx];
                grid.Rows.Clear();
                grid.Columns.Clear();
                chart.Series.Clear();
                chart.Titles.Clear();

                grid.Columns.Add("Iteracion", "Iteración");
                grid.Columns.Add("Xn", "Xn");
                grid.Columns.Add("Un", "Un");

                LCG lcg = new LCG(54321, 1103515245, 1, modulo);
                List<double> valoresUn = new List<double>();
                List<long> valoresXn = new List<long>();
                for (int i = 0; i < modulo; i++)
                {
                    double un = lcg.Next();
                    long xn = lcg.LastXn;
                    grid.Rows.Add(i + 1, xn, un);
                    valoresUn.Add(un);
                    valoresXn.Add(xn);
                }

                var serie = new System.Windows.Forms.DataVisualization.Charting.Series("Distribución módulo " + modulo);
                serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                int bins = 10;
                int[] frecuencias = new int[bins];
                foreach (var un in valoresUn)
                {
                    int bin = (int)(un * bins);
                    if (bin == bins) bin = bins - 1;
                    frecuencias[bin]++;
                }
                for (int j = 0; j < bins; j++)
                {
                    serie.Points.AddXY($"Rango {j + 1}", frecuencias[j]);
                }
                chart.Series.Add(serie);
                chart.ChartAreas[0].AxisX.Title = "Rangos";
                chart.ChartAreas[0].AxisY.Title = "Frecuencia";
                chart.Titles.Add("Distribución para módulo " + modulo);
            }
        }

    }
}
