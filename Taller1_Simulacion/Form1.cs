using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Taller1_Simulacion
{
    public partial class Form1 : Form
    {
        long paramA, paramC, paramSeed;
        LCG lcg20, lcg200, lcg2000, lcg10000, lcg20000;
        // xor ones
        List<Double> values20, values200, values2000, values10000, values20000;
        DataTable tbLcG20, tbLcG200, tbLcG2000, tbLcG10000, tbLcG20000;

        List<LCG> lgcs;

        List<long> modulos = new List<long> { 20, 200, 2000, 10000, 20000 };


        List<DataTable> tables;
        List<List<double>> valuesLists;
        List<DataGridView> dataGrids;

        public Form1()
        {
            InitializeComponent();
            InitializeObjects();
            
        }

        private void InitializeObjects () {
            
            lgcs= new List<LCG> { lcg20, lcg200, lcg2000, lcg10000, lcg20000 };
            tables = new List<DataTable> { tbLcG20, tbLcG200, tbLcG2000, tbLcG10000, tbLcG20000 };
            valuesLists = new List<List<double>> { values20, values200, values2000, values10000, values20000 };
            dataGrids = new List<DataGridView> { gridMod20 };

            for (int i = 0; i < valuesLists.Count; i++) valuesLists[i] = new List<Double>();

            loadRandomVisualizationObjects();
            loadMontecarloVisualizationObjects();

        }




        private void loadMontecarloVisualizationObjects()
        {

        }
        private void loadRandomVisualizationObjects() {

           

            for (int i = 0; i < tables.Count; i++) { 
                tables[i] = new DataTable();
                tables[i].Columns.Add("I", typeof(int));
                tables[i].Columns.Add("Xn", typeof(long));
                tables[i].Columns.Add("Un", typeof(double));
            }

            for (int i = 0; i < dataGrids.Count; i++)
            {
                dataGrids[i].AutoGenerateColumns = false;
                dataGrids[i].DataSource = tables[i];
            }




        }


        private void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            if (!validateParams())
            {
                btnRun.Enabled = true;
                return;
            }

            randomSimulation();

            // pruebas





            btnRun.Enabled = true;
        }

        private void randomSimulation() {
            buildLCGs();
            clearObjects();
            getRandomValues();

        }


        private void getRandomValues()
        {
            for ( int i = 0; i < 1; i++)
            {
                LCG lcg = lgcs[i];
                List<double> valuesList = valuesLists[i];
                DataTable table = tables[i];
                for (int j = 0; j < modulos[i]; j++)
                {
                    double un = lcg.Next();
                    long xn = lcg.LastXn;   
                    table.Rows.Add(j + 1, xn, un);
                    Console.WriteLine($"LCG {modulos[i]} - Iteración {j + 1}: Xn={xn}, Un={un}");
                    valuesList.Add(un);
                }
                
            }



        }
        private void clearObjects () {
           foreach ( DataTable tb in tables) tb.Rows.Clear();
           foreach (List<double> lst in valuesLists) lst.Clear();
        }

        private void buildLCGs() {
            for (int i = 0; i < lgcs.Count; i++)
            {
                lgcs[i] = new LCG(paramSeed, paramA, paramC, modulos[i]);
            }
        }


        private bool validateParams() {
            string strParamAInput = txtParamA.Text;
            string strParamCInput = txtParamC.Text;
            string strSeed = txtSeed.Text;

            if ((!long.TryParse(strParamAInput, out paramA) || paramA <= 0)){
                MessageBox.Show("El parámetro a debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtParamA.SelectAll();
                txtParamA.Focus();
                return false;
            }

            if ((!long.TryParse(strParamCInput, out paramC) || paramC <= 0))
            {
                MessageBox.Show("El parámetro c debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtParamC.SelectAll();
                txtParamC.Focus();
                return false;
            }

            if ((!long.TryParse(strSeed, out paramSeed) || paramSeed <= 0))
            {
                MessageBox.Show("El parámetro semilla debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSeed.SelectAll();
                txtSeed.Focus();
                return false;
            }


            return true;
        }

        private void FrequencyTest(List<long> values, Label label )
        {
            
            HashSet<long> uniqueNumbers = new HashSet<long>(values);
            int repeatedCount = values.Count - uniqueNumbers.Count;

            Dictionary<long, int> frequencyMap = new Dictionary<long, int>();
            List<long> repeatedNumbers = new List<long>();

            foreach (var num in values)
            {
                if (!frequencyMap.ContainsKey(num))
                    frequencyMap[num] = 0;
                frequencyMap[num]++;
            }

            foreach (var kvp in frequencyMap.Where(x => x.Value > 1))
            {
                repeatedNumbers.Add(kvp.Key);
            }

            if (repeatedCount == 0)
            {
                label.BackColor = System.Drawing.Color.LightGreen;
                label.ForeColor = System.Drawing.Color.DarkGreen;
                label.Text = $"✅ SIN REPETICIONES: {uniqueNumbers.Count}/{values.Count} números únicos. Periodo completo garantizado.";
            }
            else
            {
                label.BackColor = System.Drawing.Color.LightCoral;
                label.ForeColor = System.Drawing.Color.DarkRed;
                string repeatedList = string.Join(", ", repeatedNumbers.Take(10));
                if (repeatedNumbers.Count > 10)
                    repeatedList += $", ... (+{repeatedNumbers.Count - 10} más)";
                label.Text = $"⚠️ REPETICIONES DETECTADAS: {repeatedCount} repeticiones. Números: {repeatedList}";
            }
        }


        //private void InitializeMonteCarloTab()
        //{
        //    // Crear DataGridView para resultados MonteCarlo
        //    dgvMonteCarlo = new DataGridView
        //    {
        //        AllowUserToAddRows = false,
        //        AllowUserToDeleteRows = false,
        //        ReadOnly = true,
        //        Size = new System.Drawing.Size(1170, 150),
        //        Location = new System.Drawing.Point(10, 10),
        //        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        //        Name = "dgvMonteCarlo"
        //    };

        //    dgvMonteCarlo.Columns.Add("Puntos", "Cantidad de Puntos");
        //    dgvMonteCarlo.Columns.Add("AreaReal", "Área Conocida");
        //    dgvMonteCarlo.Columns.Add("AreaEstimada", "Área Estimada");
        //    dgvMonteCarlo.Columns.Add("ErrorReal", "Error Real");
        //    dgvMonteCarlo.Columns.Add("ErrorPorcentaje", "Error %");
        //    dgvMonteCarlo.Columns.Add("PuntosAdentro", "Puntos Dentro");
        //    dgvMonteCarlo.Columns.Add("PuntosTotales", "Puntos Totales");

        //    tabPage2.Controls.Add(dgvMonteCarlo);

        //    // Crear gráficos scatter para MonteCarlo
        //    chartsScatter = new System.Windows.Forms.DataVisualization.Charting.Chart[5];
        //    int[] xPositions = { 10, 340, 670, 1000, 300 };
        //    int[] yPositions = { 170, 170, 170, 170, 350 };

        //    for (int i = 0; i < 5; i++)
        //    {
        //        chartsScatter[i] = new System.Windows.Forms.DataVisualization.Charting.Chart
        //        {
        //            Size = new System.Drawing.Size(320, 150),
        //            Location = new System.Drawing.Point(xPositions[i], yPositions[i]),
        //            Name = $"scatterChart{i}"
        //        };

        //        chartsScatter[i].ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("ChartArea"));
        //        chartsScatter[i].Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend("Legend"));
        //        tabPage2.Controls.Add(chartsScatter[i]);
        //    }
        //}

        //private void InitializeCharts()
        //{
        //    dataGrids = new DataGridView[5];
        //    chartsFreq = new System.Windows.Forms.DataVisualization.Charting.Chart[5];
        //    chartsTend = new System.Windows.Forms.DataVisualization.Charting.Chart[5];
        //    labelsCorr = new System.Windows.Forms.Label[5];
        //    labelsKS = new System.Windows.Forms.Label[5];
        //    labelsRepeated = new System.Windows.Forms.Label[5];

        //    TabPage[] tabs = { tabPage3, tabPage4, tabPage5, tabPage6, tabPage7 };

        //    for (int i = 0; i < 5; i++)
        //    {
        //        // DataGridView
        //        dataGrids[i] = new DataGridView
        //        {
        //            AllowUserToAddRows = false,
        //            AllowUserToDeleteRows = false,
        //            ReadOnly = true,
        //            Size = new System.Drawing.Size(280, 200),
        //            Location = new System.Drawing.Point(10, 10),
        //            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        //            Name = $"dataGrid{i}"
        //        };

        //        // Chart Frecuencia
        //        chartsFreq[i] = new System.Windows.Forms.DataVisualization.Charting.Chart
        //        {
        //            Size = new System.Drawing.Size(350, 200),
        //            Location = new System.Drawing.Point(300, 10),
        //            Name = $"chartFreq{i}"
        //        };
        //        chartsFreq[i].ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("ChartArea"));
        //        chartsFreq[i].Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend("Legend"));

        //        // Chart Tendencia
        //        chartsTend[i] = new System.Windows.Forms.DataVisualization.Charting.Chart
        //        {
        //            Size = new System.Drawing.Size(350, 200),
        //            Location = new System.Drawing.Point(660, 10),
        //            Name = $"chartTend{i}"
        //        };
        //        chartsTend[i].ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("ChartArea"));
        //        chartsTend[i].Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend("Legend"));

        //        // Label Correlación
        //        labelsCorr[i] = new System.Windows.Forms.Label
        //        {
        //            Size = new System.Drawing.Size(350, 30),
        //            Location = new System.Drawing.Point(300, 220),
        //            Name = $"labelCorr{i}",
        //            Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
        //            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        //        };

        //        // Label K-S
        //        labelsKS[i] = new System.Windows.Forms.Label
        //        {
        //            Size = new System.Drawing.Size(350, 30),
        //            Location = new System.Drawing.Point(660, 220),
        //            Name = $"labelKS{i}",
        //            Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
        //            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        //        };

        //        // Label Números Repetidos
        //        labelsRepeated[i] = new System.Windows.Forms.Label
        //        {
        //            Size = new System.Drawing.Size(990, 25),
        //            Location = new System.Drawing.Point(10, 265),
        //            Name = $"labelRepeated{i}",
        //            Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold),
        //            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
        //            BackColor = System.Drawing.Color.LightGreen,
        //            ForeColor = System.Drawing.Color.DarkGreen
        //        };

        //        // Agregar controles a la pestaña
        //        tabs[i].Controls.Add(dataGrids[i]);
        //        tabs[i].Controls.Add(chartsFreq[i]);
        //        tabs[i].Controls.Add(chartsTend[i]);
        //        tabs[i].Controls.Add(labelsCorr[i]);
        //        tabs[i].Controls.Add(labelsKS[i]);
        //        tabs[i].Controls.Add(labelsRepeated[i]);
        //    }
        //}
        //private void GenerateRandomNumbers()
        //{
        //    for (int idx = 0; idx < modulos.Length; idx++)
        //    {
        //        int modulo = modulos[idx];
        //        var grid = dataGrids[idx];
        //        var chartFreq = chartsFreq[idx];
        //        var chartTend = chartsTend[idx];
        //        var labelCorr = labelsCorr[idx];
        //        var labelKS = labelsKS[idx];
        //        var labelRep = labelsRepeated[idx];

        //        // Limpiar
        //        grid.Rows.Clear();
        //        grid.Columns.Clear();
        //        chartFreq.Series.Clear();
        //        chartFreq.Titles.Clear();
        //        chartTend.Series.Clear();
        //        chartTend.Titles.Clear();

        //        // Configurar columnas DataGridView
        //        grid.Columns.Add("Iteracion", "Iteración");
        //        grid.Columns.Add("Xn", "Xn");
        //        grid.Columns.Add("Un", "Un");

        //        // Generar números aleatorios
        //        LCG lcg = new LCG(paramSeed, paramA, paramC, modulo);
        //        List<double> valoresUn = new List<double>();
        //        List<long> valoresXn = new List<long>();

        //        for (int i = 0; i < modulo; i++)
        //        {
        //            double un = lcg.Next();
        //            long xn = lcg.LastXn;
        //            grid.Rows.Add(i + 1, xn, un.ToString("F6"));
        //            valoresUn.Add(un);
        //            valoresXn.Add(xn);
        //        }

        //        // 1. Gráfico Frecuencia (Distribución por rangos)
        //        CreateFrequencyChart(chartFreq, valoresUn, modulo);

        //        // 2. Gráfico Tendencia
        //        CreateTrendChart(chartTend, valoresUn, modulo);

        //        // 3. Correlación
        //        double correlacion = CalculateCorrelation(valoresUn);
        //        labelCorr.Text = $"Correlación (Un, Un+1): {correlacion:F6}";

        //        // 4. Kolmogorov-Smirnov
        //        double dMax = CalculateKolmogorovSmirnov(valoresUn);
        //        labelKS.Text = $"K-S Dmax: {dMax:F6}";

        //        // 5. Detectar números repetidos
        //        DetectRepeatedNumbers(valoresXn, labelRep, modulo);
        //    }
        //}

        //private void ExecuteMonteCarlo()
        //{
        //    LCG lcgBig = new LCG(paramSeed, paramA, paramC, paramM);

        //    dgvMonteCarlo.Rows.Clear();

        //    for (int idx = 0; idx < modulos.Length; idx++)
        //    {
        //        int numPoints = modulos[idx];

        //        // Definir condiciones del área (funciones del problema)
        //        var conditions = new List<Func<Point, bool>>
        //        {
        //            (p) => p.Y <= Math.Pow(p.X, 2),  // f(x) = x^2
        //            (p) => p.Y <= Math.Cos(p.X),     // h(x) = cos(x)
        //            (p) => p.Y >= Math.Pow(p.X, 3)   // g(x) = x^3
        //        };

        //        // Límites del rectángulo de búsqueda
        //        Point lower = new Point(0, 0);
        //        Point upper = new Point(0.865474033101613, 0.679194068181104);

        //        // Ejecutar MonteCarlo
        //        var mc = new Montecarlo(lcgBig, conditions, lower, upper);
        //        List<Point> pointsInside = new List<Point>();
        //        List<Point> pointsOutside = new List<Point>();

        //        int insideCount = 0;
        //        for (int i = 0; i < numPoints; i++)
        //        {
        //            var p = new Point(
        //                lower.X + lcgBig.Next() * (upper.X - lower.X),
        //                lower.Y + lcgBig.Next() * (upper.Y - lower.Y)
        //            );

        //            bool inside = true;
        //            foreach (var condition in conditions)
        //            {
        //                if (!condition(p)) { inside = false; break; }
        //            }

        //            if (inside)
        //            {
        //                insideCount++;
        //                pointsInside.Add(p);
        //            }
        //            else
        //            {
        //                pointsOutside.Add(p);
        //            }
        //        }

        //        // Calcular área estimada
        //        double rectangleArea = (upper.X - lower.X) * (upper.Y - lower.Y);
        //        double estimatedArea = ((double)insideCount / numPoints) * rectangleArea;

        //        // Calcular errores
        //        double realError = Math.Abs(AREA_REAL - estimatedArea);
        //        double errorPercentage = (realError / AREA_REAL) * 100;

        //        // Agregar fila a tabla
        //        dgvMonteCarlo.Rows.Add(
        //            numPoints,
        //            AREA_REAL.ToString("F10"),
        //            estimatedArea.ToString("F10"),
        //            realError.ToString("F10"),
        //            errorPercentage.ToString("F4"),
        //            insideCount,
        //            numPoints
        //        );

        //        // Crear gráfico scatter
        //        CreateScatterChart(chartsScatter[idx], pointsInside, pointsOutside, numPoints);
        //    }
        //}

        //private void CreateScatterChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, 
        //    List<Point> inside, List<Point> outside, int numPoints)
        //{
        //    chart.Series.Clear();
        //    chart.Titles.Clear();

        //    // Series para puntos dentro
        //    var seriesInside = new System.Windows.Forms.DataVisualization.Charting.Series("Dentro")
        //    {
        //        ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point
        //    };
        //    seriesInside.Color = Color.Red;

        //    foreach (var p in inside)
        //    {
        //        seriesInside.Points.AddXY(p.X, p.Y);
        //    }

        //    // Series para puntos fuera
        //    var seriesOutside = new System.Windows.Forms.DataVisualization.Charting.Series("Fuera")
        //    {
        //        ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point
        //    };
        //    seriesOutside.Color = Color.Blue;

        //    foreach (var p in outside)
        //    {
        //        seriesOutside.Points.AddXY(p.X, p.Y);
        //    }

        //    chart.Series.Add(seriesInside);
        //    chart.Series.Add(seriesOutside);
        //    chart.ChartAreas[0].AxisX.Title = "X";
        //    chart.ChartAreas[0].AxisY.Title = "Y";
        //    chart.Titles.Add($"MonteCarlo ({numPoints} puntos)");
        //}

        //private void CreateFrequencyChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, List<double> values, int modulo)
        //{
        //    var serie = new System.Windows.Forms.DataVisualization.Charting.Series("Frecuencia")
        //    {
        //        ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column
        //    };

        //    int bins = 10;
        //    int[] frecuencias = new int[bins];
        //    foreach (var val in values)
        //    {
        //        int bin = (int)(val * bins);
        //        if (bin == bins) bin = bins - 1;
        //        if (bin >= 0) frecuencias[bin]++;
        //    }

        //    for (int j = 0; j < bins; j++)
        //    {
        //        serie.Points.AddXY($"R{j + 1}", frecuencias[j]);
        //    }

        //    chart.Series.Add(serie);
        //    chart.ChartAreas[0].AxisX.Title = "Rangos";
        //    chart.ChartAreas[0].AxisY.Title = "Frecuencia";
        //    chart.Titles.Add($"Frecuencia (módulo {modulo})");
        //}

        //private void CreateTrendChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, List<double> values, int modulo)
        //{
        //    var serie = new System.Windows.Forms.DataVisualization.Charting.Series("Tendencia")
        //    {
        //        ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        //    };

        //    for (int i = 0; i < values.Count; i++)
        //    {
        //        serie.Points.AddXY(i + 1, values[i]);
        //    }

        //    chart.Series.Add(serie);
        //    chart.ChartAreas[0].AxisX.Title = "Iteración";
        //    chart.ChartAreas[0].AxisY.Title = "Un";
        //    chart.Titles.Add($"Tendencia (módulo {modulo})");
        //}

        //private double CalculateCorrelation(List<double> values)
        //{
        //    if (values.Count < 2) return 0;

        //    double mean = values.Average();
        //    double sumProduct = 0, sumSq1 = 0, sumSq2 = 0;

        //    for (int i = 0; i < values.Count - 1; i++)
        //    {
        //        double diff1 = values[i] - mean;
        //        double diff2 = values[i + 1] - mean;
        //        sumProduct += diff1 * diff2;
        //        sumSq1 += diff1 * diff1;
        //        sumSq2 += diff2 * diff2;
        //    }

        //    if (sumSq1 == 0 || sumSq2 == 0) return 0;
        //    return sumProduct / Math.Sqrt(sumSq1 * sumSq2);
        //}

        //private double CalculateKolmogorovSmirnov(List<double> values)
        //{
        //    if (values.Count == 0) return 0;

        //    var sorted = values.OrderBy(x => x).ToList();
        //    double dMax = 0;

        //    for (int i = 0; i < sorted.Count; i++)
        //    {
        //        double empiricalCdf = (i + 1) / (double)sorted.Count;
        //        double diff = Math.Abs(sorted[i] - empiricalCdf);
        //        if (diff > dMax) dMax = diff;
        //    }

        //    return dMax;
        //}




    }
}
