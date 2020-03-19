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
    public partial class AutomativeLighting : Form
    {
        private SerialPort serialPort = new SerialPort();

        public AutomativeLighting()
        {
            InitializeComponent();//这是我加的新注释
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }
        bool isOpened = false;//串口状态标志
        private bool istrue = true, istrue2 = true, istrue3 = true, istrue4 = true, istrue5 = true, istrue6 = true, istrue7 = true,
           istrue8 = true, istrue9 = true, istrue10 = true, istrue11 = true, istrue12 = true, istrue13 = true, istrue14 = true,
           istrue15 = true, istrue16 = true, istrue17 = true, istrue18 = true, istrue19 = false;
    
        private void AutomativeLighting_Load(object sender, EventArgs e)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBuf"></param>
        /// <param name="len"></param>
        private void MakeAndSendToDriver(Byte[] pBuf, int len)
        {
            int iSendLen = 4;
            Byte[] pSendBuf = new Byte[iSendLen];
            if (pSendBuf == null) return;

            pSendBuf[0] = 0xff;
            pSendBuf[1] = 0x55;
            pSendBuf[2] = (Byte)iSendLen;
            pSendBuf[3] = 0x01;

            List<byte> lTemp = new List<byte>();
            lTemp.AddRange(pSendBuf);
            lTemp.AddRange(pBuf);
            pSendBuf = new byte[lTemp.Count + 1];
            lTemp.CopyTo(pSendBuf);
            //           len  mode cmd  param checksum
            //0xff,0x55,0x07,0x01,0x15,0x01, 0x2d  
            CalculateCheckSum(pSendBuf, pSendBuf.Length); //计算校验和
            //调用串口发送函数
            //接收处理函数
            processandshow(pSendBuf, pSendBuf.Length);
        }
        /// <summary>
        /// 接收处理函数
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="len"></param>
        private void processandshow(Byte[] buff, int len)
        {
            switch (buff[4])//cmd
            {
                case 0x30:
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;///看以后这些灯定义的位是 bit几 先用着 bit0
                        if (bit0 == 1)//远光开
                        {
                            pictureBox5.Show();
                        }
                        else if (bit0 == 0)//远光关
                        {
                            pictureBox5.Hide();
                        }
                        //if (buff[5] == 0x01){pictureBox5.Show();} else { pictureBox5.Hide(); }
                    }
                    break;
                case 0x31:
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)//近光开
                        {
                            pictureBox3.Show();
                        }
                        else if (bit0 == 0)//近光关
                        {
                            pictureBox3.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox3.Show(); } else { pictureBox3.Hide(); }
                    }
                    break;
                case 0x32://雾灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox6.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox6.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox6.Show(); } else { pictureBox6.Hide(); }
                    }
                    break;
                case 0x33://转向灯
                    {

                        if (buff[5] == 0x01) { pictureBox8.Show(); } else { pictureBox8.Hide(); }
                    }
                    break;
                case 0x34://倒车灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox10.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox10.Hide();
                        }
                        // if (buff[5] == 0x01) { pictureBox10.Show(); } else { pictureBox10.Hide(); }
                    }
                    break;
                case 0x35://刹车
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox11.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox11.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox11.Show(); } else { pictureBox11.Hide(); }
                    }
                    break;
                case 0x36://示宽灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox7.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox7.Hide();
                        }

                        // if (buff[5] == 0x01){pictureBox7.Show();}else { pictureBox7.Hide(); }
                    }
                    break;
                case 0x37://后备箱灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox12.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox12.Hide();
                        }

                        //if (buff[5] == 0x01) { pictureBox12.Show(); } else { pictureBox12.Hide(); }
                    }
                    break;
                case 0x38:
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox19.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox19.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox19.Show(); } else { pictureBox19.Hide(); }
                    }
                    break;
                case 0x39:
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox18.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox18.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox18.Show(); } else { pictureBox18.Hide(); }
                    }
                    break;
                case 0x3A://储物箱
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox17.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox17.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox17.Show(); } else { pictureBox17.Hide(); }
                    }
                    break;
                case 0x3B://制动灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox16.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox16.Hide();
                        }
                        // if (buff[5] == 0x01) { pictureBox16.Show(); } else { pictureBox16.Hide(); }
                    }
                    break;
                case 0x3C://行车灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox16.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox16.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox16.Show(); } else { pictureBox16.Hide(); }
                    }
                    break;
                case 0x3D://脚灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox15.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox15.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox15.Show(); } else { pictureBox15.Hide(); }
                    }
                    break;
                default:
                    break;
            }
        }

        //中控锁命令 0解锁 1上锁
        private void Set_Light(byte bMode)
        {
            Byte[] buf = { 0x15, bMode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 求和校验
        /// </summary>
        /// <param name="pBuf"></param>
        /// <param name="len"></param>
        private void CalculateCheckSum(Byte[] pBuf, int len)
        {
            //assert(pBuf != NULL);
            byte CheckSum = 0;
            for (int i = 3; i < (len - 1); i++)
            {
                CheckSum += pBuf[i];
            }
            pBuf[len - 1] = CheckSum;
        }
        private void label8_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOPen_click(object sender, EventArgs e)
        {
            openCom();
        }
        /// <summary>
        /// 
        /// </summary>
        public void openCom() {
            if (!isOpened)
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
                serialPort.PortName = comboBox1.Text;
                serialPort.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
                try
                {
                    serialPort.Open();     //打开串口
                    button10.Text = "关闭串口";
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
                    button10.Text = "打开串口";
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
            serialPort.Read(ReDatas, 0, ReDatas.Length);//
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
                                    Console.WriteLine("checkdata:" + sb.ToString());

                                    StringBuilder sb2 = new StringBuilder();
                                    for (int k = 0; k < data.Length; k++)
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
                case 0x30://远光
                    {
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox5.Show();
                            pictureBox26.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox5.Hide();
                            pictureBox26.Hide();
                        } 
                    }
                    break;
                case 0x31://近光
                    {
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox3.Show();
                            pictureBox28.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox3.Hide();
                            pictureBox28.Hide();
                        }
                    }
                    break;
                case 0x32://雾灯
                    {
                        //pictureBox13.Show();
                        if (byteArray[5] == 0x01)//前雾灯   开/关
                        {
                            pictureBox6.Show();
                            pictureBox25.Show();
                            pictureBox13.Show();
                            pictureBox21.Show();

                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox6.Hide();
                            pictureBox25.Hide();
                            pictureBox13.Hide();
                            pictureBox21.Hide ();
                        }
                        //后雾灯
                    }
                    break;
                case 0x33://转向
                    {
                        if (byteArray[5] == 0x01)//左转向灯
                        {
                            pictureBox8.Show();
                            pictureBox9.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox8.Hide();
                            pictureBox9.Hide();
                        }
                        //右转向灯
                    }
                    break;
                case 0x34://倒车
                    {
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox10.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox10.Hide();
                        }
                    }
                    break;
                case 0x35://刹车
                    {
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox11.Show();
                            pictureBox27.Show();

                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox11.Hide();
                            pictureBox27.Hide();
                        }
                    }
                    break;
                case 0x36://示宽/牌照
                    {
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox7.Show();
                            pictureBox24.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox7.Hide();
                            pictureBox24.Hide();
                        }
                    }
                    break;
                case 0x37://后备箱灯
                    {
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox12.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox12.Hide();
                        }
                    }
                    break;
                case 0x38://阅读灯
                    {
                        if (byteArray[5] == 0x01)
                        {
                            pictureBox19.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox19.Hide();
                        }
                    }
                    break;
                case 0x39://化妆灯
                    {
                        if (byteArray[5] == 0x01)//左前
                        {
                            pictureBox18.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox18.Hide();
                        }
                    }
                    break;
                case 0x3A://储物箱
                    {
                        if (byteArray[5] == 0x01)//
                        {
                            pictureBox17.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox17.Hide();
                        }
                    }
                    break;
                case 0x3B: //制动
                    {
                        if (byteArray[5] == 0x01)//
                        {
                            pictureBox11.Show();
                            pictureBox27.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox11.Hide();
                            pictureBox27.Hide();
                        }
                    }
                    break;
                case 0x3C: //日间
                    {
                        if (byteArray[5] == 0x01)//
                        {
                            pictureBox16.Show();
                            pictureBox22.Show();
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox16.Hide();
                            pictureBox22.Hide();
                        }
                    }
                    break;
                case 0x3D: //脚灯
                    {
                        if (byteArray[5] == 0x01)//
                        {
                            pictureBox15.Show();//左脚
                            pictureBox20.Show();//右脚
                        }
                        else if (byteArray[5] == 0x00)
                        {
                            pictureBox15.Hide();
                            pictureBox20.Hide();
                        }
                    }
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 字符串转换16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }
       
        /// <summary
        /// 远光灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Light_open(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue)
            {
                mode= 0x01;
                istrue = false;
            }
            else
            {
                mode = 0x00;
                istrue = true;
            }
            Byte[] buf = {0x30, mode };
            MakeAndSendToDriver(buf, 1);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBuf"></param>
        /// <param name="len"></param>
        
        /// <summary>
        /// 示宽灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_shiKuang_open(object sender, EventArgs e)
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
            Byte[] buf = { 0x36, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 近光灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x31, mode };
            MakeAndSendToDriver(buf, 1);
        }
        /// <summary>
        /// 雾灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x32, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 危险报警灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        ///左转向灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x33, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 倒车灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue6)
            {
                mode = 0x01;
                istrue6 = false;
            }
            else
            {
                mode = 0x00;
                istrue6 = true;
            }
            Byte[] buf = { 0x34, mode };
            MakeAndSendToDriver(buf, 1);
        }
        /// <summary>
        ///刹车灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x35, mode };
            MakeAndSendToDriver(buf, 1);
        }
        /// <summary>
        /// 后备箱灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            bool istrue = true;
            byte mode = 0;
            if (istrue8)
            {
                mode = 0x01;
                istrue8 = false;
            }
            else
            {
                mode = 0x00;
                istrue8 = true;
            }
            Byte[] buf = { 0x37, mode };
            MakeAndSendToDriver(buf, 1);
        }
        /// <summary>
        /// 左阅读灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x38, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 右阅读灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x38, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 0x39
        /// 化妆灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue11)
            {
                mode = 0x01;
                istrue11 = false;
            }
            else
            {
                mode = 0x00;
                istrue11 = true;
            }
            Byte[] buf = { 0x39, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 0x3A
        /// 储物箱灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue12)
            {
                mode = 0x01;
                istrue12 = false;
            }
            else
            {
                mode = 0x00;
                istrue12 = true;
            }
            Byte[] buf = { 0x3A, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 0x3B
        /// 制动灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue13)
            {
                mode = 0x01;
                istrue13 = false;
            }
            else
            {
                mode = 0x00;
                istrue13 = true;
            }
            Byte[] buf = { 0x3B, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 0x3C
        /// 日间行车灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue14)
            {
                mode = 0x01;
                istrue14 = false;
            }
            else
            {
                mode = 0x00;
                istrue14 = true;
            }
            Byte[] buf = { 0x3C, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 左脚灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue15)
            {
                mode = 0x01;
                istrue15 = false;
            }
            else
            {
                mode = 0x00;
                istrue15 = true;
            }
            Byte[] buf = { 0x3D, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 右脚灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue16)
            {
                mode = 0x01;
                istrue16 = false;
            }
            else
            {
                mode = 0x00;
                istrue16 = true;
            }
            Byte[] buf = { 0x3D, mode };
            MakeAndSendToDriver(buf, 2);
        }
        /// <summary>
        /// 右转向灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue17)
            {
                mode = 0x01;
                istrue17 = false;
            }
            else
            {
                mode = 0x00;
                istrue17 = true;
            }
            Byte[] buf = { 0x33, mode };
            MakeAndSendToDriver(buf, 2);
        }
    }
}
    