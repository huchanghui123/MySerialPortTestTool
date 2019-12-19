using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MySerialPort
{
    public partial class Form2 : Form
    {
        private String[] sps = null;
        private int serialCount = 1;
        private int defaultWidth = 300;
        private int defaultHeight = 300;
        
        private int panelX = 10;
        private int panelY = 10;
        private int statePanelX = 10;
        private int statePanelY = 310;

        private StringBuilder sb = new StringBuilder();
        private List<Panel> recevie_panel_list = new List<Panel>();
        private List<Panel> state_panel_list = new List<Panel>();
        private List<SerialPort> serial_list = new List<SerialPort>();

        public Form2()
        {
            InitializeComponent();
            //this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Location = new Point(200,20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        }

        private void TestAllLoad(object sender, EventArgs e)
        {
            sps = SerialPort.GetPortNames();
            serialCount = sps.Length;
            int count = 0;
            int formWidth = 0;
            int formHeight = 380;
            //多于3个串口，高度翻倍，比如有5串，上3下2
            if (serialCount > 3)
            {
                formHeight += 360;
                count = serialCount - 3;
            }
            for (int i = 0;i < serialCount - count; i++)
            {
                formWidth += defaultWidth;
            }
            Size newSize = new Size(formWidth, formHeight);
            this.MaximumSize = this.MinimumSize = newSize;
            this.Size = newSize;

            CreateReceivePanelList();
            CreateStatePanelList();
            CreateSerialPort();
            OpenSerialPort();
            timer1.Start();
        }

        private Label CreateRecevideLabel(String label_text)
        {
            Label label = new Label
            {
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                AutoSize = true,
                Font = new System.Drawing.Font("楷体", 12F, System.Drawing.FontStyle.Regular,
                    System.Drawing.GraphicsUnit.Point, ((byte)(134))),
                Text = label_text
            };
            return label;
        }

        private TextBox CreateRecevideTextBox()
        {
            TextBox textBox = new TextBox
            {
                Font = new System.Drawing.Font("宋体", 10F),
                Multiline = true,
                ScrollBars = System.Windows.Forms.ScrollBars.Vertical,
                Size = new System.Drawing.Size(defaultWidth - 20, defaultWidth - 25),
                Name = "recevie"
            };
            textBox.Location = new System.Drawing.Point(0, 20);
            return textBox;
        }

        private Panel CreateReceivePanel(int i,int type)
        {
            panelY = 10;
            Panel panel = new Panel
            {
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            };
            Label label = null;
            if (type == 1)
            {
                panel.Name = sps[i + 3];
                label = CreateRecevideLabel(sps[i + 3]);
                panelY += 340;
                panel.TabIndex = i + 3;
            }
            else
            {
                panel.Name = sps[i];
                label = CreateRecevideLabel(sps[i]);
                panel.TabIndex = i;
            }
            
            //Label label = CreateRecevideLabel(sps[i + 3]);
            TextBox textBox = CreateRecevideTextBox();

            panelX = i * defaultWidth == 0 ? 10 : i * defaultWidth;
            
            panel.Location = new System.Drawing.Point(panelX, panelY);
            panel.Size = new System.Drawing.Size(defaultWidth - 20, defaultHeight);

            panel.Controls.Add(label);
            panel.Controls.Add(textBox);
            

            return panel;
        }

        //生成接收面板
        private void CreateReceivePanelList()
        {
            recevie_panel_list.Clear();
            int Xcount = 0;
            int Ycount = 0;

            if (serialCount > 3)
            {
                Xcount = 3;
                Ycount = serialCount - 3;
            }
            else
            {
                Xcount = serialCount;
            }

            //X轴
            for (int i = 0; i < Xcount; i++)
            {
                Panel panel = CreateReceivePanel(i, 0);

                recevie_panel_list.Add(panel);
            }
            //Y轴
            for (int i = 0; i < Ycount; i++)
            {
                Panel panel = CreateReceivePanel(i, 1);

                recevie_panel_list.Add(panel);
            }

            foreach (Panel panel in recevie_panel_list)
            {
                this.Controls.Add(panel);
            }
        }

        private Label CreateStateLabel()
        {
            Label label = new Label
            {
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                AutoSize = true,
                Location = new System.Drawing.Point(4, 4),
                Font = new System.Drawing.Font("楷体", 10F, System.Drawing.FontStyle.Regular,
                    System.Drawing.GraphicsUnit.Point, ((byte)(134))),
                Text = "State：Close",
                Name = "STATE"
            };
            return label;
        }

        private Label CreateStateSignalLabel(int pointx,String text)
        {
            Label label = new Label
            {
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                AutoSize = true,
                Location = new System.Drawing.Point(pointx, 4),
                Font = new System.Drawing.Font("楷体", 10F, System.Drawing.FontStyle.Regular,
                    System.Drawing.GraphicsUnit.Point, ((byte)(134))),
                BackColor = Color.LightGray,
                Text = text,
                Name = text
            };

            return label;
        }

        private Panel CreateStatePanel(int i, int type)
        {
            statePanelY = 310;
            Panel panel = new Panel
            {
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            };
            if (type == 1)
            {
                panel.Name = sps[i + 3] + "_State";
                statePanelY += 350;
                panel.TabIndex = i + 3;
            }
            else
            {
                panel.Name = sps[i] + "_State";
                panel.TabIndex = i;
            }

            Label label = CreateStateLabel();
            Label label_CTS = CreateStateSignalLabel(120, "CTS");
            Label label_DSR = CreateStateSignalLabel(160, "DSR");
            Label label_DCD = CreateStateSignalLabel(200, "DCD");

            statePanelX = i * defaultWidth == 0 ? 10 : i * defaultWidth;
            panel.Location = new System.Drawing.Point(statePanelX, statePanelY);
            panel.Size = new System.Drawing.Size(250, 30);

            panel.Controls.Add(label);
            panel.Controls.Add(label_CTS);
            panel.Controls.Add(label_DSR);
            panel.Controls.Add(label_DCD);

            return panel;
        }

        //生成状态面板
        private void CreateStatePanelList()
        {
            state_panel_list.Clear();
            int Xcount = 0;
            int Ycount = 0;

            if (serialCount > 3)
            {
                Xcount = 3;
                Ycount = serialCount - 3;
            }
            else
            {
                Xcount = serialCount;
            }
            for (int i = 0; i < Xcount; i++)
            {
                Panel panel = CreateStatePanel(i, 0);
                state_panel_list.Add(panel);
            }
            for (int i = 0; i < Ycount; i++)
            {
                Panel panel = CreateStatePanel(i, 1);
                state_panel_list.Add(panel);
            }

            foreach (Panel panel in state_panel_list)
            {
                this.Controls.Add(panel);
            }

        }

        private void CreateSerialPort()
        {
            serial_list.Clear();
            for (int i = 0; i < serialCount; i++)
            {
                SerialPort serialPort = new SerialPort();
                serialPort.PortName = sps[i];
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.Handshake = Handshake.None;
                serialPort.Encoding = System.Text.Encoding.GetEncoding("GB2312");

                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;
                serialPort.RtsEnable = true;
                serialPort.DtrEnable = true;

                serialPort.DataReceived += new SerialDataReceivedEventHandler(My_SerialPort_DataReceived);
                serial_list.Add(serialPort);
            }
        }
        
        private void OpenSerialPort()
        {
            foreach (SerialPort sp in serial_list)
            {
                try
                {
                    sp.Open();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    sp.Close();
                    Console.WriteLine("OpenSerialPort:" + ex.Message.ToString());
                }
                
            }
        }

        private void My_SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            //byte[] ReDatas = new byte[serialPort.BytesToRead];
            String recevieName = "";
            String message = "";
            try
            {
                recevieName = serialPort.PortName;
                Console.WriteLine("Received:" + recevieName + ":" + serialPort.IsOpen);
                //serialPort.Read(ReDatas, 0, ReDatas.Length);
                //String str = Encoding.ASCII.GetString(ReDatas);

                message = serialPort.ReadLine();
                //message = serialPort.ReadExisting();
                Console.WriteLine(recevieName + " Received data:" + message);

                foreach (Panel recevie_panel in recevie_panel_list)
                {
                    //sb.Clear();
                    sb.Remove(0, sb.Length);
                    sb.Append(message);

                    if (recevieName.Equals(recevie_panel.Name))
                    {
                        Invoke((EventHandler)(delegate
                        {
                            ((TextBox)recevie_panel.Controls["recevie"]).AppendText(sb.ToString());
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                Console.WriteLine("Received:" + ex.Message.ToString());
            }
            
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int index = 0;
            foreach (Panel state_panel in state_panel_list)
            {
                try
                {
                    index = state_panel_list.IndexOf(state_panel);
                    Console.WriteLine(serial_list[index].PortName + "----------" + serial_list[index].IsOpen);
                    if (serial_list[index].IsOpen)
                    {
                        String str = serial_list[index].PortName + "发送中...";
                        Console.WriteLine("send data:" + str);
                        serial_list[index].WriteLine(str);
                        //serial_list[index].Write(str);
                    }

                    Invoke((EventHandler)(delegate
                    {

                        if (serial_list[index].IsOpen)
                        {
                            ((Label)state_panel.Controls["STATE"]).Text = "State：Open";
                        }
                        else
                        {
                            ((Label)state_panel.Controls["STATE"]).Text = "State：Close";
                        }
                        //*******************************CTS********************************
                        if (serial_list[index].CtsHolding)
                        {
                            ((Label)state_panel.Controls["CTS"]).BackColor = Color.Green;
                        }
                        else
                        {
                            ((Label)state_panel.Controls["CTS"]).BackColor = Color.LightGray;
                        }
                        //*******************************DSR********************************
                        if (serial_list[index].DsrHolding)
                        {
                            ((Label)state_panel.Controls["DSR"]).BackColor = Color.Green;
                        }
                        else
                        {
                            ((Label)state_panel.Controls["DSR"]).BackColor = Color.LightGray;
                        }
                        //*******************************DCD*********************************
                        if (serial_list[index].CDHolding)
                        {
                            ((Label)state_panel.Controls["DCD"]).BackColor = Color.Green;
                        }
                        else
                        {
                            ((Label)state_panel.Controls["DCD"]).BackColor = Color.LightGray;
                        }

                    }));
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    Console.WriteLine("Timer_Tick:"+ ex.Message);
                }
            }
        }
        
        private void Form2_Closing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("FormClosing...............");
            foreach (SerialPort sp in serial_list)
            {
                sp.Close();
            }
            timer1.Stop();
        }
    }
}
