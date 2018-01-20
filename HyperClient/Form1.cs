using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HyperClient
{
    public partial class Form1 : Form
    {
        Sender sendingComponent;
        CPCC cpcc;
        public Form1(Sender sender, CPCC cpcc, string callSign)
        {
            this.sendingComponent = sender;
            this.cpcc = cpcc;
            InitializeComponent();
            this.Text = "Client " + callSign;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Thread sendThread = new Thread(() => send());
            sendThread.Start();

        }

        private void send()
        {

            string message = textBox1.Text;
            int interval, numberOfPackets, label;
            bool randomSize = checkBox1.Checked;
            if (Int32.TryParse(textBox2.Text, out interval) && Int32.TryParse(textBox3.Text, out numberOfPackets) && Int32.TryParse(textBox4.Text, out label))
            {
                sendingComponent.send(message, label, numberOfPackets, interval, randomSize);
            }

        }

        private void makeCallRequest()
        {
            //trzeba do tego przekazać argumenty pozwalające zbudować odpowiedniego requesta
            cpcc.sendCallRequest();
        }

        private void makeHangUpRequest()
        {
            //trzeba do tego przekazać argumenty pozwalające zbudować odpowiedniego requesta
            cpcc.sendHangUpRequest();
        }
       

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public void logReceivedPacket(string message, string timestamp)
        {
            this.richTextBox1.SelectionColor = Color.Blue;
            this.richTextBox1.SelectedText += "Received message ";
            this.richTextBox1.SelectionColor = Color.Black;
            this.richTextBox1.SelectedText += message;
            this.richTextBox1.SelectionColor = Color.DarkMagenta;
            this.richTextBox1.SelectedText += timestamp;
            this.richTextBox1.ScrollToCaret();
        }

        public void logSentPacket(string message, string timestamp)
        {
            this.richTextBox1.SelectionColor = Color.Green;
            this.richTextBox1.SelectedText += "Sent message ";
            this.richTextBox1.SelectionColor = Color.Black;
            this.richTextBox1.SelectedText += message;
            this.richTextBox1.SelectionColor = Color.DarkMagenta;
            this.richTextBox1.SelectedText += timestamp;
            this.richTextBox1.ScrollToCaret();
        }

        public void logReceivedOrder(string message, string timestamp)
        {
            this.richTextBox2.SelectionColor = Color.Blue;
            this.richTextBox2.SelectedText += "Received new message from NCC: ";
            this.richTextBox2.SelectionColor = Color.Black;
            this.richTextBox2.SelectedText += message;
            this.richTextBox2.SelectionColor = Color.DarkMagenta;
            this.richTextBox2.SelectedText += timestamp;
            this.richTextBox2.ScrollToCaret();
        }

        public void logSentOrder(string message, string timestamp)
        {
            this.richTextBox2.SelectionColor = Color.Green;
            this.richTextBox2.SelectedText += "Sent new message to NCC: ";
            this.richTextBox2.SelectionColor = Color.Black;
            this.richTextBox2.SelectedText += message;
            this.richTextBox2.SelectionColor = Color.DarkMagenta;
            this.richTextBox2.SelectedText += timestamp;
            this.richTextBox2.ScrollToCaret();
        }



        public void allowSenderToLog()
        {
            sendingComponent.setConnectionToLogViewer(this);
        }
        public void allowCPCCToLog()
        {
            cpcc.setConnectionToLogViewer(this);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
                sendingComponent.stopSending();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread callRequestThread = new Thread(() => makeCallRequest());
            callRequestThread.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Thread hangUpRequestThread = new Thread(() => makeHangUpRequest());
            hangUpRequestThread.Start();
        }
    }
}
