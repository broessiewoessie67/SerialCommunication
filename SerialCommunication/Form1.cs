using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    serialPortArduino.Close();
                    radioButtonVerbonden.Checked = false;
                    buttonConnect.Text = "Connect";
                    labelStatus.Text = "Status: Disconnected";

                }
                else
                {
                    serialPortArduino.PortName = (string)comboBoxPoort.SelectedItem;
                    serialPortArduino.BaudRate = Int32.Parse((string)comboBoxBaudrate.SelectedItem);
                    serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;

                    if(radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
                    else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
                    else if (radioButtonParityNone.Checked) serialPortArduino.Parity = Parity.None;
                    else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
                    else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;

                    if (radioButtonStopbitsNone.Checked) serialPortArduino.StopBits = StopBits.None;
                    else if (radioButtonStopbitsOne.Checked) serialPortArduino.StopBits = StopBits.One;
                    else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
                    else if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;

                    if (radioButtonHandshakeNone.Checked) serialPortArduino.Handshake = Handshake.None;
                    else if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
                    else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
                    else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;
                    
                    
                    serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;
                    serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;

                    serialPortArduino.Open();
                    string commando = "ping";
                    serialPortArduino.WriteLine(commando);
                    string antwoord = serialPortArduino.ReadLine();
                    antwoord = antwoord.TrimEnd();
                    if (antwoord == "pong")
                    {
                        radioButtonVerbonden.Checked = true;
                        buttonConnect.Text = "Disconnect";
                        labelStatus.Text = "Status: Verbonden"; 

                    }
                    else
                    { serialPortArduino.Close();
                        labelStatus.Text = "Error: verkeerd antwoord"; 
                    }
                }

            }
            catch (Exception exception)
            { labelStatus.Text = "Error: " + exception.Message;
            serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";

            }
            
        }

        private void tabPageInstellingen_Click(object sender, EventArgs e)
        {

        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 0x219 is het Windows bericht voor 'Hardware Change'
            if (m.Msg == 0x219)
            {
                // Als de poort open staat, maar hij is niet meer fysiek aanwezig in de lijst
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();
                    if (!ports.Contains(serialPortArduino.PortName))
                    {
                        // De kabel is eruit! Voer de reset uit.
                        ResetVerbindingNaVerwijderen();
                    }
                }
            }
        }

        // 2. Deze methode herstelt de UI en sluit de poort netjes af
        private void ResetVerbindingNaVerwijderen()
        {
            try
            {
                // Probeer de poort softwarematig te sluiten
                if (serialPortArduino.IsOpen)
                {
                    serialPortArduino.Close();
                }
            }
            catch { /* Negeren, poort is toch al weg */ }

            // UI Terugzetten naar 'Niet verbonden' stand
            buttonConnect.Text = "Connect";             // Knop terug naar Connect
            radioButtonVerbonden.Checked = false;      // Lampje/Radiobutton uit
            labelStatus.Text = "Status: Niet verbonden (USB losgekoppeld)";
            labelStatus.ForeColor = System.Drawing.Color.Red;

            // Toon een duidelijke melding aan de gebruiker
            MessageBox.Show("De USB-kabel is uitgetrokken. De verbinding is verbroken.",
                            "Verbinding Verloren",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            {
                if (!serialPortArduino.IsOpen)
                    return;
                try
                {
                    // Gewenste temperatuur (AO)
                    serialPortArduino.WriteLine("get a0");
                    if (!serialPortArduino.IsOpen)
                        return;
                    serialPortArduino.ReadExisting();
                    serialPortArduino.WriteLine("get a0");
                    string antwoord = serialPortArduino.ReadLine().Trim();
                    if (antwoord.Length < 4)
                        return;
                    if (!int.TryParse(antwoord.Substring(4), out int rawGewenst))
                        return;
                    labelAnalog0.Text = rawGewenst.ToString();
                    double gewensteTemp = (40.0 / 1023.0) * rawGewenst + 5.0;
                    labelGewensteTemp.Text = gewensteTemp.ToString("0.0") + " °C";
                    // Huidige temperatur (A1)
                    serialPortArduino.WriteLine("get a1");
                    if (!serialPortArduino.IsOpen)
                        return;
                    serialPortArduino.ReadExisting();
                    serialPortArduino.WriteLine("get a1");
                    string antwoord2 = serialPortArduino.ReadLine().Trim();
                    if (antwoord2.Length < 4)
                        return;
                    if (!int.TryParse(antwoord2.Substring(4), out int rawHuidig))
                        return;
                    if (rawHuidig < 20)
                        return;
                    double huidigeTemp = (rawHuidig * 500) / 1023.0;
                    labelHuidigeTemp.Text = huidigeTemp.ToString("0.0") + " °C";
                    // LED aansturen
                    if (huidigeTemp < gewensteTemp)
                        serialPortArduino.WriteLine("set d2 high");
                    else
                        serialPortArduino.WriteLine("set d2 low");
                }
                catch (Exception exception)
                {
                    labelStatus.Text = "Error: " + exception.Message;
                    serialPortArduino.Close();
                    radioButtonVerbonden.Checked = false;
                    buttonConnect.Text = "Connect";
                }
            }


        }
    }

}

