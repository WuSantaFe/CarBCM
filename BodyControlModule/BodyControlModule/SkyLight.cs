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
    public partial class SkyLight : Form
    {
        private SerialPort serialPort = new SerialPort();
        bool isOpened = false;//串口状态标志

        public SkyLight()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }
        private bool istrue = true, istrue2 = true, istrue3 = true, istrue4 = true, istrue5 = true, istrue6 = true, istrue7 = true,
         istrue8 = true, istrue9 = true, istrue10 = true, istrue11 = true, istrue12 = true, istrue13 = true, istrue14 = true,
         istrue15 = true, istrue16 = true, istrue17 = true, istrue18 = true, istrue19 = false;

        private void button20_Click(object sender, EventArgs e)
        {
            if (!isOpened)
            {
                serialPort.PortName = comboBox1.Text;
                serialPort.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
                try
                {
                    serialPort.Open();     //打开串口
                    button20.Text = "关闭串口";
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
                    button20.Text = "打开串口";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[serialPort.BytesToRead];
            serialPort.Read(ReDatas, 0, ReDatas.Length);//读取数据
            ProcessShowData(ReDatas);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
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
                case 0x13:
                    {
                        //天窗  开/关
                        int bit0 = (byteArray[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox19.Show();
                        }
                        else if (bit0 == 0) 
                        {
                            pictureBox19.Hide();
                        }

                        //天窗起翘 /开/关
                        int bit1 = (byteArray[5] & 0x02) == 0x02 ? 1 : 0;
                        if (bit1 == 1)
                        {
                            pictureBox6.Show();
                        }
                        else if (bit1 == 0)
                        {
                            pictureBox6.Hide();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        private void SkyLight_Load(object sender, EventArgs e)
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
                case 0x13:
                    {
                        if (button.Name.Equals("button1"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//开启天窗
                            {
                                pictureBox19.Show();
                            }
                            else
                            {
                                MessageBox.Show("未触发","提示");
                            }
                        }
                        if (button.Name.Equals("button2"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 0)//关闭天窗
                            {
                                pictureBox19.Hide();
                            }
                        }
                        if (button.Name.Equals("button5"))
                        {
                            int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 ==1 )//起翘天窗
                            {
                                pictureBox6.Show();
                            }
                        }
                        if (button.Name.Equals("button6"))
                        {
                            int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 0)//关闭起翘天窗
                            {
                                pictureBox6.Hide();
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
        /// <summary>
        /// 关闭起翘天窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue4)
            {
                mode = 0x02;
                istrue4 = false;
            }
            else
            {
                mode = 0x00;
                istrue4 = true;
            }
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 起翘天窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue3)
            {
                mode = 0x01;
                istrue3 = false;
            }
            else
            {
                mode = 0x00;
                istrue3 = true;
            }
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 关闭天窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue2)
            {
                mode = 0x01;
                istrue2 = false;
            }
            else
            {
                mode = 0x00;
                istrue2 = true;
            }
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }

        /// <summary>
        /// 开启天窗
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
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }
    }

}
