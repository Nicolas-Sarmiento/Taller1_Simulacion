using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Taller1_Simulacion
{
    public partial class Form1 : Form
    {
        long paramA, paramC, paramSeed;
        ulong xorseed;
        int shiftA, shiftB, shiftC;
        LCG lcg20, lcg200, lcg2000, lcg10000, lcg20000;
        
        List<Double> xorvalues20, xorvalues200, xorvalues2000, xorvalues10000, xorvalues20000;
        List<Double> values20, values200, values2000, values10000, values20000;

        DataTable tbLcG20, tbLcG200, tbLcG2000, tbLcG10000, tbLcG20000;
        DataTable tbxor20, tbxor200, tbxor2000, tbxor10000, tbxor20000;


        List<LCG> lgcs;

        xorShift xorGenerator;
        List<long> modulos = new List<long> { 20, 200, 2000, 10000, 20000 };
        List<int> randomQuantity = new List<int> { 20, 200, 2000, 10000, 20000 };


        List<DataTable> tablesLCG;
        List<DataTable> tablesXOR;
        List<List<double>> valuesLCGLists;
        List<List<double>> valuesXORLists;

        List<DataGridView> dataGridsLCG;
        List<DataGridView> dataGridsXOR;

        List<Label> labelsRepeatedLCG;
        List<Label> labelsRepeatedXOR;

        List<Chart> rangedCharts;
        List<Chart> trendLCGCharts;
        List<Chart> trendXORCharts;

        List<Chart> correlationLCGCharts;
        List<Chart> correlationXORCharts;

        List<Label> KSLCGlabels;
        List<Label> KSXORlabels;

        List<Chart> montecarloLCGCharts;
        List<Chart> montecarloXORCharts;

        List<Label> montecarloLCGResultsLabels;
        List<Label> montecarloXORResultsLabels;

        List<double> xValues, fun1, fun2, fun3, squareX, squareY;
        Montecarlo montecarloSim;

        Point maxPoint = new Point { X = 0.865474033101613, Y = 0.679194068181104 };
        Point minPoint = new Point { X = 0.0, Y = 0.0 };

        const double realArea = 0.0737586345871;

        public Form1()
        {
            InitializeComponent();
            InitializeObjects();

        }

        

        private void InitializeObjects() {

            lgcs = new List<LCG> { lcg20, lcg200, lcg2000, lcg10000, lcg20000 };

            tablesLCG = new List<DataTable> { tbLcG20, tbLcG200, tbLcG2000, tbLcG10000, tbLcG20000 };
            tablesXOR = new List<DataTable> { tbxor20, tbxor200, tbxor2000, tbxor10000, tbxor20000 };

            valuesXORLists = new List<List<double>> { xorvalues20, xorvalues200, xorvalues2000, xorvalues10000, xorvalues20000 };
            valuesLCGLists = new List<List<double>> { values20, values200, values2000, values10000, values20000 };

            dataGridsLCG = new List<DataGridView> { gridMod20, gridMod200, gridMod2000, gridMod10000, gridMod20000 };
            dataGridsXOR = new List<DataGridView> { gridXor20, gridXor200, gridXor2000, gridXor10000, gridXor20000 };

            labelsRepeatedLCG = new List<Label> { lblLCGfreqTest20, lblLCGfreqTest200, lblLCGfreqTest2000, lblLCGfreqTest10000, lblLCGfreqTest20000 };
            labelsRepeatedXOR = new List<Label> { lblXORfreqTest20, lblXORfreqTest200, lblXORfreqTest2000, lblXORfreqTest10000, lblXORfreqTest20000 };

            rangedCharts = new List<Chart> { chRanges20, chRanges200, chRanges2000, chRanges10000, chRanges20000 };

            trendLCGCharts = new List<Chart> { chLCGTrendTest20, chLCGTrendTest200, chLCGTrendTest2000, chLCGTrendTest10000, chLCGTrendTest20000 };
            trendXORCharts = new List<Chart> { chXORTrendTest20, chXORTrendTest200, chXORTrendTest2000, chXORTrendTest10000, chXORTrendTest20000 };

            correlationLCGCharts = new List<Chart> { chLCGCorrelation20, chLCGCorrelation200, chLCGCorrelation2000, chLCGCorrelation10000, chLCGCorrelation20000 };
            correlationXORCharts = new List<Chart> { chXORCorrelation20, chXORCorrelation200, chXORCorrelation2000, chXORCorrelation10000, chXORCorrelation20000 };

            KSLCGlabels = new List<Label> { lblLCGKSTest20, lblLCGKSTest200, lblLCGKSTest2000, lblLCGKSTest10000, lblLCGKSTest20000 };
            KSXORlabels = new List<Label> { lblXORKSTest20, lblXORKSTest200, lblXORKSTest2000, lblXORKSTest10000, lblXORKSTest20000 };

            montecarloLCGCharts = new List<Chart> { chMontecarloLGCI10 };
            montecarloXORCharts = new List<Chart> { chMontecarloXORI10 };

            montecarloLCGResultsLabels = new List<Label> { lblLCGResultI10 };
            montecarloXORResultsLabels = new List<Label> { lblXORResultI10 };




            for (int i = 0; i < valuesLCGLists.Count; i++) valuesLCGLists[i] = new List<Double>();
            for (int i = 0; i < valuesXORLists.Count; i++) valuesXORLists[i] = new List<Double>();

            loadRandomVisualizationObjects();
            loadMontecarloVisualizationObjects();

        }




        private void loadMontecarloVisualizationObjects()
        {
            montecarloSim = new Montecarlo(
                new List<Func<Point, bool>> {
                    p => p.Y <= Math.Cos(p.X),
                    p => p.Y <= Math.Pow(p.X, 2),
                    p => p.Y >= Math.Pow(p.X, 3)
                },
                minPoint,
                maxPoint
            );

            loadFunctions();
            for (int i = 0; i < montecarloLCGCharts.Count; i++) {
                montecarloLCGCharts[i].Series["SeriesFun1"].Points.DataBindXY(xValues, fun1);
                montecarloLCGCharts[i].Series["SeriesFun2"].Points.DataBindXY(xValues, fun2);
                montecarloLCGCharts[i].Series["SeriesFun3"].Points.DataBindXY(xValues, fun3);
                montecarloLCGCharts[i].Series["SeriesSquare"].Points.DataBindXY(squareX, squareY);
            }
            for ( int i = 0; i < montecarloXORCharts.Count; i++)
            {
                montecarloXORCharts[i].Series["SeriesFun1"].Points.DataBindXY(xValues, fun1);
                montecarloXORCharts[i].Series["SeriesFun2"].Points.DataBindXY(xValues, fun2);
                montecarloXORCharts[i].Series["SeriesFun3"].Points.DataBindXY(xValues, fun3);
                montecarloXORCharts[i].Series["SeriesSquare"].Points.DataBindXY(squareX, squareY);
            }
        }

        private void loadFunctions()
        {
                xValues = new List<double>();
                fun1 = new List<double>();
                fun2 = new List<double>();
                fun3 = new List<double>();
                squareX = new List<double> { 0, 0.865474033101613, 0.865474033101613, 0, 0 };
                squareY = new List<double> { 0, 0, 0.679194068181104, 0.679194068181104, 0 };
    
                for (double x = 0; x <= 1; x += 0.01)
                {
                    xValues.Add(x);
                    fun1.Add(Math.Pow(x, 2));
                    fun2.Add(Math.Pow(x, 3));
                    fun3.Add(Math.Cos(x));
                    
                }


        }


        private void loadRandomVisualizationObjects() {



            for (int i = 0; i < tablesLCG.Count; i++) {
                tablesLCG[i] = new DataTable();
                tablesLCG[i].Columns.Add("I", typeof(int));
                tablesLCG[i].Columns.Add("Xn", typeof(long));
                tablesLCG[i].Columns.Add("Un", typeof(double));
            }

            for (int i = 0; i < dataGridsLCG.Count; i++)
            {
                dataGridsLCG[i].AutoGenerateColumns = false;
                dataGridsLCG[i].DataSource = tablesLCG[i];
            }

            for (int i = 0; i < tablesXOR.Count; i++)
            {
                tablesXOR[i] = new DataTable();
                tablesXOR[i].Columns.Add("I", typeof(int));
                tablesXOR[i].Columns.Add("Xn", typeof(ulong));
                tablesXOR[i].Columns.Add("Un", typeof(double));
            }

            for (int i = 0; i < dataGridsXOR.Count; i++)
            {
                dataGridsXOR[i].AutoGenerateColumns = false;
                dataGridsXOR[i].DataSource = tablesXOR[i];
            }
        }


        private void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            btnRunMontecarlo.Enabled = false;
            tbcMontecarloIterations.Visible = false;
            if (!validateParams())
            {
                btnRun.Enabled = true;
                return;
            }

            randomSimulation();

            btnRun.Enabled = true;
            btnRunMontecarlo.Enabled = true;
        }

        private void randomSimulation() {
            buildGenerators();
            clearObjects();
            getRandomValues();

            runFrequencyTests();
            runDistributionTests();
            runTrendTests();
            runCorrelationTests();
            runKSTests();
        }

        private void getRandomValues()
        {
            getRandomLCGValues();
            getRandomXorValues();
        }

        private void getRandomLCGValues()
        {
            for (int i = 0; i < valuesLCGLists.Count; i++)
            {
                LCG lcg = lgcs[i];
                List<double> valuesList = valuesLCGLists[i];
                DataTable table = tablesLCG[i];
                for (int j = 0; j < modulos[i]; j++)
                {
                    double un = lcg.Next();
                    long xn = lcg.LastXn;
                    table.Rows.Add(j + 1, xn, un);
                    valuesList.Add(un);
                }
            }
        }

        private void getRandomXorValues() {
            for (int i = 0; i < valuesXORLists.Count; i++)
            {
                for (int j = 0; j < randomQuantity[i]; j++)
                {
                    double un = xorGenerator.Next();
                    valuesXORLists[i].Add(un);
                    tablesXOR[i].Rows.Add(j + 1, xorGenerator.x, un);
                }
            }
        }

        private void clearObjects() {
            foreach (DataTable tb in tablesLCG) tb.Rows.Clear();
            foreach (List<double> lst in valuesLCGLists) lst.Clear();

            foreach (DataTable tb in tablesXOR) tb.Rows.Clear();
            foreach (List<double> lst in valuesXORLists) lst.Clear();
        }

        private void buildLCGs() {
            for (int i = 0; i < lgcs.Count; i++)
            {
                lgcs[i] = new LCG(paramSeed, paramA, paramC, modulos[i]);
            }
        }

        private void buildGenerators()
        {
            buildLCGs();
            xorGenerator = new xorShift(xorseed, shiftA, shiftB, shiftC);

        }


        private bool validateParams() {
            string strParamAInput = txtParamA.Text;
            string strParamCInput = txtParamC.Text;
            string strSeed = txtSeed.Text;

            string strShiftA = txtShiftA.Text;
            string strShiftB = txtShiftB.Text;
            string strShiftC = txtShiftC.Text;
            string strxorseed = txtXorSeed.Text;


            if ((!long.TryParse(strParamAInput, out paramA) || paramA <= 0)) {
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

            if (!ulong.TryParse(strxorseed, out xorseed)) {
                MessageBox.Show("El parámetro semilla de xor shift debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtXorSeed.SelectAll();
                txtXorSeed.Focus();
                return false;
            }

            if (!int.TryParse(strShiftA, out shiftA) || shiftA <= 0)
            {
                MessageBox.Show("El parámetro semilla de shift A debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtShiftA.SelectAll();
                txtShiftA.Focus();
                return false;
            }

            if (!int.TryParse(strShiftB, out shiftB) || shiftB <= 0)
            {
                MessageBox.Show("El parámetro semilla de shift B debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtShiftB.SelectAll();
                txtShiftB.Focus();
                return false;
            }

            if (!int.TryParse(strShiftC, out shiftC) || shiftC <= 0)
            {
                MessageBox.Show("El parámetro semilla de shift C debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtShiftC.SelectAll();
                txtShiftC.Focus();
                return false;
            }

            return true;
        }

        private void runFrequencyTests() {
            for (int i = 0; i < valuesLCGLists.Count; i++)
            {
                FrequencyTest(valuesLCGLists[i], labelsRepeatedLCG[i]);
            }
            for (int i = 0; i < valuesXORLists.Count; i++)
            {
                FrequencyTest(valuesXORLists[i], labelsRepeatedXOR[i]);
            }
        }

        private void runDistributionTests() {

            for (int i = 0; i < rangedCharts.Count; i++)
            {
                graphDistributedRanges(rangedCharts[i], valuesLCGLists[i], 0);
                graphDistributedRanges(rangedCharts[i], valuesXORLists[i], 1);
            }

        }

        private void runTrendTests() {
            for (int i = 0; i < trendLCGCharts.Count; i++)
            {
                graphTrendChart(trendLCGCharts[i], valuesLCGLists[i]);
                graphTrendChart(trendXORCharts[i], valuesXORLists[i]);
            }
        }

        private void runCorrelationTests()
        {
            for ( int i = 0; i < correlationLCGCharts.Count; i++)
            {
                graphCorrelation(correlationLCGCharts[i], valuesLCGLists[i]);
                double corrLCG = calcCorrelation(valuesLCGLists[i]);
                TextAnnotation noteLCG = (TextAnnotation)correlationLCGCharts[i].Annotations["txtAnnLCGCorrelation"];
                if (double.IsNaN(corrLCG))
                {
                    noteLCG.Text = $"Correlación (r): No se pudo calcular (n <= 2)";
                    
                }else
                {
                    noteLCG.Text = $"Correlación (r): {corrLCG:F4}";
                }
                

                graphCorrelation(correlationXORCharts[i], valuesXORLists[i]);
                double corrXOR = calcCorrelation(valuesXORLists[i]);
                TextAnnotation noteXOR = (TextAnnotation)correlationXORCharts[i].Annotations["txtAnnXorCorrelation"];
                if (double.IsNaN(corrXOR))
                {
                    noteXOR.Text = $"Correlación (r): No se pudo calcular (n <= 2)";
                }
                else
                {
                    noteXOR.Text = $"Correlación (r): {corrXOR:F4}";
                }

            }
        }

        private void runKSTests() {
            for (int i = 0; i < KSLCGlabels.Count; i++)
            {
                KSTest(valuesLCGLists[i], KSLCGlabels[i]);
            }
            for (int i = 0; i < KSXORlabels.Count; i++)
            {
                KSTest(valuesXORLists[i], KSXORlabels[i]);
            }
        }


        private void FrequencyTest(List<double> values, Label label)
        {

            HashSet<double> uniqueNumbers = new HashSet<double>(values);
            int repeatedCount = values.Count - uniqueNumbers.Count;

            Dictionary<double, int> frequencyMap = new Dictionary<double, int>();
            List<double> repeatedNumbers = new List<double>();

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
                label.BackColor = Color.LightGreen;
                label.ForeColor = Color.DarkGreen;
                label.Text = $"✅ SIN REPETICIONES: {uniqueNumbers.Count}/{values.Count} números únicos. Periodo completo garantizado.";
            }
            else
            {
                label.BackColor = Color.LightCoral;
                label.ForeColor = Color.DarkRed;
                string repeatedList = string.Join(", ", repeatedNumbers.Take(10));
                if (repeatedNumbers.Count > 10)
                    repeatedList += $", ... (+{repeatedNumbers.Count - 10} más)";
                label.Text = $"⚠️ REPETICIONES DETECTADAS: {repeatedCount} repeticiones. Números: {repeatedList}";
            }
        }

        private void graphDistributedRanges(Chart ch, List<double> lst, int serie)
        {
            int ranges = 10;
            double rangeSize = 1.0 / ranges;

            int[] frequencies = new int[ranges];

            foreach (double Un in lst)
            {
                int rangeIndex = (int)Math.Floor(Un / rangeSize);
                if (rangeIndex >= ranges) rangeIndex = ranges - 1;
                frequencies[rangeIndex]++;
            }
            ch.Series[serie].Points.Clear();
            string[] tags = new string[ranges];
            for (int i = 0; i < ranges; i++)
            {
                string tag = $"{(i * rangeSize):F2} - {((i + 1) * rangeSize):F2}";
                tags[i] = tag;
            }
            ch.Series[serie].Points.DataBindXY(tags, frequencies);
            ch.Series[serie].Points.ResumeUpdates();
        }

        private void graphTrendChart(Chart ch, List<double> lst)
        {
            ch.Series[0].Points.Clear();
            ch.Series[0].Points.DataBindY(lst);
            ch.Series[0].Points.ResumeUpdates();
        }

        private void graphCorrelation(Chart ch, List<double> lst)
        {
            ch.Series[0].Points.Clear();
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            for (int i = 0; i < lst.Count - 1; i+=2)
            {
                xValues.Add(lst[i]);
                yValues.Add(lst[i + 1]);
            }
            ch.Series[0].Points.DataBindXY(xValues, yValues);
            ch.Series[0].Points.ResumeUpdates();
        }

        private double calcCorrelation(List<double> lst) {
            int n = lst.Count - 1;
            if (n <= 2) return double.NaN;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
            for ( int i = 0; i < n; i++)
            {
                double x = lst[i];
                double y = lst[i + 1];
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
                sumY2 += y * y;
            }
            double num = (n * sumXY) - (sumX * sumY);   
            double den = Math.Sqrt(((n * sumX2) - (sumX * sumX)) * ((n * sumY2) - (sumY * sumY)));

            if (den == 0 ) return 0.0;
            return num / den;
        }

        private void KSTest(List<double> lst, Label kslabel)
        {
            int n = lst.Count;
            if (n == 0) return;

            List<double> lstsorted = new List<double>(lst);
            lstsorted.Sort();

            double dPlusMax = 0;
            double dMinusMax = 0;

            for (int i = 0; i < n; i++)
            {
                double i_math = i + 1;
                double Ui = lstsorted[i];

                double dPlus = (i_math / n) - Ui;
                double dMinus = Ui - ((i_math - 1) / n);

                dPlusMax = Math.Max(dPlusMax, dPlus);
                dMinusMax = Math.Max(dMinusMax, dMinus);

            }

            double dCalculated = Math.Max(dPlusMax, dMinusMax);
            double dCritical = 1.36 / Math.Sqrt(n);

            double percent = calcRandomnessPercent(dCalculated, n);

            kslabel.Text = $"D calculado = {dCalculated:F4}\nD crítico = {dCritical:F4}\nPorcentaje de compatibilidad con la distribución uniforme de los números generados = {percent:F2}%";
            if (percent >= 5.0)
            {
                kslabel.ForeColor = Color.DarkGreen;
                kslabel.BackColor = Color.LightGreen;
            
            }
            else { 
                kslabel.ForeColor = Color.DarkRed; 
                kslabel.BackColor = Color.LightCoral;
            }
            
        }
        private double calcRandomnessPercent ( double dCalculated , int n)
        {
            double x = dCalculated * Math.Sqrt(n);
            if (x < 0.27) return 100.0;

            double sum = 0;
            for (int j = 1; j <= 100; j++)
            {
                double term = Math.Pow(-1, j - 1) * Math.Exp(-2 * j * j * x * x);
                sum += term;

                if (Math.Abs(term) < 1e-10) break;
            }

            double p = 2 * sum;
            if (p > 1.0) p = 1.0;
            if (p < 0.0) p = 0.0;

            return p * 100.0;
        }

        private void btnRunMontecarlo_Click(object sender, EventArgs e)
        {
            btnRunMontecarlo.Enabled = false;

            for (int i = 0; i < valuesLCGLists.Count; i++) { 
                
                runMontecarlo(valuesLCGLists[i], montecarloLCGCharts[0], montecarloLCGResultsLabels[0]);

            }

            for (int i = 0; i < valuesXORLists.Count; i++) { 
                
                runMontecarlo(valuesXORLists[i], montecarloXORCharts[0], montecarloXORResultsLabels[0]);

            }



            tbcMontecarloIterations.Visible = true;
            btnRunMontecarlo.Enabled = true;
        }

        private void runMontecarlo(List<double> values, Chart ch, Label lbl )
        {
            // Shuffle values to ensure randomness in point selection
            var pointsList = montecarloSim.processPoints(values);
            int totalPoints = pointsList.inside.Count + pointsList.outside.Count;
            int inside = pointsList.inside.Count;
            double estimatedArea = montecarloSim.EstimateArea(totalPoints, inside );
            // Update label results
            lbl.Text = $"Puntos dentro: {inside}\nPuntos totales: {totalPoints}\nÁrea estimada: {estimatedArea:F4}";
            ch.Series["SeriesInsidePoints"].Points.Clear();
            ch.Series["SeriesOutsidePoints"].Points.Clear();
            ch.Series["SeriesInsidePoints"].Points.DataBind(pointsList.inside, "X", "Y", "");
            ch.Series["SeriesOutsidePoints"].Points.DataBind(pointsList.outside, "X", "Y", "");

        }

        private double calcRealErrror( double result )
        {
            return 0.0;
        }

        private double calcTheoricalError( int usedPoints )
        {
            return 0.0;
        }
    }
}
