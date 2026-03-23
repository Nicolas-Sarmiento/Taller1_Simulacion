using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Taller1_Simulacion
{
    /// <summary>
    /// Formulario principal de la aplicación.
    /// Genera los números pseudoaleatorios y ejecuta las pruebas estadísticas.
    /// Autores: Nicolas Sarmiento y Edwar Ramirez.
    /// TLDR: Profe, intentamos lo mejor, sabemos que no es la mejor interfaz gráfica, pero intentamos lo
    /// mejor :D Tenganos piedad a la hora de calificar. Compensamos la falta de creatividad en la interfaz con
    /// otros generadores y un documento más riguroso. 😁 (una notica extra por el esfuerzo plis)
    /// </summary>
    public partial class Form1 : Form
    {
        // Parámetros LCG
        long paramA, paramC, paramSeed;
        // Parámetros XorShift
        ulong xorseed;
        int shiftA, shiftB, shiftC;

        // Generadores e instancias
        LCG lcg20, lcg200, lcg2000, lcg10000, lcg20000;
        xorShift xorGenerator;
        List<LCG> lgcs;

        // Estructuras de datos para almacenar resultados de los generadores
        List<double> xorvalues20, xorvalues200, xorvalues2000, xorvalues10000, xorvalues20000;
        List<double> values20, values200, values2000, values10000, values20000;
        DataTable tbLcG20, tbLcG200, tbLcG2000, tbLcG10000, tbLcG20000;
        DataTable tbxor20, tbxor200, tbxor2000, tbxor10000, tbxor20000;

        // Configuración de las iteraciones y módulos para las pruebas
        List<long> modulos = new List<long> { 20, 200, 2000, 10000, 20000 };
        List<int> randomQuantity = new List<int> { 20, 200, 2000, 10000, 20000 };
        List<int> pointsIterations = new List<int> { 10, 100, 1000, 5000, 10000 };

        // Variables de almacenamiento temporal para resultados de pruebas estadísticas
        List<double> lcgRepeated = new List<double>();
        List<double> xorRepeated = new List<double>();
        List<double> lcgCorrelations = new List<double>();
        List<double> xorCorrelations = new List<double>();
        List<double> lcgKSD = new List<double>();
        List<double> xorKSD = new List<double>();
        List<double> lcgKSPercents = new List<double>();
        List<double> xorKSPercents = new List<double>();

        // Listas agrupadotas para facilitar la iteración de la UI
        List<DataTable> tablesLCG;
        List<DataTable> tablesXOR;
        List<List<double>> valuesLCGLists;
        List<List<double>> valuesXORLists;
        List<DataGridView> dataGridsLCG;
        List<DataGridView> dataGridsXOR;
        List<Label> labelsRepeatedLCG, labelsRepeatedXOR;
        List<Chart> rangedCharts, trendLCGCharts, trendXORCharts;
        List<Chart> correlationLCGCharts, correlationXORCharts;
        List<Label> KSLCGlabels, KSXORlabels;
        List<Chart> montecarloLCGCharts, montecarloXORCharts;
        List<Label> montecarloLCGResultsLabels, montecarloXORResultsLabels;

        // Datos para las gráficas de Montecarlo
        List<double> xValues, fun1, fun2, fun3, squareX, squareY;
        
        // Clases de utilidad
        Montecarlo montecarloSim;
        ShuffleFisherYates shuffler;
        IRandomGenerator rgnShuffler;

        // Puntos delimitadores para el rectángulo de Montecarlo
        Point maxPoint = new Point { X = 0.865474033101613, Y = 0.679194068181104 };
        Point minPoint = new Point { X = 0.0, Y = 0.0 };

        // Constante del área real calculada analíticamente para comparar errores
        const double REALAREA = 0.0737586345871;

        double absError, relError, theoricalError;
        double estimatedArea;

        public Form1()
        {
            InitializeComponent();
            InitializeObjects();
        }

        /// <summary>
        /// Agrupa e inicializa todos los controles visuales (Grids, Gráficas, Labels) 
        /// en listas para poder iterarlos fácilmente mediante bucles 'for'.
        /// </summary>
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

            montecarloLCGCharts = new List<Chart> { chMontecarloLGCI10, chMontecarloLGCI100, chMontecarloLGCI1000, chMontecarloLGCI5000, chMontecarloLGCI10000 };
            montecarloXORCharts = new List<Chart> { chMontecarloXORI10, chMontecarloXORI100, chMontecarloXORI1000, chMontecarloXORI5000, chMontecarloXORI10000 };

            montecarloLCGResultsLabels = new List<Label> { lblLCGResultI10, lblLCGResultI100, lblLCGResultI1000, lblLCGResultI5000, lblLCGResultI10000 };
            montecarloXORResultsLabels = new List<Label> { lblXORResultI10, lblXORResultI100, lblXORResultI1000, lblXORResultI5000, lblXORResultI10000 };

            for (int i = 0; i < valuesLCGLists.Count; i++) valuesLCGLists[i] = new List<Double>();
            for (int i = 0; i < valuesXORLists.Count; i++) valuesXORLists[i] = new List<Double>();

            loadRandomVisualizationObjects();
            loadMontecarloVisualizationObjects();
        }

        /// <summary>
        /// Inicializa el motor de Montecarlo con las tres funciones matemáticas 
        /// que delimitan el área de intersección y prepara las series de los gráficos.
        /// </summary>
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

            rgnShuffler = new xorShiftPlus( 23, 17, 26 );
            shuffler = new ShuffleFisherYates(rgnShuffler);

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

        /// <summary>
        /// Precalcula los puntos X e Y de las funciones matemáticas (Cos(x), x^2, x^3) 
        /// para dibujar el contorno del área en las gráficas de Montecarlo.
        /// </summary>
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

        /// <summary>
        /// Configura las columnas de los DataTables en memoria para recibir los datos de 
        /// los generadores aleatorios (Iteración, ValorCrudo, ValorUniforme).
        /// </summary>
        private void loadRandomVisualizationObjects() {
            for (int i = 0; i < tablesLCG.Count; i++) {
                tablesLCG[i] = new DataTable();
                tablesLCG[i].Columns.Add("I", typeof(int));
                tablesLCG[i].Columns.Add("Xn", typeof(long));
                tablesLCG[i].Columns.Add("Un", typeof(double));
            }

            for (int i = 0; i < tablesXOR.Count; i++)
            {
                tablesXOR[i] = new DataTable();
                tablesXOR[i].Columns.Add("I", typeof(int));
                tablesXOR[i].Columns.Add("Xn", typeof(ulong));
                tablesXOR[i].Columns.Add("Un", typeof(double));
            }
        }

        /// <summary>
        /// Evento disparado al solicitar la generación de números aleatorios.
        /// Ejecuta los cálculos asíncronos y pinta las pruebas estadísticas.
        /// </summary>
        private async void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            btnRunMontecarlo.Enabled = false;
            loadingIcon.Visible = true;
            tbcMontecarloIterations.Visible = false;
            
            if (!validateParams())
            {
                btnRun.Enabled = true;
                return;
            }

            unBindDataGrids(); // Desconecta la UI para mayor rendimiento
            
            // Ejecuta el peso matemático en un hilo secundario
            await Task.Run(() => { randomSimulation(); });
            
            loadRandomDataGrids(); // Reconecta la UI
            runRandomTests();
            setSimulationResults();

            btnRun.Enabled = true;
            btnRunMontecarlo.Enabled = true;
            loadingIcon.Visible = false;
        }

        private void setSimulationResults(){
            gridRandomResults.Rows.Clear();
            for (int i = 0; i < randomQuantity.Count; i++)
            {
                gridRandomResults.Rows.Add(randomQuantity[i], lcgRepeated[i], xorRepeated[i], lcgCorrelations[i], xorCorrelations[i], lcgKSD[i], xorKSD[i], lcgKSPercents[i], xorKSPercents[i]);
            }
        }

        private void randomSimulation() {
            buildGenerators();
            clearObjects();
            getRandomValues();
        }
            
        private void runRandomTests()
        {
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

        /// <summary>
        /// Genera y almacena los números usando el Congruencial Lineal (LCG).
        /// </summary>
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

        /// <summary>
        /// Genera y almacena los números usando XorShift.
        /// </summary>
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

            lcgRepeated.Clear(); xorRepeated.Clear();
            lcgCorrelations.Clear(); xorCorrelations.Clear();
            lcgKSD.Clear(); xorKSD.Clear();
            lcgKSPercents.Clear(); xorKSPercents.Clear(); 
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

        /// <summary>
        /// Valida que las entradas del usuario en los TextBoxes sean números válidos
        /// y correspondan a los rangos permitidos para los generadores.
        /// </summary>
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
                return false;
            }

            if ((!long.TryParse(strParamCInput, out paramC) || paramC <= 0)) {
                MessageBox.Show("El parámetro c debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if ((!long.TryParse(strSeed, out paramSeed) || paramSeed <= 0)) {
                MessageBox.Show("El parámetro semilla debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!ulong.TryParse(strxorseed, out xorseed)) {
                MessageBox.Show("El parámetro semilla de xor shift debe ser un número entero positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(strShiftA, out shiftA) || shiftA <= 0 || shiftA >= 64) {
                MessageBox.Show("El parámetro semilla de shift A debe ser un número entero positivo [1, 64).", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(strShiftB, out shiftB) || shiftB <= 0 || shiftB >= 64) {
                MessageBox.Show("El parámetro semilla de shift B debe ser un número entero positivo [1, 64).", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(strShiftC, out shiftC) || shiftC <= 0 || shiftC >= 64) {
                MessageBox.Show("El parámetro semilla de shift C debe ser un número entero positivo [1, 64).", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void loadRandomDataGrids()
        {
            for (int i = 0; i < dataGridsXOR.Count; i++)
            {
                dataGridsXOR[i].AutoGenerateColumns = false;
                dataGridsXOR[i].DataSource = tablesXOR[i];
            }
            for (int i = 0; i < dataGridsLCG.Count; i++)
            {
                dataGridsLCG[i].AutoGenerateColumns = false;
                dataGridsLCG[i].DataSource = tablesLCG[i];
            }
        }

        private void unBindDataGrids()
        {
            for (int i = 0; i < dataGridsXOR.Count; i++) dataGridsXOR[i].DataSource = null;
            for (int i = 0; i < dataGridsLCG.Count; i++) dataGridsLCG[i].DataSource = null;
        }

        private void runFrequencyTests() {
            for (int i = 0; i < valuesLCGLists.Count; i++) FrequencyTest(valuesLCGLists[i], labelsRepeatedLCG[i], lcgRepeated);
            for (int i = 0; i < valuesXORLists.Count; i++) FrequencyTest(valuesXORLists[i], labelsRepeatedXOR[i], xorRepeated);
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

        /// <summary>
        /// Ejecuta y grafica la prueba de correlación matemática comparando U(i) vs U(i+1)
        /// </summary>
        private void runCorrelationTests()
        {
            for ( int i = 0; i < correlationLCGCharts.Count; i++)
            {
                graphCorrelation(correlationLCGCharts[i], valuesLCGLists[i]);
                double corrLCG = calcCorrelation(valuesLCGLists[i]);
                TextAnnotation noteLCG = (TextAnnotation)correlationLCGCharts[i].Annotations["txtAnnLCGCorrelation"];
                
                if (double.IsNaN(corrLCG)) noteLCG.Text = $"Correlación (r): No se pudo calcular (n <= 2)";
                else noteLCG.Text = $"Correlación (r): {corrLCG:F4}";
                
                lcgCorrelations.Add(corrLCG);

                graphCorrelation(correlationXORCharts[i], valuesXORLists[i]);
                double corrXOR = calcCorrelation(valuesXORLists[i]);
                TextAnnotation noteXOR = (TextAnnotation)correlationXORCharts[i].Annotations["txtAnnXorCorrelation"];
                
                if (double.IsNaN(corrXOR)) noteXOR.Text = $"Correlación (r): No se pudo calcular (n <= 2)";
                else noteXOR.Text = $"Correlación (r): {corrXOR:F4}";
                
                xorCorrelations.Add(corrXOR);
            }
        }

        private void runKSTests() {
            for (int i = 0; i < KSLCGlabels.Count; i++) KSTest(valuesLCGLists[i], KSLCGlabels[i], lcgKSD, lcgKSPercents);
            for (int i = 0; i < KSXORlabels.Count; i++) KSTest(valuesXORLists[i], KSXORlabels[i], xorKSD, xorKSPercents);
        }

        /// <summary>
        /// Prueba de Frecuencia: Detecta números repetidos en la secuencia generada 
        /// usando un HashSet para comprobar la longitud del periodo y evitar solapamientos.
        /// </summary>
        private void FrequencyTest(List<double> values, Label label, List<double> doubleNums )
        {
            HashSet<double> uniqueNumbers = new HashSet<double>(values);
            int repeatedCount = values.Count - uniqueNumbers.Count;

            Dictionary<double, int> frequencyMap = new Dictionary<double, int>();
            List<double> repeatedNumbers = new List<double>();

            foreach (var num in values)
            {
                if (!frequencyMap.ContainsKey(num)) frequencyMap[num] = 0;
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
            doubleNums.Add(repeatedCount);
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

        /// <summary>
        /// Calcula el Coeficiente de Correlación de Pearson (r) para verificar 
        /// la independencia estadística entre valores consecutivos generados.
        /// </summary>
        private double calcCorrelation(List<double> lst) {
            int n = lst.Count - 1;
            if (n <= 2) return double.NaN;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
            for ( int i = 0; i < n; i++)
            {
                double x = lst[i];
                double y = lst[i + 1];
                sumX += x; sumY += y;
                sumXY += x * y;
                sumX2 += x * x; sumY2 += y * y;
            }
            double num = (n * sumXY) - (sumX * sumY);   
            double den = Math.Sqrt(((n * sumX2) - (sumX * sumX)) * ((n * sumY2) - (sumY * sumY)));

            if (den == 0 ) return 0.0;
            return num / den;
        }

        /// <summary>
        /// Prueba de Kolmogorov-Smirnov: Determina si los números generados siguen
        /// una distribución uniforme continua comparando la Frecuencia Esperada vs Observada.
        /// </summary>
        private void KSTest(List<double> lst, Label kslabel, List<double> dValues, List<double> percentValues )
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
            dValues.Add(dCalculated);
            percentValues.Add(percent);
        }

        /// <summary>
        /// Estima matemáticamente el P-Value (Porcentaje de confianza) basado 
        /// en el valor crítico obtenido del Test de Kolmogorov-Smirnov.
        /// </summary>
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

        /// <summary>
        /// Evento disparado al solicitar la ejecución de la simulación de Montecarlo.
        /// Recopila los resultados de todos los generadores y calcula los errores de estimación.
        /// </summary>
        private async void btnRunMontecarlo_Click(object sender, EventArgs e)
        {
            btnRunMontecarlo.Enabled = false;
            loadingIcon.Visible = true;

            List<double> lcgAbsErrors = new List<double>();
            List<double> lcgRelErrors = new List<double>();
            List<double> lcgEstimatedAreas = new List<double>();

            List<double> xorAbsErrors = new List<double>();
            List<double> xorRelErrors = new List<double>();
            List<double> xorEstimatedAreas = new List<double>();

            List<double> theoricalErrors = new List<double>();

            for (int i = 0; i < valuesLCGLists.Count; i++) { 
                await runMontecarlo(valuesLCGLists[i], montecarloLCGCharts[i], montecarloLCGResultsLabels[i]);
                lcgAbsErrors.Add(absError);
                lcgRelErrors.Add(relError);
                lcgEstimatedAreas.Add(estimatedArea);
                theoricalErrors.Add(theoricalError);
            }

            for (int i = 0; i < valuesXORLists.Count; i++) { 
                await runMontecarlo(valuesXORLists[i], montecarloXORCharts[i], montecarloXORResultsLabels[i]);
                xorAbsErrors.Add(absError);
                xorRelErrors.Add(relError);
                xorEstimatedAreas.Add(estimatedArea);
            }

            gridMontecarloResults.Rows.Clear();
            for ( int i = 0; i < pointsIterations.Count; i++)
            {
                gridMontecarloResults.Rows.Add(pointsIterations[i], lcgEstimatedAreas[i], xorEstimatedAreas[i], lcgAbsErrors[i], xorAbsErrors[i], lcgRelErrors[i], xorRelErrors[i], theoricalErrors[i] );
            }

            double lcgAvgArea = lcgEstimatedAreas.Average();
            double xorAvgArea = xorEstimatedAreas.Average();

            double lcgBestArea = calcMedian(lcgEstimatedAreas);
            double xorBestArea = calcMedian(xorEstimatedAreas);

            double lcgAvgAreaAbsError = calcRealErrror(lcgBestArea, REALAREA);
            double xorAvgAreaAbsError = calcRealErrror(xorBestArea, REALAREA);

            double lcgAvgAreaRelError = calcRelativeError(lcgBestArea, REALAREA);
            double xorAvgAreaRelError = calcRelativeError(xorBestArea, REALAREA);

            gridMontecarloAvg.Rows.Clear();
            gridMontecarloAvg.Rows.Add("Congruencial",  lcgAvgArea,lcgBestArea, lcgAvgAreaRelError, lcgAvgAreaAbsError);
            gridMontecarloAvg.Rows.Add("Xor shift", xorAvgArea, xorBestArea, xorAvgAreaRelError, xorAvgAreaAbsError);

            chMontecarloAbsolute.Series[0].Points.Clear();
            chMontecarloAbsolute.Series[1].Points.Clear();
            chMontecarloAbsolute.Series[0].Points.DataBindXY(pointsIterations, lcgAbsErrors);
            chMontecarloAbsolute.Series[1].Points.DataBindXY(pointsIterations, xorAbsErrors);

            chMontecarloRelative.Series[0].Points.Clear();
            chMontecarloRelative.Series[1].Points.Clear();
            chMontecarloRelative.Series[0].Points.DataBindXY(pointsIterations, lcgRelErrors);
            chMontecarloRelative.Series[1].Points.DataBindXY(pointsIterations, xorRelErrors);

            chMontecarloTheory.Series[0].Points.Clear();
            chMontecarloTheory.Series[0].Points.DataBindXY(pointsIterations, theoricalErrors);

            tbcMontecarloIterations.Visible = true;
            loadingIcon.Visible = false;
            btnRunMontecarlo.Enabled = true;
        }

        /// <summary>
        /// Método central de la Simulación de Montecarlo estructurado con hilos (Task.Run).
        /// Recibe una lista de números pseudoaleatorios, los mezcla, calcula qué puntos 
        /// caen dentro de la curva y devuelve los resultados a la UI sin congelarla.
        /// </summary>
        private async Task runMontecarlo(List<double> values, Chart ch, Label lbl)
        {
            var resultados = await Task.Run(() =>
            {
                shuffler.Shuffle(values);

                var localPointsList = montecarloSim.processPoints(values);

                int localInside = localPointsList.inside.Count;
                int localTotalPoints = localInside + localPointsList.outside.Count;

                double localEstimatedArea = montecarloSim.EstimateArea(localTotalPoints, localInside);
                double localAbsError = calcRealErrror(localEstimatedArea, REALAREA);
                double localRelError = calcRelativeError(localEstimatedArea, REALAREA);
                double localTheoricalError = calcTheoricalError(localTotalPoints);
                
                return new
                {
                    PointsList = localPointsList,
                    Inside = localInside,
                    TotalPoints = localTotalPoints,
                    EstimatedArea = localEstimatedArea,
                    AbsError = localAbsError,
                    RelError = localRelError,
                    TheoricalError = localTheoricalError
                };
            });
            
            estimatedArea = resultados.EstimatedArea;
            absError = resultados.AbsError;
            relError = resultados.RelError;
            theoricalError = resultados.TheoricalError;
          
            lbl.Text = $"Puntos dentro = {resultados.Inside}\n" +
                       $"Puntos totales = {resultados.TotalPoints}\n" +
                       $"Área estimada = {resultados.EstimatedArea:F6}\n" +
                       $"Área calculada analíticamente = {REALAREA:F6} \n" +
                       $"Error absoluto = {resultados.AbsError:F6} \n" +
                       $"Error relativo = {(resultados.RelError * 100.0):F4}%\n" +
                       $"Error teórico ~ {resultados.TheoricalError:F6} \n" +
                       $"ℹ️ El error teórico está acotado por O(1 / √N)";
  
            ch.Series["SeriesInsidePoints"].Points.Clear();
            ch.Series["SeriesOutsidePoints"].Points.Clear();
            ch.Series["SeriesInsidePoints"].Points.DataBind(resultados.PointsList.inside, "X", "Y", "");
            ch.Series["SeriesOutsidePoints"].Points.DataBind(resultados.PointsList.outside, "X", "Y", "");
        }

        /// <summary>
        /// Calcula el Error Absoluto: |Valor Real - Valor Estimado|
        /// </summary>
        private double calcRealErrror( double result, double realResult )
        {
            return Math.Abs( realResult - result );
        }

        /// <summary>
        /// Calcula el Error Relativo Porcentual: |Valor Real - Valor Estimado| / Valor Real
        /// </summary>
        private double calcRelativeError(double result, double realResult )
        {
            return Math.Abs(result - realResult) / Math.Abs(realResult);
        }

        /// <summary>
        /// Calcula la cota de error esperada matemáticamente por la ley de los grandes números: 1 / √N
        /// </summary>
        private double calcTheoricalError(  int totalPoints )
        {
            return (double) 1 / (Math.Sqrt(totalPoints));
        }

        /// <summary>
        /// Obtiene la mediana estadística de una lista de datos (valor central ordenado).
        /// </summary>
        private double calcMedian( List<double> lst ) { 
            if ( lst == null || lst.Count == 0) return 0.0;
            var sorted = lst.OrderBy(x => x).ToList();
            int medianIndex = sorted.Count / 2;
            double median = (sorted.Count % 2 == 0) ? (sorted[medianIndex - 1] + sorted[medianIndex]) / 2.0 : sorted[medianIndex];
            return median;
        }
    }
}