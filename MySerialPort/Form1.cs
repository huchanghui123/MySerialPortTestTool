using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MySerialPort
{
    public partial class Form1 : Form
    {
        private long receive_count = 0; //接收字节计数
        private long send_count = 0;    //发送字节计数
        private int port_count = 0;

        private string indata = "";
        private StringBuilder sb = new StringBuilder();
        private DateTime current_time = new DateTime();
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

        private Form2 testAll = null;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            version_label.Text = "版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            port_count = SerialPort.GetPortNames().Length;
            comboBox1.Items.AddRange(SerialPort.GetPortNames());

            string[] baud2 = { "9600", "19200", "38400", "56000", "57600", "115200", "128000" };
            comboBox2.Items.AddRange(baud2);
            string[] baud4 = { StopBits.None+"", StopBits.One+"", StopBits.Two+"", StopBits.OnePointFive+"" };
            comboBox4.Items.AddRange(baud4);
            string[] baud5 = { Parity.None+"", Parity.Odd+"", Parity.Even+"", Parity.Mark+"", Parity.Space+"" };
            comboBox5.Items.AddRange(baud5);
            string[] baud6 = { Handshake.None+"", Handshake.XOnXOff+"", Handshake.RequestToSend+"", Handshake.RequestToSendXOnXOff+""};
            comboBox6.Items.AddRange(baud6);
            //设置默认值
            comboBox1.Text = serialPort1.PortName;
            comboBox2.Text = "115200";
            comboBox3.Text = serialPort1.DataBits.ToString();
            comboBox4.Text = serialPort1.StopBits.ToString();
            comboBox5.Text = serialPort1.Parity.ToString();
            comboBox6.Text = serialPort1.Handshake.ToString();
            
            serialPort1.Encoding = System.Text.Encoding.GetEncoding("GB2312");

            String time = GetComputerStartTime().ToString();
            start_time.Text = time;
        }

        private void OpenSerial_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();

                    open_btn.Text = "打开串口";
                    label23.Text = "CLOSE";

                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                    comboBox6.Enabled = true;
                    checkBox4.Checked = false;
                    checkBox4.Enabled = false;
                    //textBox_receive.Text = "";
                    //textBox_send.Text = "";
                    rts_check.Enabled = true;
                    dtr_check.Enabled = true;
                    

                    label7.Text = "串口接收区";
                    send_btn.Enabled = false;
                    testall_btn.Enabled = true;
                    pictureBox1.Image = global::MySerialPort.Properties.Resources.sun_128px_close;

                    dcd_label.BackColor = Color.LightGray;
                    cts_label.BackColor = Color.LightGray;
                    dsr_label.BackColor = Color.LightGray;
                }
                else
                {
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    comboBox6.Enabled = false;
                    checkBox4.Enabled = true;
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.DataBits = int.Parse(comboBox3.Text.ToUpperInvariant());
                    serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), comboBox4.Text, true);
                    serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), comboBox5.Text, true);
                    serialPort1.Handshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox6.Text, true);

                    if (rts_check.Checked)
                    {
                        serialPort1.RtsEnable = true;
                    }
                    else
                    {
                        serialPort1.RtsEnable = false;
                    }

                    if (dtr_check.Checked)
                    {
                        serialPort1.DtrEnable = true;
                    }
                    else
                    {
                        serialPort1.DtrEnable = false;
                    }
                    rts_check.Enabled = false;
                    dtr_check.Enabled = false;
                    
                    serialPort1.Open();

                    //Console.WriteLine(serialPort1.BaudRate+"--"+ serialPort1.DataBits + "--" 
                    //    + serialPort1.StopBits + "--" + serialPort1.Parity + "--" + serialPort1.Handshake);
                    
                    open_btn.Text = "关闭串口";
                    label23.Text = "OPEN";
                    label7.Text = comboBox1.Text+" Ready";
                    send_btn.Enabled = true;
                    testall_btn.Enabled = false;
                    pictureBox1.Image = global::MySerialPort.Properties.Resources.sun_128px_open;

                    //Console.WriteLine("CDHolding:" + serialPort1.CDHolding+ " CtsHolding:"
                    //    + serialPort1.CtsHolding  + " DsrHolding:" + serialPort1.DsrHolding);
                    if (serialPort1.CDHolding)
                    {
                        dcd_label.BackColor = Color.Green;
                    }
                    if (serialPort1.CtsHolding)
                    {
                        cts_label.BackColor = Color.Green;
                    }
                    if (serialPort1.DsrHolding)
                    {
                        dsr_label.BackColor = Color.Green;
                    }

                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                open_btn.Text = "打开串口";
                MessageBox.Show(ex.Message);
                
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
                checkBox4.Checked = false;
                checkBox4.Enabled = false;
            }
        }


        private void Send_Click(object sender, EventArgs e)
        {
            byte[] temp = new byte[1];
            try
            {
                if (serialPort1.IsOpen)
                {
                    int num = 0;

                    serialPort1.WriteLine(textBox_send.Text);
                    num = textBox_send.Text.Length;
                    send_count += num;
                    label_send.Text = "发送字节："+send_count.ToString() + "Bytes"; 
                    if (checkBox2.Checked)
                    {
                        textBox_send.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                serialPort1.Close();
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                open_btn.Text = "打开串口";
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
            }
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            byte[] ReDatas = new byte[serialPort.BytesToRead];
            try
            {
                
                //Console.WriteLine("Received:" + ReDatas.Length);
                indata = serialPort.ReadLine();
                //serialPort.Read(ReDatas, 0, ReDatas.Length);
                //indata = Encoding.ASCII.GetString(ReDatas, 0, ReDatas.Length);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                Invoke((EventHandler)(delegate {
                    textBox_receive.AppendText(ex.Message+ " Received:"+ ReDatas.Length);
                    send_btn.Enabled = false;
                }));
            }

            string str0 = "";
            string str = "";
            if (indata.IndexOf(">@") > 0)
            {
                str0 = indata.Split('@')[0];
                str = indata.Split('@')[1];
                //Console.WriteLine(str0 + "-------" + str);
            }
            if (stringComparer.Equals("<start_time>", str0))
            {
                Update_time(str);
            }
            else
            {
                int num = indata.Length;
                receive_count += num;
                //Console.WriteLine("-------" + indata + " num:" + num);
                if (stringComparer.Equals("reboot", indata))
                {
                    //1s 后重启计算机
                    System.Diagnostics.Process.Start("shutdown.exe", "-r -t 1");
                }
                else if (stringComparer.Equals("shutdown", indata))
                {
                    //1s 后关闭计算机
                    System.Diagnostics.Process.Start("shutdown.exe", "-s -t 1");
                }
                else if (stringComparer.Equals("pause", indata))
                {
                    try
                    {
                        Invoke((EventHandler)(delegate {
                            checkBox4.Checked = false;
                        }));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                else if (stringComparer.Equals("continue", indata))
                {
                    try
                    {
                        Invoke((EventHandler)(delegate {
                            checkBox4.Checked = true;
                        }));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                //Framework 4.0以上才有Clear
                //sb.Clear();
                sb.Remove(0,sb.Length);
                sb.Append(indata);
                indata = "";
                if (checkBox1.Checked)
                {
                    sb.Append("\r\n");
                }

                try
                {
                    Invoke((EventHandler)(delegate {

                        if (checkBox3.Checked)
                        {
                            current_time = System.DateTime.Now;
                            textBox_receive.AppendText(current_time.ToString("HH:mm:ss") + "  " + sb.ToString());
                        }
                        else
                        {
                            textBox_receive.AppendText(sb.ToString());
                        }

                        label_receive.Text = "接收字节：" + receive_count.ToString() + "Bytes";
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox_receive.Text = "";  
            receive_count = 0;          
            label_receive.Text = "接收字节：" + receive_count.ToString() + "Bytes";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox_send.Text = "";
            send_count = 0;
            label_send.Text = "发送字节：" + send_count.ToString() + "Bytes";
        }

        private void AutoSend_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                //自动发送开始
                numericUpDown1.Enabled = false;
                textBox_send.Enabled = false;
                send_btn.Enabled = false;
                timer1.Interval = (int)numericUpDown1.Value;
                timer1.Start();
                label10.Text = "自动发送中...";
                rts_check.Enabled = false;
                dtr_check.Enabled = false;
            }
            else
            {
                //停止自动发送
                numericUpDown1.Enabled = true;
                textBox_send.Enabled = true;
                send_btn.Enabled = true;
                timer1.Stop();
                label10.Text = "";
                rts_check.Enabled = true;
                dtr_check.Enabled = true;
            }
        }

        private void Timer1_tick(object sender, EventArgs e)
        {
            //调用发送按钮回调函数
            Send_Click(send_btn, new EventArgs());
        }

        //获取系统开机时间
        private DateTime GetComputerStartTime()
        {
            int result = Environment.TickCount & Int32.MaxValue;     //获取系统启动后运行的毫秒数
            TimeSpan m_WorkTimeTemp = new TimeSpan(Convert.ToInt64(Convert.ToInt64(result) * 10000));

            DateTime startTime = System.DateTime.Now.AddDays(m_WorkTimeTemp.Days);
            startTime = startTime.AddHours(-m_WorkTimeTemp.Hours);
            startTime = startTime.AddMinutes(-m_WorkTimeTemp.Minutes);
            startTime = startTime.AddSeconds(-m_WorkTimeTemp.Seconds);
            //Console.WriteLine("GetComputerStartTime:"+ startTime.ToString());
            return startTime;
        }

        private void Update_time(string starttime)
        {
            try
            {
                Invoke((EventHandler)(delegate {
                    start_time.Text = starttime;
                    serialPort1.WriteLine("<update_time_end>");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TestAllSerialPort(object sender, EventArgs e)
        {
            testAll = new Form2();
            testAll.ShowDialog();
        }

        private void Pin_Changed(object sender, SerialPinChangedEventArgs e)
        {
            //Console.WriteLine("Pin_Changed..............");
            if (serialPort1.CDHolding)
            {
                dcd_label.BackColor = Color.Green;
            }
            else
            {
                dcd_label.BackColor = Color.LightGray;
            }
            if (serialPort1.CtsHolding)
            {
                cts_label.BackColor = Color.Green;
            }
            else
            {
                cts_label.BackColor = Color.LightGray;
            }
            if (serialPort1.DsrHolding)
            {
                dsr_label.BackColor = Color.Green;
            }
            else
            {
                dsr_label.BackColor = Color.LightGray;
            }
        }
    }
}
