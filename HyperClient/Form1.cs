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
        public Form1(Sender sender, string callSign)
        {
            this.sendingComponent = sender;
            InitializeComponent();
            this.Text = "Client " + callSign;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*string message = textBox1.Text;
            string destination = textBox4.Text;
            int interval, numberOfPackets;
            bool randomSize = radioButton1.Checked;
            if(Int32.TryParse(textBox2.Text, out interval) && Int32.TryParse(textBox3.Text, out numberOfPackets))
            {
                sendingComponent.send(message, destination, numberOfPackets, interval, randomSize);
            }*/
            Thread sendThread = new Thread(() => send());
            sendThread.Start();
            
        }

        private void send()
        {
            
            
                if(comboBox1.Items.Contains(comboBox1.Text))
                {
                    string label = comboBox1.Text.Split()[0];
                    string message = textBox1.Text;
                    int interval, numberOfPackets;
                    bool randomSize = checkBox1.Checked;
                    if (Int32.TryParse(textBox2.Text, out interval) && Int32.TryParse(textBox3.Text, out numberOfPackets))
                {
                    sendingComponent.send(message, label, numberOfPackets, interval, randomSize);
                }
                }
                
            
            
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
            this.richTextBox2.SelectionColor = Color.Red;
            this.richTextBox2.SelectedText += "Received new order from NMS: ";
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

        public void addDestination(string destination)
        {
            comboBox1.Items.Add(destination);
        }

        public void removeDestination(string destination)
        {
            comboBox1.Items.Remove(destination);
            if (comboBox1.Text == destination)
                comboBox1.Text = "";
        }

    }
}
