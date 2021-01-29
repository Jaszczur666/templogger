using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace TempLogger
{
    public partial class Form1 : Form
    {
        static Stopwatch sw=new Stopwatch();
        public double temperature;
        private long i=0;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log.Info("Start programu");
            sw.Start();
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string s in ports)
            {
               comboBox1.Items.Add(s);
                
            }
            try
            {
                string config = System.IO.File.ReadAllText("ports.cfg");
                var result = System.Text.RegularExpressions.Regex.Split(config, "\r\n|\r|\n");
                comboBox1.Text = result[0];
            }
            catch { };
            //            serialPort1.Open();
        }

        private void Chart1_Click(object sender, EventArgs e)
        {

        }
        private void UpdateChart()
        {
            Invoke((MethodInvoker)delegate {
                log.Info("Chart update ");
                log.Debug("temperature is " + temperature.ToString()) ;
            chart1.Series[0].Points.AddXY(sw.Elapsed.TotalSeconds, temperature);
            });


        }
        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string data = serialPort1.ReadLine();
            System.Globalization.CultureInfo cul = System.Globalization.CultureInfo.InvariantCulture;
            double.TryParse(data,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture, out temperature);
            log.Debug("dane "+ sw.Elapsed.TotalSeconds.ToString("g5",cul)+" "+temperature.ToString("g5", cul));
            System.IO.File.AppendAllText("zrzut.dat", sw.Elapsed.TotalSeconds.ToString("g5", cul) + " " + temperature.ToString("g5", cul) +" "+ DateTime.Now.ToString() + "\r\n") ;

            if (i % 20 == 0)
            {
                UpdateChart();
                i = 0;
            }
            i++;
            log.Debug("i=" + i.ToString());

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            log.Debug("Startujmy");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.Text;
            serialPort1.Open();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string config;
            config = comboBox1.Text;
            System.IO.File.WriteAllText("ports.cfg", config);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string wynik="";
                int size = chart1.Series[0].Points.Count;
                for (int i = 0; i < size; i++) {
                    wynik += chart1.Series[0].Points[i].XValue.ToString() + " " + chart1.Series[0].Points[i].YValues[0].ToString()+"\r\n";
                        }
                File.WriteAllText(saveFileDialog1.FileName, wynik);
            }
        }
    }
}
