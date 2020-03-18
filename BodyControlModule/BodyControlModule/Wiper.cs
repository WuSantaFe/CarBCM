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
    public partial class Wiper : Form
    {
        private SerialPort serialPort = new SerialPort();

        public Wiper()
        {
            InitializeComponent();
            //serialPort.DataReceived += new SerialDataReceivedEventHandler(post_DataReceived);//串口接收处理函数
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }
        bool isOpened = false;//串口状态标志
        private bool istrue = true, istrue2 = true, istrue3 = true, istrue4 = true, istrue5 = true, istrue6 = true, istrue7 = true,
         istrue8 = true, istrue9 = true, istrue10 = true, istrue11 = true, istrue12 = true, istrue13 = true, istrue14 = true,
         istrue15 = true, istrue16 = true, istrue17 = true, istrue18 = true, istrue19 = false;

        private void button9_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// bool isOpened = false;//串口状态标志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (!isOpened)
            {
                serialPort.PortName = comboBox1.Text;
                serialPort.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
                try
                {
                    serialPort.Open();     //打开串口
                    button7.Text = "关闭串口";
                    comboBox1.Enabled = false;//关闭使能
                    comboBox2.Enabled = false;
                    isOpened = true;
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(post_DataReceived);//串口接收处理函数
                }
                catch
                {
                    MessageBox.Show("串口打开失败！");
                }
            }
            else
            {
                try
                {
                    serialPort.Close();     //关闭串口
                    button7.Text = "打开串口";
                    comboBox1.Enabled = true;//打开使能
                    comboBox2.Enabled = true;
                    isOpened = false;
                }
                catch
                {
                    MessageBox.Show("串口关闭失败！");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //发送数据
            if (serialPort.IsOpen)
            {//如果串口开启
                if (textBox2.Text.Trim() != "")//如果框内不为空则
                {
                    serialPort.Write(textBox2.Text.Trim());//写数据
                }
                else
                {
                    MessageBox.Show("发送框没有数据");
                }
            }
            else
            {
                MessageBox.Show("串口未打开");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            byte[] ReDatas = new byte[serialPort.BytesToRead];
            Console.WriteLine(serialPort.BytesToRead);
            serialPort.Read(ReDatas, 0, ReDatas.Length);//读取数据
            ProcessShowData(ReDatas);
        }

        private void ProcessShowData(byte[] data)
        {
            this.BeginInvoke(new MethodInvoker(delegate
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
        private void show(byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byteArray.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", byteArray[i]);
            }
            Console.WriteLine(sb.ToString());
            switch (byteArray[4])
            {
                case 0x12:
                    {
                        //雨刮   开/关
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox6.Show();
                            pictureBox19.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox6.Hide();
                            pictureBox19.Hide();
                        }
                        // 间歇循环 开/关
                        if (byteArray[7] == 0x02)
                        {
                            pictureBox7.Show();
                        }
                        else if (byteArray[7] == 0x00)
                        {
                            pictureBox7.Hide();
                        }
                        // 高速雨刮 开/关
                        if (byteArray[7] == 0x02)//应该不是第7位
                        {
                            pictureBox4.Show();
                        }
                        else if (byteArray[7] == 0x00)
                        {
                            pictureBox4.Hide();
                        }
                    }
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
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
        /// <summary>
        /// 接收处理函数
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="len"></param>
        private void ProcessAndShow(Byte[] buff, int len, object sender)
        {
            Button button = (Button)sender;
            switch (buff[4])
            {
                case 0x12:
                    {
                        if (button.Name.Equals("button1"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//雨刮摆动一次
                            {
                                pictureBox6.Show();
                            }
                            else if (bit0 == 0)
                            {
                                pictureBox6.Hide();
                            }
                        }
                        if (button.Name.Equals("button2"))
                        {
                            int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 1)//间歇摆动
                            {
                                pictureBox7.Show();
                            }
                            else if (bit1 == 0)
                            {
                                pictureBox7.Hide();
                            }
                        }
                        if (button.Name.Equals("button4"))
                        {
                            int bit2 = (buff[5] & 0x03) == 0x03 ? 1 : 0;
                            if (bit2 == 1)//中速摆动
                            {
                                pictureBox5.Show();
                            }
                            else if (bit2 == 0)
                            {
                                pictureBox5.Hide();
                            }
                        }
                        if (button.Name.Equals("button5"))
                        {
                            int bit3 = (buff[5] & 0x04) == 0x04 ? 1 : 0;
                            if (bit3 == 1)//高速摆动
                            {
                                pictureBox4.Show();
                            }
                            else if (bit3 == 0)
                            {
                                pictureBox4.Hide();
                            }
                        }
                        if (button.Name.Equals("button6"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//玻璃洗涤
                            {
                                pictureBox19.Show();
                            }
                            else if (bit0 == 0)
                            {
                                pictureBox19.Hide();
                            }
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
        private void CalculateCheckSum(byte[] BuffData, int len)
        {
            if (BuffData == null) return;
            int CheckSum = 0;
            for (int i = 3; i < (len - 1); i++)
            {
                CheckSum += BuffData[i];
            }
            BuffData[len - 1] = (byte)CheckSum;
        }
        /// <summary>
        /// 摆动一次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 间歇摆动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue2)
            {
                mode = 0x02;
                istrue2 = false;
            }
            else
            {
                mode = 0x00;
                istrue2 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 中速摆动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue2)
            {
                mode = 0x02;
                istrue2 = false;
            }
            else
            {
                mode = 0x00;
                istrue2 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 高速摆动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue3)
            {
                mode = 0x03;
                istrue3 = false;
            }
            else
            {
                mode = 0x00;
                istrue3 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 玻璃洗涤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue4)
            {
                mode = 0x01;
                istrue4 = false;
            }
            else
            {
                mode = 0x00;
                istrue4 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }

        private void Wiper_Load(object sender, EventArgs e)
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
    }
}
