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
    public partial class BcmMain : Form
    {
        System.Timers.Timer tmr = null;
        const int RECVDATAMAX = 2048;
        const int AUTOCLEARTIME = 2000;
        const int WAITRECVTIME = 180;
        int VerifyDataLength = 0;
        int ReDataBuffPos = 0;
        byte[] ReDatasBuff = new byte[RECVDATAMAX];
        byte[] RecvData = null;
        SerialPorCommon spc = new SerialPorCommon();
        private SerialPort serialPort = new SerialPort();
        bool isOpened = false;//串口状态标志
        private bool istrue   = true, istrue2  = true, istrue3  = true, istrue4  = true, istrue5  = true, istrue6  = true, istrue7  = true,
                     istrue8  = true, istrue9  = true, istrue10 = true, istrue11 = true, istrue12 = true, istrue13 = true, istrue14 = true,
                     istrue15 = true, istrue16 = true, istrue17 = true, istrue18 = true, istrue19 = true, istrue20 = true, istrue21 = true,
                     istrue22 = true, istrue23 = true, istrue24 = true;
        public BcmMain()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }
        #region 初始化
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BcmMain_Load(object sender, EventArgs e)
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
            //Byte[] bBig = { 0xFF, 0x55 } ;
            //Byte[] bSmall = {0x05, 0x01, 0xF1 };
            //List<byte> lTemp = new List<byte>();
            //lTemp.AddRange(bBig);
            //lTemp.AddRange(bSmall);
            //bBig = new byte[lTemp.Count];
            //lTemp.CopyTo(bBig);
            //Console.WriteLine(bBig);
            //Console.WriteLine(bSmall);



        }

        private void light_click_open(object sender, EventArgs e)
        {
            //if (label1.Visible == true)
            //{
            //    label1.Visible = false;
            //}
            //else if (label1.Visible == false)
            //{
            //    label1.Visible = true;
            //}
            AutomativeLighting atl = new AutomativeLighting();
            atl.Show();
            //this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void exit_click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CarWindow_Click(object sender, EventArgs e)
        {
            CarWindow cw = new CarWindow();
            cw.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            KeyLessStartup kls = new KeyLessStartup();
            kls.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Wiper wiper = new Wiper();
            wiper.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SkyLight sl = new SkyLight();
            sl.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics,
                               this.panel1.ClientRectangle,
                               Color.LightSeaGreen,//7f9db9
                               1,
                               ButtonBorderStyle.Solid,
                               Color.LightSeaGreen,
                               1,
                               ButtonBorderStyle.Solid,
                               Color.LightSeaGreen,
                               1,
                               ButtonBorderStyle.Solid,
                               Color.LightSeaGreen,
                               1,
                               ButtonBorderStyle.Solid);
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics,
                               this.groupBox1.ClientRectangle,
                               Color.LightSeaGreen,//7f9db9
                               1,
                               ButtonBorderStyle.Solid,
                               Color.LightSeaGreen,
                               1,
                               ButtonBorderStyle.Solid,
                               Color.LightSeaGreen,
                               1,
                               ButtonBorderStyle.Solid,
                               Color.LightSeaGreen,
                               1,
                               ButtonBorderStyle.Solid);
        }
       
        private void label3_Paint(object sender, PaintEventArgs e)
        {

        }
        #endregion
        #region 串口返回 开始 

        private void click_openCom(object sender, EventArgs e)
        {
            openCom();
        }
        private void openCom()
        {
            if (!isOpened)
            {
                serialPort.PortName = comboBox1.Text;
                serialPort.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
                try
                {
                    serialPort.Open();     //打开串口
                    button12.Text = "关闭串口";
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
                    button12.Text = "打开串口";
                    comboBox1.Enabled = true;//打开使能
                    comboBox2.Enabled = true;
                    isOpened = false;
                    RefreshDataProcessStatus("");
                }
                catch
                {
                    MessageBox.Show("串口关闭失败！");
                }
            }
        }

        private void ClearStatus(object source, System.Timers.ElapsedEventArgs e)
        {
            RefreshDataProcessStatus("");
            StopTimer(null != tmr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(WAITRECVTIME);
            // 串口接收数据
            byte[] ReDatas = new byte[serialPort.BytesToRead];
            //Console.WriteLine(serialPort.BytesToRead+"++++++++++++++++++++++");
            serialPort.Read(ReDatas, 0, ReDatas.Length);
            // 串口数据收到的一个字节0xff会被解析成0x30{0} 0x78{x} 0x66{f} 0x66{f}，需要重新将其转成0xff再进行匹配
            byte[] changedata = HexStrTobyte(new ASCIIEncoding().GetString(ReDatas));
            changedata.CopyTo(ReDatasBuff, ReDataBuffPos);
            VerifyDataLength += changedata.Length;
            // 出现一帧数据中某一字节接收导致解析错误，丢弃数据重新匹配，例如0xbc，被分开接收成0x0b及0x0c，则重新开始从0xff开始寻找匹配，0xff之前的数据则丢弃
            if (VerifyDataLength > 32)
            {
                for (int i = 0; i < VerifyDataLength; i++)
                {
                    if(i!=0 && 0xff == ReDatasBuff[i])
                    {
                        byte[] temp = new byte[VerifyDataLength - i];
                        Array.Copy(ReDatasBuff, i, temp, 0, (VerifyDataLength - i));
                        Array.Clear(ReDatasBuff, 0, RECVDATAMAX);
                        temp.CopyTo(ReDatasBuff, 0);
                        VerifyDataLength -= i;
                        RefreshDataProcessStatus("====出现错误数据，重新从0xff开始匹配====");
                        //Console.WriteLine("====出现错误数据，重新从0xff开始匹配====");
                        break;
                    }
                }
            }
            // 匹配数据，收到的数据可能不是完整的一帧，匹配到完整的一帧数据才送去做状态显示处理，否则重新累加再进行匹配
            if (VerifyRecvDataCompelete(ReDatasBuff, VerifyDataLength, out RecvData))
            {
                if(VerifyDataLength > RecvData.Length)
                {
                    byte[] temp = new byte[VerifyDataLength - RecvData.Length];
                    Array.Copy(ReDatasBuff, VerifyDataLength, temp, 0, (VerifyDataLength - RecvData.Length));
                    Array.Clear(ReDatasBuff, 0, RECVDATAMAX);
                    temp.CopyTo(ReDatasBuff, 0);
                }
                else
                {
                    Array.Clear(ReDatasBuff, 0, RECVDATAMAX);
                }
                ReDataBuffPos -= RecvData.Length;
                VerifyDataLength -= RecvData.Length;
                if (ReDataBuffPos < 0) ReDataBuffPos = 0;
                if (VerifyDataLength < 0) VerifyDataLength = 0;

                this.ProcessShowData(RecvData);
                //this.AddData(ReDatas);  // 串口数据输出，调试时可使用
            }
            else
            {
                ReDataBuffPos += changedata.Length;
            }
        }

        private void StopTimer(bool isRuning)
        {
            if (isRuning)
            {
                tmr.Stop();
                tmr = null;
            }
            else
            {
                tmr = null;
            }
        }

        private void TimerClearStatus()
        {
            StopTimer(null != tmr);

            tmr = new System.Timers.Timer(AUTOCLEARTIME);
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(ClearStatus);
            tmr.AutoReset = false;
            tmr.Enabled = true;
        }

        public void AddData(byte[] data)
        {
            AddContent(new ASCIIEncoding().GetString(data));
        }

        private void AddContent(string content)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                this.textBox2.AppendText(content+"\r\n");
                this.textBox2.ScrollToCaret();
            }));
            

        }

        private byte[] HexStrTobyte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }

        private bool VerifyRecvDataCompelete(byte[] data, int datalen, out byte[] checkdata)
        {
            // CMD 0x01-点火/电压/电流  0x02-车门/车锁/车窗  0x03-雨刮/洗涤/后视镜  0x04-车灯
            // HEAD   HEAD  LEN   MODE1 MODE2 CMD  param  param param param param param param
            // {0xff, 0x55, 0x00, 0xF3, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
            bool bRet = false;
            checkdata = new byte[datalen];
            int pos = 0;
            int len = 0;
            for (int i = 0; i < datalen; i++)
            {
                checkdata[pos] = data[i];
                pos++;
                //Console.WriteLine("checkdata[{0:d}]={1:x}  data[{2:d}]={3:x}",i, checkdata[pos - 1], i, data[i]);
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
                                    RefreshDataProcessStatus("====校验和出错====");
                                    //Console.WriteLine("====校验和出错====");
                                    break;
                                }
                                RefreshDataProcessStatus("====数据解析成功====");
                                TimerClearStatus();
                                //Console.WriteLine("====数据解析成功====");
                                bRet = true;
                                return bRet;
                            }
                            else
                            {
                                RefreshDataProcessStatus("====数据长度不匹配====");
                                //Console.WriteLine("====数据长度不匹配====");
                            }
                        }
                        break;
                }
                    
            }
            return bRet;
        }

        private void RefreshDataProcessStatus(string str)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                label9.Text = str;
                label9.Refresh();
            }));
        }

        private void ProcessShowData(byte[] data)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                this.show(data);
            }));
        }

        private void show(byte[] byteArray)
        {
            switch (byteArray[5])//CMD
            {
                case 0x02://车窗、门锁、门
                    {
                        #region 车窗 开始 byteArray[6]--车窗状态
                        //左前车窗
                        int bit_0 = (byteArray[6] & 0x01) == 0x01 ? 1 : 0;
                        int bit_1 = (byteArray[6] & 0x02) == 0x02 ? 1 : 0;
                        if (bit_0 ==1 && bit_1 == 0)//上升中
                        {
                            ucLabel1.Show();
                        }
                        else if (bit_0 == 0 && bit_1 == 1) //下降中
                        {
                            ucLabel1.Hide();
                        }
                        else if (bit_0 == 1 && bit_1 == 1)//未闭合
                        {
                            ucLabel1.Show();
                        }
                        else//关闭
                        {
                            ucLabel1.Hide();
                        }
                        //右前车窗
                        int bit_2 = (byteArray[6] & 0x04) == 0x04 ? 1 : 0;
                        int bit_3 = (byteArray[6] & 0x08) == 0x08 ? 1 : 0;
                        if (bit_2 == 1 && bit_3 ==0 )//上升中
                        {
                            ucLabel3.Show();
                        }
                        else if (bit_2 == 0 && bit_3 == 1)//下降中
                        {
                            ucLabel3.Hide();
                        }
                        else if (bit_2 == 1 && bit_3 == 1)//未闭合
                        {
                            ucLabel3.Show();
                        }
                        else //关闭
                        {
                            ucLabel3.Hide();
                        }
                        //左后车窗
                        int bit_4 = (byteArray[6] & 0x10) == 0x10 ? 1 : 0;
                        int bit_5 = (byteArray[6] & 0x20) == 0x20 ? 1 : 0;

                        if (bit_4 == 1 && bit_5 == 0)//上升中
                        {
                            ucLabel5.Show();
                        }
                        else if (bit_4 == 0 && bit_5 == 1) // 下降中
                        {
                            ucLabel5.Hide();
                        }
                        else if (bit_4 == 0 && bit_5 == 1)//未闭合
                        {
                            ucLabel5.Show();
                        }
                        else//关闭
                        {
                            ucLabel5.Hide();
                        }
                        //右后车窗
                        int bit_6 = (byteArray[6] & 0x40) == 0x40 ? 1 : 0;
                        int bit_7 = (byteArray[6] & 0x80) == 0x80 ? 1 : 0;
                        if (bit_6 == 1 && bit_7 == 0)//上升中
                        {
                            ucLabel4.Show();
                        }
                        else if (bit_6 == 0 && bit_7 == 1) //下降中
                        {
                            ucLabel4.Hide();
                        }
                        else if (bit_6 == 0 && bit_7 == 1)//未闭合
                        {
                            ucLabel4.Show();
                        }
                        else//关闭
                        {
                            ucLabel4.Hide();
                        }
                        #endregion 车窗结束
                        #region 门锁 开始 byteArray[7]--锁状态
                        //左前门锁
                        int lock_bit_0 = (byteArray[7] & 0x01) == 0x01 ? 1 : 0;
                        if (lock_bit_0 == 1)//解锁
                        {
                           
                        }
                        else if (lock_bit_0 == 0) //上锁
                        {
                           
                        }
                        //右前门锁
                        int lock_bit_1 = (byteArray[7] & 0x02) == 0x02 ? 1 : 0;
                        if (lock_bit_1 == 1)//解锁
                        {

                        }
                        else if (lock_bit_1 == 0) //上锁
                        {

                        }
                        //左后门锁
                        int lock_bit_2 = (byteArray[7] & 0x04) == 0x04 ? 1 : 0;
                        if (lock_bit_2 == 1)//解锁
                        {

                        }
                        else if (lock_bit_2 == 0) //上锁
                        {

                        }
                        //右后门锁
                        int lock_bit_3 = (byteArray[7] & 0x08) == 0x08 ? 1 : 0;
                        if (lock_bit_3 == 1)//解锁
                        {

                        }
                        else if (lock_bit_3 == 0) //上锁
                        {

                        }
                        //后备箱锁
                        int lock_bit_4 = (byteArray[7] & 0x10) == 0x10 ? 1 : 0;
                        if (lock_bit_4 == 1)//解锁
                        {

                        }
                        else if (lock_bit_4 == 0) //上锁
                        {

                        }
                        //引擎盖锁
                        int lock_bit_5 = (byteArray[7] & 0x20) == 0x20 ? 1 : 0;
                        if (lock_bit_5 == 1)//解锁
                        {

                        }
                        else if (lock_bit_5 == 0) //上锁
                        {

                        }
                        #endregion 门锁结束
                        #region 门 开始 byteArray[8]--门状态
                        //左前车门
                        int door_bit_0 = (byteArray[8] & 0x01) == 0x01 ? 1 : 0;
                        if (door_bit_0 == 1)//左前门打开
                        {
                            pictureBox14.Show();
                        }
                        else if (door_bit_0 == 0) //左前门关闭
                        {
                            pictureBox14.Hide();
                        }
                        //右前车门
                        int door_bit_1 = (byteArray[8] & 0x02) == 0x02 ? 1 : 0;
                        if (door_bit_1 == 1)//右前门打开
                        {
                            pictureBox16.Show();
                        }
                        else if (door_bit_1 == 0)//右前门关闭
                        {
                            pictureBox16.Hide();
                        }
                        //左后门
                        int door_bit_2 = (byteArray[8] & 0x04) == 0x04 ? 1 : 0;
                        if (door_bit_2 == 1)//左后门打开
                        {
                            pictureBox15.Show();
                        }
                        else if (door_bit_2 == 0) //左后门关闭
                        {
                            pictureBox15.Hide();
                        }
                        //右后门
                        int door_bit_3 = (byteArray[8] & 0x08) == 0x08 ? 1 : 0;
                        if (door_bit_3 == 1)//右后门打开
                        {
                            pictureBox17.Show();
                        }
                        else if (door_bit_3 == 0) //右后门关闭
                        {
                            pictureBox17.Hide();
                        }
                        //后备箱
                        int door_bit_4 = (byteArray[8] & 0x10) == 0x10 ? 1 : 0;
                        if (door_bit_4 == 1)//后备箱打开
                        {
                            pictureBox46.Show();
                            pictureBox54.Show();
                        }
                        else if (door_bit_4 == 0) //后备箱关闭
                        {
                            pictureBox46.Hide();
                            pictureBox54.Show();
                        }
                        //引擎盖
                        int door_bit_5 = (byteArray[8] & 0x20) == 0x20 ? 1 : 0;
                        if (door_bit_5 == 1)//引擎盖打开
                        {
                            pictureBox53.Show();
                            pictureBox44.Show();
                        }
                        else if (door_bit_5 == 0) //引擎盖关闭
                        {
                            pictureBox53.Hide();
                            pictureBox44.Hide();
                        }
                        #endregion 门 结束
                    }
                    break;
                case 0x03://雨刮器、后视镜、天窗、洗涤器
                    {
                        #region 雨刮 后视镜 开始 byteArray[6]--雨刮、后视镜状态 byteArray[7]--雨刮开关和后视镜加热状态
                        //前雨刮位置
                        int yghsj_bit_0 = (byteArray[6] & 0x01) == 0x01 ? 1 : 0;
                        if (yghsj_bit_0 == 1)//未在初始位
                        {

                        }
                        else if (yghsj_bit_0 == 0)//初始位
                        {

                        }
                        //前雨刮状态
                        int yghsj_bit_1 = (byteArray[6] & 0x02) == 0x02 ? 1 : 0;
                        int yghsj_bit_2 = (byteArray[6] & 0x04) == 0x04 ? 1 : 0;
                        if (yghsj_bit_1 == 1 && yghsj_bit_2 ==0)//低速摆动
                        {
                            button4.Show();
                            button5.Show();
                            pictureBox39.Show();
                        }
                        else if (yghsj_bit_1 == 0 && yghsj_bit_2 == 1)//高速摆动
                        {
                            button4.Show();
                            button5.Show();
                            pictureBox39.Show();
                        }
                        else if (yghsj_bit_1 == 1 && yghsj_bit_2 == 1)//间歇性摆动
                        {
                            button4.Show();
                            button5.Show();
                            pictureBox39.Show();
                        }
                        else//关闭
                        {
                            button4.Hide();
                            button5.Hide();
                            pictureBox39.Hide();
                        }
                        //后雨刮位置
                        int yghsj_bit_3 = (byteArray[6] & 0x08) == 0x08 ? 1 : 0;
                        if (yghsj_bit_3 == 1)//未在初始位
                        {
                            button6.Hide();
                        }
                        else if (yghsj_bit_3 == 0)//初始位
                        {
                            button6.Hide();
                        }
                        //后雨刮状态
                        int yghsj_bit_4 = (byteArray[6] & 0x10) == 0x10 ? 1 : 0;
                        int yghsj_bit_5 = (byteArray[6] & 0x20) == 0x20 ? 1 : 0;
                        if (yghsj_bit_4 == 1 && yghsj_bit_5 == 0)//低速摆动
                        {
                            button6.Show();
                        }
                        else if (yghsj_bit_4 == 0 && yghsj_bit_5 == 1)//高速摆动
                        {
                            button6.Show();
                        }
                        else if (yghsj_bit_4 == 1 && yghsj_bit_5 == 1)//间歇性摆动
                        {
                            button6.Show();
                        }
                        else//关闭
                        {
                            button6.Hide();
                        }
                        //后视镜
                        //左后视镜折叠  
                        int yghsj_bit_6 = (byteArray[6] & 0x40) == 0x40 ? 1 : 0;
                        if (yghsj_bit_6 == 1)//展开
                        {
                            //label2.Show();
                            label1.Show();
                        }
                        else if (yghsj_bit_6 == 0)//折叠
                        {
                            //label2.Hide ();
                            label1.Hide();
                        }
                        //右后视镜折叠  
                        int yghsj_bit_7 = (byteArray[6] & 0x80) == 0x80 ? 1 : 0;
                        if (yghsj_bit_6 == 1)//展开
                        {
                            label2.Show();
                            //label1.Show();
                        }
                        else if (yghsj_bit_6 == 0)//折叠
                        {
                            label2.Hide ();
                            //label1.Hide();
                        }
                        //左后视镜加热
                        int hsjjr_bit_1 = (byteArray[7] & 0x01) == 0x01 ? 1 : 0;
                        if (hsjjr_bit_1 == 1)//加热
                        {
                            pictureBox51.Show();
                            //pictureBox52.Show();
                        }
                        else if (hsjjr_bit_1 == 0)//未加热
                        {
                            pictureBox51.Hide();
                            //pictureBox52.Hide();
                        }
                        //右后视镜加热
                        int hsjjr_bit_2 = (byteArray[7] & 0x02) == 0x02 ? 1 : 0;
                        if (hsjjr_bit_2 == 1)//加热
                        {
                            //pictureBox51.Show();
                            pictureBox52.Show();
                        }
                        else if (hsjjr_bit_2 == 0)//未加热
                        {
                            //pictureBox51.Hide();
                            pictureBox52.Hide();
                        }
                        #endregion  雨刮 后视镜 结束
                        #region 洗涤器、天窗  开始 byteArray[8]--洗涤器状态及天窗状态
                        //前洗涤器
                        int tcxd_bit_2 = (byteArray[8] & 0x04) == 0x04 ? 1 : 0;
                        if (tcxd_bit_2 == 1)//打开
                        {
                            pictureBox4.Show();
                        }
                        else if (tcxd_bit_2 == 0)//关闭
                        {
                            pictureBox4.Hide();
                        }
                        //后洗涤器
                        int tcxd_bit_3 = (byteArray[8] & 0x08) == 0x08 ? 1 : 0;
                        if (tcxd_bit_3 == 1)//打开
                        {
                            button7.Show();
                        }
                        else if (tcxd_bit_3 == 0)//关闭
                        {
                            button7.Hide();
                        }
                        //天窗
                        int tcxd_bit_4 = (byteArray[8] & 0x10) == 0x10 ? 1 : 0;
                        if (tcxd_bit_4 == 1)//打开
                        {
                            button2.Show();
                        }
                        else if (tcxd_bit_4 == 0)//关闭
                        {
                            button2.Hide();
                        }
                        #endregion 洗涤器、天窗 结束
                    }
                    break;
                case 0x04://车灯
                    {
                        //远光
                        int light_bit_0 = (byteArray[6] & 0x01) == 0x01 ? 1 : 0;
                        if (light_bit_0 == 1)//远光开
                        {
                            pictureBox3.Show();
                            pictureBox5.Show();
                            pictureBox35.Show();
                        }
                        else if (light_bit_0 == 0)//远光关
                        {
                            pictureBox3.Hide();
                            pictureBox5.Hide();
                            pictureBox35.Hide();
                        }
                        //近光
                        int light_bit_1 = (byteArray[6] & 0x02) == 0x02 ? 1 : 0;
                        if (light_bit_1 == 1)//开
                        {
                            pictureBox2.Show();
                            pictureBox6.Show();
                            pictureBox37.Show();
                        }
                        else if (light_bit_1 == 0)//关
                        {
                            pictureBox2.Hide();
                            pictureBox6.Hide();
                            pictureBox37.Hide();
                        }
                        //左转向灯
                        int light_bit_2 = (byteArray[6] & 0x04) == 0x04 ? 1 : 0;
                        if (light_bit_2 == 1)//开
                        {
                            pictureBox4.Show();
                            pictureBox1.Show();
                            pictureBox26.Show();
                        }
                        else if (light_bit_2 == 0)//关
                        {
                            pictureBox4.Hide();
                            pictureBox1.Hide();
                            pictureBox26.Hide();
                        }
                        //右转向灯
                        int light_bit_3 = (byteArray[6] & 0x08) == 0x08 ? 1 : 0;
                        if (light_bit_3 == 1)//开
                        {
                            pictureBox23.Show();
                            pictureBox11.Show();
                            pictureBox38.Show();
                        }
                        else if (light_bit_3 == 0)//关
                        {
                            pictureBox23.Hide();
                            pictureBox11.Hide();
                            pictureBox38.Hide();
                        }
                        //前雾灯
                        int light_bit_4 = (byteArray[6] & 0x10) == 0x10 ? 1 : 0;
                        if (light_bit_4 == 1)//开
                        {
                            pictureBox8.Show();
                            pictureBox10.Show();
                            pictureBox34.Show();
                        }
                        else if (light_bit_4 == 0)//关
                        {
                            pictureBox8.Hide();
                            pictureBox10.Hide();
                            pictureBox34.Hide();
                        }
                        //后雾灯
                        int light_bit_5 = (byteArray[6] & 0x20) == 0x20 ? 1 : 0;
                        if (light_bit_5 == 1)//开
                        {
                            pictureBox48.Show();
                            pictureBox27.Show();
                            pictureBox22.Show();
                        }
                        else if (light_bit_5 == 0)//关
                        {
                            pictureBox48.Hide();
                            pictureBox27.Hide();
                            pictureBox22.Hide();
                        }
                        //行车灯
                        int light_bit_6 = (byteArray[6] & 0x40) == 0x40 ? 1 : 0;
                        if (light_bit_6 == 1)//开
                        {
                            pictureBox7.Show();
                            pictureBox9.Show();
                        }
                        else if (light_bit_6 == 0)//关
                        {
                            pictureBox7.Show();
                            pictureBox9.Show();
                        }
                        //倒车灯
                        int light_bit_7 = (byteArray[6] & 0x80) == 0x80 ? 1 : 0;
                        if (light_bit_7 == 1)//开
                        {
                            pictureBox25.Show();
                            pictureBox19.Show();
                        }
                        else if (light_bit_7 == 0)//关
                        {
                            pictureBox25.Show();
                            pictureBox19.Show();
                        }
                        //刹车灯
                        int sc_bit_0 = (byteArray[7] & 0x01) == 0x01 ? 1 : 0;
                        if (sc_bit_0 == 1)//开
                        {
                            pictureBox25.Show();
                            pictureBox19.Show();
                        }
                        else if (sc_bit_0 == 0)//关
                        {
                            pictureBox25.Show();
                            pictureBox19.Show();
                        }
                        //示宽灯(牌照灯)
                        int sk_bit_1 = (byteArray[7] & 0x02) == 0x02 ? 1 : 0;
                        if (sk_bit_1 == 1)//开
                        {
                            pictureBox18.Show();
                        }
                        else if (sk_bit_1 == 0)//关
                        {
                            pictureBox18.Hide();
                        }
                        //危险警告灯
                        int wx_bit_2 = (byteArray[7] & 0x04) == 0x04 ? 1 : 0;
                        if (wx_bit_2 == 1)//开
                        {
                            pictureBox18.Show();
                        }
                        else if (wx_bit_2 == 0)//关
                        {
                            pictureBox18.Hide();
                        }
                        //双闪灯(远近关交替)
                        int ss_bit_3 = (byteArray[7] & 0x08) == 0x08 ? 1 : 0;
                        if (ss_bit_3 == 1)//开
                        {
                           
                        }
                        else if (ss_bit_3 == 0)//关
                        {
                           
                        }
                        //后备箱灯
                        int hb_bit_4 = (byteArray[7] & 0x10) == 0x10 ? 1 : 0;
                        if (hb_bit_4 == 1)//开
                        {
                            pictureBox50.Show();
                        }
                        else if (hb_bit_4 == 0)//关
                        {
                            pictureBox50.Hide();
                        }
                        //前左阅读
                        int hb_bit_5 = (byteArray[7] & 0x20) == 0x20 ? 1 : 0;
                        if (hb_bit_5 == 1)//开
                        {
                            pictureBox19.Show();
                        }
                        else if (hb_bit_5 == 0)//关
                        {
                            pictureBox19.Hide();
                        }
                        //前右阅读灯
                        int hb_bit_6 = (byteArray[7] & 0x40) == 0x40 ? 1 : 0;
                        if (hb_bit_6 == 1)//开
                        {
                            pictureBox12.Show();
                        }
                        else if (hb_bit_6 == 0)//关
                        {
                            pictureBox12.Hide();
                        }
                        //后左阅读灯
                        int hz_bit_7 = (byteArray[7] & 0x80) == 0x80 ? 1 : 0;
                        if (hz_bit_7 == 1)//开
                        {
                            pictureBox29.Show();
                        }
                        else if (hz_bit_7 == 0)//关
                        {
                            pictureBox29.Hide();
                        }
                        //后右阅读灯
                        int hz_bit_0 = (byteArray[8] & 0x01) == 0x01 ? 1 : 0;
                        if (hz_bit_0 == 1)//开
                        {
                            pictureBox30.Show();
                        }
                        else if (hz_bit_0 == 0)//关
                        {
                            pictureBox30.Hide();
                        }
                        //储物箱灯
                        int cw_bit_1 = (byteArray[8] & 0x02) == 0x02 ? 1 : 0;
                        if (cw_bit_1 == 1)//开
                        {
                            pictureBox9.Show();
                        }
                        else if (cw_bit_1 == 0)//关
                        {
                            pictureBox9.Hide();
                        }
                        //主驾化妆灯
                        int zh_bit_2 = (byteArray[8] & 0x04) == 0x04 ? 1 : 0;
                        if (zh_bit_2 == 1)//开
                        {
                            button1.Show();
                        }
                        else if (zh_bit_2 == 0)//关
                        {
                            button1.Hide();
                        }
                        //副驾化妆灯
                        int fh_bit_3 = (byteArray[8] & 0x08) == 0x08 ? 1 : 0;
                        if (fh_bit_3 == 1)//开
                        {
                            button8.Show();
                        }
                        else if (fh_bit_3 == 0)//关
                        {
                            button8.Hide();
                        }
                    }
                    break;///
            }
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
            for (int i = 2; i < (len - 1); i++)
            {
                CheckSum += pBuf[i];
            }
            pBuf[len - 1] = CheckSum;
        }
        #endregion 串口返回 结束
        #region 车灯请求部分 开始
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
            pSendBuf[3] = 0x81;

            List<byte> lTemp = new List<byte>();
            lTemp.AddRange(pSendBuf);
            lTemp.AddRange(pBuf);
            pSendBuf = new byte[lTemp.Count + 1];
            lTemp.CopyTo(pSendBuf);
            //           len  mode cmd  param checksum
            //0xff,0x55,0x07,0x01,0x15,0x01, 0x2d  
            CalculateCheckSumRequest(pSendBuf, pSendBuf.Length); //计算校验和
            //调用串口发送函数
            SendData(pSendBuf, pSendBuf.Length);
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < pSendBuf.Length; i++)
            //{
            //    sb.AppendFormat("{0:x2}" + " ", pSendBuf[i]);
            //}
            //Console.WriteLine(sb.ToString()+"----------------------------");
            //接收处理函数
           // processandshow(pSendBuf, pSendBuf.Length);
        }
        //下发数据
        private void SendData(Byte[] pBuf, int len)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(pBuf, 0, pBuf.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("设备未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 求和校验
        /// </summary>
        /// <param name="pBuf"></param>
        /// <param name="len"></param>
        private void CalculateCheckSumRequest(Byte[] pBuf, int len)
        {
            //assert(pBuf != NULL);
            byte CheckSum = 0;
            for (int i = 2; i < (len - 1); i++)
            {
                CheckSum += pBuf[i];
            }
            pBuf[len - 1] = CheckSum;
        }
        /// <summary>
        /// 控制处理
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="len"></param>
        private void processandshow(Byte[] buff, int len)
        {
            switch (buff[4])//cmd
            {
                case 0x30://远光
                    {
                        if (buff[5]==0x01)//远光开
                        {
                            pictureBox5.Show();
                            pictureBox35.Show();
                            pictureBox3.Show();
                        }
                        else if (buff[5] == 0x00)//远光关
                        {
                            pictureBox5.Hide();
                            pictureBox35.Hide();
                            pictureBox3.Hide();
                        }
                        //if (buff[5] == 0x01){pictureBox5.Show();} else { pictureBox5.Hide(); }
                    }
                    break;
                case 0x31://近光
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)//近光开
                        {
                            pictureBox2.Show();
                            pictureBox6.Show();
                            pictureBox37.Show();
                        }
                        else if (bit0 == 0)//近光关
                        {
                            pictureBox2.Hide();
                            pictureBox6.Hide();
                            pictureBox37.Hide();
                        }
                    }
                    break;
                case 0x32://雾灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox8.Show();
                            pictureBox10.Show();
                            pictureBox34.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox8.Hide();
                            pictureBox10.Hide();
                            pictureBox34.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox6.Show(); } else { pictureBox6.Hide(); }
                    }
                    break;
                case 0x33://转向灯
                    {

                        if (buff[5] == 0x01)
                        {
                            pictureBox4.Show();
                            pictureBox1.Show();
                            pictureBox26.Show();
                        }
                        else if (buff[5] == 0x00)
                        {
                            pictureBox4.Hide();
                            pictureBox1.Hide();
                            pictureBox26.Hide();
                        }
                        //右转向灯
                    }
                    break;
                case 0x34://倒车灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox25.Show();
                            pictureBox19.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox25.Hide();
                            pictureBox19.Hide();
                        }
                        // if (buff[5] == 0x01) { pictureBox10.Show(); } else { pictureBox10.Hide(); }
                    }
                    break;
                case 0x35://刹车
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox25.Show();
                            pictureBox19.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox25.Hide();
                            pictureBox19.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox11.Show(); } else { pictureBox11.Hide(); }
                    }
                    break;
                case 0x36://示宽灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox18.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox18.Show();
                        }

                        // if (buff[5] == 0x01){pictureBox7.Show();}else { pictureBox7.Hide(); }
                    }
                    break;
                case 0x37://后备箱灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox50.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox50.Hide();
                        }

                        //if (buff[5] == 0x01) { pictureBox12.Show(); } else { pictureBox12.Hide(); }
                    }
                    break;
                case 0x38://阅读灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox13.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox19.Show();
                        }
                        //if (buff[5] == 0x01) { pictureBox19.Show(); } else { pictureBox19.Hide(); }
                    }
                    break;
                case 0x39://化妆灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            button1.Show();
                        }
                        else if (bit0 == 0)
                        {
                            button1.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox18.Show(); } else { pictureBox18.Hide(); }
                    }
                    break;
                case 0x3A://储物箱
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox9.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox9.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox17.Show(); } else { pictureBox17.Hide(); }
                    }
                    break;
                case 0x3B://制动灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox25.Show();
                            pictureBox19.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox25.Hide();
                            pictureBox19.Hide();
                        }
                        // if (buff[5] == 0x01) { pictureBox16.Show(); } else { pictureBox16.Hide(); }
                    }
                    break;
                case 0x3C://行车灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox7.Show();
                            pictureBox9.Show();
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox7.Hide();
                            pictureBox9.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox16.Show(); } else { pictureBox16.Hide(); }
                    }
                    break;
                case 0x3D://脚灯
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        if (bit0 == 1)
                        {
                            pictureBox11.Show();//左脚
                            pictureBox10.Show();//右脚
                        }
                        else if (bit0 == 0)
                        {
                            pictureBox11.Hide();
                            pictureBox10.Hide();
                        }
                        //if (buff[5] == 0x01) { pictureBox15.Show(); } else { pictureBox15.Hide(); }
                    }
                    break;
                default:
                    break;
            }
        }
        private void button65_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue)
            {
                mode = 0x01;
                spc.istrue = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue = true;
            }
            Byte[] buf = { 0x30, mode };
            MakeAndSendToDriver(buf, 1);
        }

        private void button66_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue3)
            {
                mode = 0x01;
                spc.istrue3 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue3 = true;
            }
            Byte[] buf = { 0x31, mode };
            MakeAndSendToDriver(buf, 1);
        }

        private void button67_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue4)
            {
                mode = 0x01;
                spc.istrue4 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue4 = true;
            }
            Byte[] buf = { 0x32, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button59_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue5)
            {
                mode = 0x01;
                spc.istrue5 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue5 = true;
            }
            Byte[] buf = { 0x33, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button58_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue17)
            {
                mode = 0x01;
                spc.istrue17 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue17 = true;
            }
            Byte[] buf = { 0x33, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button56_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue6)
            {
                mode = 0x01;
                spc.istrue6 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue6 = true;
            }
            Byte[] buf = { 0x34, mode };
            MakeAndSendToDriver(buf, 1);
        }

        private void button63_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue7)
            {
                mode = 0x01;
                spc.istrue7 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue7 = true;
            }
            Byte[] buf = { 0x35, mode };
            MakeAndSendToDriver(buf, 1);
        }

        private void btnShiKuang_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue2)
            {
                mode = 0x01;
                spc.istrue2 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue2 = true;
            }
            Byte[] buf = { 0x36, mode };
            MakeAndSendToDriver(buf, 2);
        }
        private void button60_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue8)
            {
                mode = 0x01;
                spc.istrue8 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue8 = true;
            }
            Byte[] buf = { 0x37, mode };
            MakeAndSendToDriver(buf, 1);
        }

        private void button55_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue11)
            {
                mode = 0x01;
                spc.istrue11 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue11 = true;
            }
            Byte[] buf = { 0x39, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button62_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue9)
            {
                mode = 0x01;
                spc.istrue9 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue9 = true;
            }
            Byte[] buf = { 0x38, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button57_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue10)
            {
                mode = 0x01;
                spc.istrue10 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue10 = true;
            }
            Byte[] buf = { 0x38, mode };
            MakeAndSendToDriver(buf, 2);
        }
        private void button54_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue12)
            {
                mode = 0x01;
                spc.istrue12 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue12 = true;
            }
            Byte[] buf = { 0x3A, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button51_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue15)
            {
                mode = 0x01;
                spc.istrue15 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue15 = true;
            }
            Byte[] buf = { 0x3D, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button52_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue16)
            {
                mode = 0x01;
                spc.istrue16 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue16 = true;
            }
            Byte[] buf = { 0x3D, mode };
            MakeAndSendToDriver(buf, 2);
        }

        private void button61_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (spc.istrue14)
            {
                mode = 0x01;
                spc.istrue14 = false;
            }
            else
            {
                mode = 0x00;
                spc.istrue14 = true;
            }
            Byte[] buf = { 0x3C, mode };
            MakeAndSendToDriver(buf, 2);
        }
        #endregion 车灯请求部分 开始
        #region 车窗部分 开始

        private void button41_Click(object sender, EventArgs e)
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
            SendToPort(buf, 1, sender);
        }
        private void button40_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x10, mode, };
            SendToPort(buf, 1, sender);
        }

        private void button37_Click(object sender, EventArgs e)
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

        private void button36_Click(object sender, EventArgs e)
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

        private void button35_Click(object sender, EventArgs e)
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
            SendToPort(buf, 1, sender);
        }

        private void button34_Click(object sender, EventArgs e)
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
            SendToPort(buf, 1, sender);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            AutomativeLighting al = new AutomativeLighting();
            al.Show();
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
        }

        private void button33_Click(object sender, EventArgs e)
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
            SendToPort(buf, 1, sender);
        }

        private void button32_Click(object sender, EventArgs e)
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
            SendToPort(buf, 1, sender);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            byte mode = 0x00;
            Byte[] buf = { 0x11, mode };
            SendToPort(buf, 1, sender);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            byte mode = 0x01;
            Byte[] buf = { 0x11, mode };
            SendToPort(buf, 1, sender);
        }

        private void button27_Click(object sender, EventArgs e)
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
            SendToPort(buf, 1, sender);
        }

        private void button26_Click(object sender, EventArgs e)
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
        #endregion 车窗部分结束
        #region 一键启动 开始
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button50_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue11)
            {
                mode = 0x02;
                istrue11 = false;
            }
            else
            {
                mode = 0x00;
                istrue11 = true;
            }
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }

        private void button49_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }

        private void button48_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue13)
            {
                mode = 0x00;
                istrue13 = false;
            }
            else
            {
                mode = 0x00;
                istrue13 = true;
            }
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }

        private void button43_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x15, mode };
            SendToPort(buf, 1, sender);
        }

        private void button42_Click(object sender, EventArgs e)
        {

        }

        private void button47_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue14)
            {
                mode = 0x02;
                istrue14 = false;
            }
            else
            {
                mode = 0x00;
                istrue14 = true;
            }
            Byte[] buf = { 0x14, mode };
            SendToPort(buf, 1, sender);
        }
        #endregion 一键启动结束
        #region 天窗部分 开始

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button25_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue16)
            {
                mode = 0x02;
                istrue16 = false;
            }
            else
            {
                mode = 0x00;
                istrue16 = true;
            }
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }

        private void button24_Click(object sender, EventArgs e)
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
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue18)
            {
                mode = 0x01;
                istrue18 = false;
            }
            else
            {
                mode = 0x00;
                istrue18 = true;
            }
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue19)
            {
                mode = 0x02;
                istrue19 = false;
            }
            else
            {
                mode = 0x00;
                istrue19 = true;
            }
            Byte[] buf = { 0x13, mode };
            SendToPort(buf, 1, sender);
        }
        #endregion 天窗结束
        #region 雨刮部分 开始
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue20)
            {
                mode = 0x01;
                istrue20 = false;
            }
            else
            {
                mode = 0x00;
                istrue20 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue21)
            {
                mode = 0x02;
                istrue21 = false;
            }
            else
            {
                mode = 0x00;
                istrue21 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue22)
            {
                mode = 0x02;
                istrue22 = false;
            }
            else
            {
                mode = 0x00;
                istrue22 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue23)
            {
                mode = 0x03;
                istrue23 = false;
            }
            else
            {
                mode = 0x00;
                istrue23 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            byte mode = 0;
            if (istrue24)
            {
                mode = 0x01;
                istrue24 = false;
            }
            else
            {
                mode = 0x00;
                istrue24 = true;
            }
            Byte[] buf = { 0x12, mode };
            SendToPort(buf, 1, sender);
        }
        #endregion  雨刮部分 结束
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

            CalculateCheckSumRequest(SendBuff, SendBuff.Length);//求和校验
            //ProcessAndShow(SendBuff, SendBuff.Length, sender);//处理数据
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="len"></param>
        /// <param name="sender"></param>
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
                                ucLabel1.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }
                        if (button.Name.Equals("button1"))
                        {
                            int bit0 = (buff5 & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 0)//左前车窗闭合（上升）
                            {
                                ucLabel1.Hide();
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
                                ucLabel3.Show();
                            }
                            else
                            {
                                MessageBox.Show("松手", "提示");
                            }
                        }
                        if (button.Name.Equals("button11"))
                        {
                            int bit1 = (buff5 & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 0)//右前车窗关闭（上升）
                            {
                                ucLabel3.Hide();
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
                                ucLabel5.Show();
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
                                ucLabel5.Hide();
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
                                ucLabel4.Show();
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
                                ucLabel4.Hide();
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
                            int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 1)//后视镜加热
                            {
                                pictureBox51.Show();
                                pictureBox52.Show();
                            }
                            else if (bit1 == 0)
                            {
                                pictureBox51.Hide();
                                pictureBox52.Hide();
                            }
                        }
                        if (button.Name.Equals("button22"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//后视镜折叠
                            {
                                label2.Show();
                                label1.Show();
                            }
                            else if (bit0 == 0)
                            {
                                label2.Hide();
                                label1.Hide();
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
                case 0x12://雨刮
                    {
                        if (button.Name.Equals("button1"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//雨刮摆动一次
                            {
                                button4.Show();
                                button5.Show();
                                pictureBox39.Show();
                            }
                            else if (bit0 == 0)
                            {
                                button4.Hide();
                                button5.Hide();
                                pictureBox39.Hide();
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
                                button4.Show();
                                button5.Show();
                                pictureBox39.Show();
                            }
                            else if (bit2 == 0)
                            {
                                button4.Hide();
                                button5.Hide();
                                pictureBox39.Hide();
                            }
                        }
                        if (button.Name.Equals("button5"))
                        {
                            int bit3 = (buff[5] & 0x04) == 0x04 ? 1 : 0;
                            if (bit3 == 1)//高速摆动
                            {
                                pictureBox4.Show();
                                pictureBox39.Show();
                            }
                            else if (bit3 == 0)
                            {
                                pictureBox4.Hide();
                                pictureBox39.Hide();
                            }
                        }
                        if (button.Name.Equals("button6"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//玻璃洗涤
                            {
                                pictureBox4.Show();
                            }
                            else if (bit0 == 0)
                            {
                                pictureBox4.Hide();
                            }
                        }
                    }
                    break;
                case 0x13://天窗
                    {
                        if (button.Name.Equals("button1"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 1)//开启天窗
                            {
                                button2.Show();
                            }
                            else
                            {
                                MessageBox.Show("未触发", "提示");
                            }
                        }
                        if (button.Name.Equals("button2"))
                        {
                            int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                            if (bit0 == 0)//关闭天窗
                            {
                                button2.Hide();
                            }
                        }
                        if (button.Name.Equals("button5"))
                        {
                            int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 1)//起翘天窗
                            {
                                button2.Show();
                            }
                        }
                        if (button.Name.Equals("button6"))
                        {
                            int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                            if (bit1 == 0)//关闭起翘天窗
                            {
                                button2.Hide();
                            }
                        }
                    }
                    break;

                case 0x14://车门 后备箱
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                        int bit2 = (buff[5] & 0x03) == 0x03 ? 1 : 0;
                        int bit3 = (buff[5] & 0x04) == 0x04 ? 1 : 0;
                        int bit4 = (buff[5] & 0x05) == 0x05 ? 1 : 0;
                        if (button.Name.Equals("button3"))
                        {

                            if (bit0 == 0 && bit1 == 0 && bit2 == 0 && bit3 == 0)//车门上锁
                            {
                                pictureBox8.Show();
                            }
                        }
                        if (button.Name.Equals("button2"))
                        {
                            if (bit0 == 1 && bit1 == 1 && bit2 == 1 && bit3 == 1)//车门解锁
                            {
                                pictureBox8.Show();
                            }
                        }
                        if (button.Name.Equals("button1"))
                        {
                            if (bit0 == 1 && bit1 == 0 && bit2 == 0 && bit3 == 0)//解锁驾驶门
                            {
                                pictureBox14.Show();
                            }
                            else if (true)
                            {

                            }
                        }
                        if (button.Name.Equals("button4"))
                        {
                            if (bit4 == 1)//解锁后备箱
                            {
                                pictureBox46.Show();
                                pictureBox54.Show();
                            }
                            else if (bit4 == 0)
                            {
                                pictureBox46.Hide();
                                pictureBox54.Show();
                            }
                        }
                    }
                    break;

                case 0x15://中控车门上锁
                    {
                        int bit0 = (buff[5] & 0x01) == 0x01 ? 1 : 0;
                        int bit1 = (buff[5] & 0x02) == 0x02 ? 1 : 0;
                        int bit2 = (buff[5] & 0x03) == 0x03 ? 1 : 0;
                        int bit3 = (buff[5] & 0x04) == 0x04 ? 1 : 0;
                        int bit4 = (buff[5] & 0x05) == 0x05 ? 1 : 0;
                        if (bit0 == 0 && bit1 == 0 && bit2 == 0 && bit3 == 0)//车门上锁
                        {
                            pictureBox47.Show();
                            pictureBox14.Hide();
                            pictureBox16.Hide();
                            pictureBox15.Hide();
                            pictureBox17.Hide();
                        }
                        else if (bit0 == 1 && bit1 == 1 && bit2 == 1 && bit3 == 1)
                        {
                            pictureBox47.Hide();
                            pictureBox14.Show();
                            pictureBox16.Show();
                            pictureBox15.Show();
                            pictureBox17.Show();
                        }

                    }
                    break;
                default:
                    break;
            }
        }
    }
}