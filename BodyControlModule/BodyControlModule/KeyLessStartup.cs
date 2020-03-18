using Microsoft.Win32;
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

namespace BodyControlModule
{
    public partial class KeyLessStartup : Form
    {
        private SerialPort serialPort = new SerialPort();
        bool isOpened = false;//串口状态标志
        SerialPorCommon apc = new SerialPorCommon();

        public KeyLessStartup()
        {
            InitializeComponent();
            serialPort.DataReceived += new SerialDataReceivedEventHandler(post_DataReceived);//串口接收处理函数
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
           
        }
        private bool istrue = true, istrue2 = true, istrue3 = true, istrue4 = true, istrue5 = true, istrue6 = true, istrue7 = true,
        istrue8 = true, istrue9 = true, istrue10 = true, istrue11 = true, istrue12 = true, istrue13 = true, istrue14 = true,
        istrue15 = true, istrue16 = true, istrue17 = true, istrue18 = true, istrue19 = false;

        private void button9_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button20_Click(object sender, EventArgs e)
        {
            // atl.openCom();
            apc.openCom(comboBox1, comboBox2, button20);
            //if (!isOpened)
            //{
            //    serialPort.PortName = comboBox1.Text;
            //    serialPort.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
            //    try
            //    {
            //        serialPort.Open();     //打开串口
            //        button20.Text = "关闭串口";
            //        comboBox1.Enabled = false;//关闭使能
            //        comboBox2.Enabled = false;
            //        isOpened = true;
            //        serialPort.DataReceived += new SerialDataReceivedEventHandler(post_DataReceived);//串口接收处理函数
            //    }
            //    catch
            //    {
            //        MessageBox.Show("串口打开失败！");
            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        serialPort.Close();     //关闭串口
            //        button20.Text = "打开串口";
            //        comboBox1.Enabled = true;//打开使能
            //        comboBox2.Enabled = true;
            //        isOpened = false;
            //    }
            //    catch
            //    {
            //        MessageBox.Show("串口关闭失败！");
            //    }
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[serialPort.BytesToRead];
            Console.WriteLine(serialPort.BytesToRead);
            serialPort.Read(ReDatas, 0, ReDatas.Length);//读取数据
            apc.ProcessShowData(ReDatas,this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ProcessShowData(byte[] data,Form from)
        {
            from.BeginInvoke(new MethodInvoker(delegate
            {
                byte[] checkdata = new byte[data.Length];
                int pos = 0;
                int len = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    checkdata[pos] = data[i];
                    pos++;
                    switch (pos)
                    {
                        case 1://head ff
                            if (checkdata[pos - 1] != 0xff) pos = 0;
                            break;
                        case 2://head 55
                            if (checkdata[pos - 1] != 0x55) pos = 0;
                            break;
                        case 3://length
                            len = data[2];
                            break;
                        default:
                            {
                                if (pos == len)
                                {
                                    CalculateCheckSum(checkdata, pos);
                                    if (checkdata[pos - 1] != data[i]) // check failed
                                    {
                                        pos = 0;
                                        len = 0;
                                        break;
                                    }
                                    // process data
                                    this.show(data);
                                }
                            }
                            break;
                    }
                }
            }));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buff"></param>
        public  void show(byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byteArray.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", byteArray[i]);
            }
            Console.WriteLine(sb.ToString());
            switch (byteArray[4])
            {
                case 0x14:
                    {   
                        //左前车门
                        int bit0 = (byteArray[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0==1)//左前门打开
                        {
                            pictureBox7.Show();
                        }
                        else if (bit0 ==0) //左前门关闭
                        {
                            pictureBox7.Hide();
                        }

                        //右前车门
                        int bit1 = (byteArray[5] & 0x02) == 0x02 ? 1 : 0;
                        if (bit1 == 1)//右前门打开
                        {
                            pictureBox5.Show();
                        }
                        else if (bit1 == 0)//右前门关闭
                        {
                            pictureBox5.Hide();
                        }
                        //左后门
                        int bit2 = (byteArray[5] & 0x03) == 0x03 ? 1 : 0;
                        if (bit2 == 1)//左后门打开
                        {
                            pictureBox2.Show();
                        }
                        else if (bit2 == 0) //左后门关闭
                        {
                            pictureBox2.Hide();
                        }
                        //右后门
                        int bit3 = (byteArray[5] & 0x04) == 0x04 ? 1 : 0;
                        if (bit3 == 1)//右后门打开
                        {
                            pictureBox4.Show();
                        }
                        else if (bit3 == 0) //右后门关闭
                        {
                            pictureBox4.Hide();
                        }
                        //后备箱
                        int bit4 = (byteArray[5] & 0x05) == 0x05 ? 1 : 0;
                        if (bit4 == 1)//后备箱打开
                        {
                            pictureBox1.Show();
                        }
                        else if (bit4 == 0) //后备箱关闭
                        {
                            pictureBox1.Hide();
                        }
                        //引擎盖
                        int bit5 = (byteArray[5] & 0x06) == 0x06 ? 1 : 0;
                        if (bit5 == 1)//引擎盖打开
                        {
                            pictureBox6.Show();
                        }
                        else if (bit5 == 0) //引擎盖关闭
                        {
                            pictureBox6.Hide();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BuffData"></param>
        /// <param name="len"></param>
        /// <param name="sender"></param>
        private void SendToPort(Byte[] BuffData, int len, object sender)
        {
            int SendLen = 4;
            Byte[] SendBuff = new Byte[SendLen];
            if (SendBuff == null) return;
            SendBuff[0] = 0xff;
            SendBuff[1] = 0x55;
            SendBuff[2] = (Byte)SendLen;
            SendBuff[3] = 0x01;

            List<byte> lTemp = new List<byte>();
            lTemp.AddRange(SendBuff);
            lTemp.AddRange(BuffData);
            SendBuff = new byte[lTemp.Count + 1];
            lTemp.CopyTo(SendBuff);

            CalculateCheckSum(SendBuff, SendBuff.Length);//求和校验
            ProcessAndShow(SendBuff, SendBuff.Length, sender);//处理数据
        }
        private void ProcessAndShow(Byte[] buff, int len, object sender)
        {
            Button button = (Button)sender;
            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
            int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
            int bit2 = (buff[5] & 0x03) == 0x03 ? 1 : 0;
            int bit3 = (buff[5] & 0x04) == 0x04 ? 1 : 0;
            int bit4 = (buff[5] & 0x05) == 0x05 ? 1 : 0;
            switch (buff[4])
            {
                case 0x14:
                    {
                       
                        if (button.Name.Equals("button3"))
                        {
                            
                            if (bit0 ==0 && bit1 == 0 && bit2== 0 && bit3== 0 )//车门上锁
                            {
                                pictureBox8.Show();
                            }
                        }
                        if (button.Name.Equals("button2"))
                        {                          
                            if (bit0 == 1 && bit1 == 1 && bit2 == 1 && bit3 == 1 )//车门解锁
                            {
                                pictureBox8.Show();
                            }
                        }
                        if (button.Name.Equals("button1"))
                        {
                            if (bit0==1 && bit1 == 0 && bit2 == 0 && bit3 == 0)//解锁驾驶门
                            {
                                pictureBox8.Show();
                            }
                        }
                        if (button.Name.Equals("button4"))
                        {
                            if (bit4==1)//解锁后备箱
                            {
                                pictureBox1.Show ();
                            }
                            else if (bit4==0)
                            {
                                pictureBox1.Hide();
                            }
                        }
                    }
                    break;

                case 0x15:
                    {
                        if (bit0 == 0 && bit1 == 0 && bit2 == 0 && bit3 == 0)//车门上锁
                        {
                            pictureBox8.Show();
                        }
                        else if (bit0 == 1 && bit1 == 1 && bit2 == 1 && bit3 == 1)
                        {
                            pictureBox8.Hide();
                        }

                    }
                    break;
                default:
                    break;
            }
        }
            /// <summary>
            /// 求和校验
            /// </summary>
            /// <param name="pBuf"></param>
            /// <param name="len"></param>
            private void CalculateCheckSum(Byte[] BuffData, int len)
        {
            if (BuffData == null) return;
            byte CheckSum = 0;
            for (int i = 3; i < (len - 1); i++)
            {
                CheckSum += BuffData[i];
            }
            BuffData[len - 1] = CheckSum;
        }
        private void KeyLessStartup_Load(object sender, EventArgs e)
        {
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();
                comboBox1.Items.Clear();
                foreach (string sName in sSubKeys)
                {
                    string sValue = (string)keyCom.GetValue(sName);
                    comboBox1.Items.Add(sValue);
                }
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            comboBox2.Text = "115200";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// 车门上锁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue)
            {
                mode = 0x00;
                istrue= false;
            }
            else
            {
                mode = 0x00;
                istrue = true;
            }
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 4门解锁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue)
            {
                mode = 0x01;
                istrue = false;
            }
            else
            {
                mode = 0x00;
                istrue = true;
            }
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 中控上锁车门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue)
            {
                mode = 0x01;
                istrue = false;
            }
            else
            {
                mode = 0x00;
                istrue = true;
            }
            Byte[] buf = { 0x15, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 解锁驾驶门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue)
            {
                mode = 0x02;
                istrue = false;
            }
            else
            {
                mode = 0x00;
                istrue = true;
            }
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 解锁后背箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue)
            {
                mode = 0x02;
                istrue = false;
            }
            else
            {
                mode = 0x00;
                istrue = true;
            }
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }
    }
}
