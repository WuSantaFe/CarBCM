using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using Microsoft.Win32;

namespace BodyControlModule
{
    public partial class CarWindow : Form
    {

        private SerialPort serialPort = new SerialPort();
        bool isOpened = false;//串口状态标志

        private bool istrue = true, istrue2 = true, istrue3 = true, istrue4 = true, istrue5 = true, istrue6 = true, istrue7 = true,
          istrue8 = true, istrue9 = true, istrue10 = true, istrue11 = true, istrue12 = true, istrue13 = true, istrue14 = true,
          istrue15 = true, istrue16 = true, istrue17 = true, istrue18 = true, istrue19 = false;

        public CarWindow()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            Console.WriteLine(serialPort.BytesToRead);
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
                                    StringBuilder sb = new StringBuilder();
                                    for (int j = 0; j < checkdata.Length; j++)
                                    {
                                        sb.AppendFormat("{0:x2}" + " ", checkdata[j]);
                                    }
                                    Console.WriteLine("checkdata:"+sb.ToString());

                                    StringBuilder sb2 = new StringBuilder();
                                    for (int k= 0; k < data.Length; k++)
                                    {
                                        sb2.AppendFormat("{0:x2}" + " ", data[k]);
                                    }
                                    Console.WriteLine("data:" + sb2.ToString());
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
                case 0x10:
                    {
                        //左前车窗
                        int bit0 = (byteArray[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)//左前车窗未闭合
                        {
                            pictureBox1.Show();
                        }
                        else if (bit0 == 0) //左前车窗关闭
                        {
                            pictureBox1.Hide();
                        }
                        //右前车窗
                        int bit1 = (byteArray[5] & 0x02) == 0x02 ? 1 : 0;
                        if (bit1 == 1)//右前车窗未闭合
                        {
                            pictureBox12.Show();
                        }
                        else if (bit1 == 0)//右前车窗关闭
                        {
                            pictureBox12.Hide();
                        }
                        //左后车窗
                        int bit2 = (byteArray[5] & 0x03) == 0x03 ? 1 : 0;
                        if (bit2 == 1)//左后车窗未闭合
                        {
                            pictureBox13.Show();
                        }
                        else if (bit2 == 0) // 左后车窗关闭
                        {
                            pictureBox13.Hide();
                        }
                        //右后车窗
                        int bit3 = (byteArray[5] & 0x04) == 0x04 ? 1 : 0;
                        if (bit3 == 1)//右后车窗未闭合
                        {
                            pictureBox10.Show();
                        }
                        else if (bit3 == 0) //右后车窗关闭
                        {
                            pictureBox10.Hide();
                        }
                    }
                    break;
                case 0x11:
                    {
                        //后视镜折叠  开/关
                        int bit0 = (byteArray[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox14.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox14.Hide();
                        }
                        //后视镜加热 开/关
                        int bit1 = (byteArray[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit1 == 1)
                        {
                            pictureBox4.Show();
                        }
                        else if (bit1 == 0)
                        {
                            pictureBox4.Hide();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        private void SendToPort(Byte[] BuffData,int len,object sender)
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

            CalculateCheckSum(SendBuff,SendBuff.Length);//求和校验
            ProcessAndShow(SendBuff,SendBuff.Length,sender);//处理数据
        }
        /// <summary>
        /// 接收处理函数
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="len"></param>
        private void ProcessAndShow(Byte[] buff, int len, object sender)
        {
            Button button = (Button)sender;
            switch (buff[4])//cmd
            {
                case 0x10://车窗
                    {
                                      
                        byte buff5 = buff[5];
                        if (button.Name.Equals("button2"))
                        {
                            int bit0 = (buff5 & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//左前车窗未闭合(下降)
                            {
                                pictureBox2.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手","提示");
                            }
                        }
                        if (button.Name.Equals("button1"))
                        {
                            int bit0 = (buff5 & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 0)//左前车窗闭合（上升）
                            {
                                pictureBox1.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }


                        if (button.Name.Equals("button10"))
                        {
                            int bit1 = (buff5 & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 1)//右前车窗未闭合（下降）
                            {
                               pictureBox11.Show();
                            }
                            else 
                            {
                                MessageBox.Show("松手","提示");
                            }
                        }
                        if (button.Name.Equals("button11"))
                        {
                            int bit1 = (buff5 & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 0)//右前车窗关闭（上升）
                            {
                                pictureBox12.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }

                        if (button.Name.Equals("button12"))
                        {
                            int bit2 = (buff5 & 0x03) == 0x03 ? 1 : 0;
                            if (bit2 == 1)//左后车窗未关闭（下降）
                            {
                                pictureBox9.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }
                        if (button.Name.Equals("button13"))
                        {
                            int bit2 = (buff5 & 0x03) == 0x03 ? 1 : 0;
                            if (bit2 == 0)//左后车窗关闭（上升）
                            {
                                pictureBox13.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }

                        if (button.Name.Equals("button14"))
                        {
                            int bit3 = (buff5 & 0x04) == 0x04 ? 1 : 0;
                            if (bit3 == 1)//右后车窗未关闭（下降）
                            {
                                pictureBox5.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }
                        if (button.Name.Equals("button15"))
                        {
                            int bit3 = (buff5 & 0x04) == 0x04 ? 1 : 0;
                            if (bit3 == 0)//右后车窗关闭（上升）
                            {
                                pictureBox13.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }
                    }
                    break;
                case 0x11://后视镜
                    {
                        if (button.Name.Equals("button23"))
                        {
                            int bit1= (buff[5] & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 ==1 )//后视镜加热
                            {
                                pictureBox4.Show();
                            }
                            else if (bit1 ==0 )
                            {
                                pictureBox4.Hide();
                            }         
                        }
                        if (button.Name.Equals("button22"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//后视镜折叠
                            {
                                pictureBox14.Show();
                            }
                            else if (bit0 == 0)
                            {
                                pictureBox14.Hide();
                            }
                        }
                        if (button.Name.Equals("button7"))
                        {
                            pictureBox3.Show();
                        }
                        if (button.Name.Equals("button8"))
                        {
                            pictureBox15.Show();
                        }
                    }
                    break;
                case 0x12:
                    {

                    }
                    break;
                case 0x13:
                    {

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
        /// 左前车窗上升
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
            Byte[] buf = { 0x10, mode };
            SendToPort(buf, 1,sender);

        }
        /// <summary>
        /// 左前车窗下降
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
            Byte[] buf = { 0x10, mode,};
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 上升右前车窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x10, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 下降右前车窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x10, mode };
            SendToPort(buf, 1, sender);
        }
        /// <summary>
        /// 上升左后车窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue5)
            {
                mode = 0x01;
                istrue5 = false;
            }
            else
            {
                mode = 0x00;
                istrue5 = true;
            }
            Byte[] buf = { 0x10, mode };
            SendToPort(buf, 1,sender);
        }
        /// <summary>
        /// 下降左后车窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue6)
            {
                mode = 0x02;
                istrue6 = false;
            }
            else
            {
                mode = 0x00;
                istrue6 = true;
            }
            Byte[] buf = { 0x10, mode };
            SendToPort(buf, 1,sender);
        }
        /// <summary>
        /// 上升右后车窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue7)
            {
                mode = 0x01;
                istrue7 = false;
            }
            else
            {
                mode = 0x00;
                istrue7 = true;
            }
            Byte[] buf = { 0x10, mode };
            SendToPort(buf, 1,sender);
        }
        /// <summary>
        /// 下降右后车窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue8)
            {
                mode = 0x02;
                istrue8 = false;
            }
            else
            {
                mode = 0x00;
                istrue8 = true;
            }
            Byte[] buf = { 0x10, mode };
            SendToPort(buf, 1,sender);
        }
        /// <summary>
        /// 左后视镜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            byte mode = 0x00;
            Byte[] buf = { 0x11, mode };
            SendToPort(buf, 1,sender);
        }
        /// <summary>
        /// 右后视镜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            byte mode = 0x01;
            Byte[] buf = { 0x11, mode };
            SendToPort(buf, 1,sender);
        }
        /// <summary>
        /// 后视镜折叠/打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button22_Click(object sender, EventArgs e)
        {

            byte mode = 0;
            if (istrue9)
            {
                mode = 0x01;
                istrue9 = false;
            }
            else
            {
                mode = 0x00;
                istrue9 = true;
            }
            Byte[] buf = { 0x11, mode };
            SendToPort(buf, 1,sender);
        }
        /// <summary>
        /// 后视镜打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button21_Click(object sender, EventArgs e)
        {
            byte mode = 0x00;
            Byte[] buf = { 0x11, mode };
            //SendToPort(buf, 1);
        }

        private void CarWindow_Load(object sender, EventArgs e)
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
        private void button18_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 后视镜加热
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button23_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue10)
            {
                mode = 0x01;
                istrue10 = false;
            }
            else
            {
                mode = 0x00;
                istrue10 = true;
            }
            Byte[] buf = { 0x11, mode };
            SendToPort(buf, 1, sender);
        }
    }
}
