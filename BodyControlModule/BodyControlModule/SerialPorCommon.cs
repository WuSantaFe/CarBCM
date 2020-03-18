using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BodyControlModule
{
    class SerialPorCommon
    {
        public  bool istrue = true, istrue2 = true, istrue3 = true, istrue4 = true, istrue5 = true, istrue6 = true, istrue7 = true,
           istrue8 = true, istrue9 = true, istrue10 = true, istrue11 = true, istrue12 = true, istrue13 = true, istrue14 = true,
           istrue15 = true, istrue16 = true, istrue17 = true, istrue18 = true, istrue19 = false;

        private SerialPort serialPort = new SerialPort();
        bool isOpened = false;//串口状态标志
        //KeyLessStartup kls2 = new KeyLessStartup();

        public void openCom(ComboBox cb1, ComboBox cb2, Button btn)
        {
            if (!isOpened)
            {
                RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
                if (keyCom != null)
                {
                    string[] sSubKeys = keyCom.GetValueNames();
                    cb1.Items.Clear();
                    foreach (string sName in sSubKeys)
                    {
                        string sValue = (string)keyCom.GetValue(sName);
                        cb1.Items.Add(sValue);
                    }
                    if (cb1.Items.Count > 0)
                        cb1.SelectedIndex = 0;
                }
                cb2.Text = "115200";
                serialPort.PortName = cb1.Text;
                serialPort.BaudRate = Convert.ToInt32(cb2.Text, 10);
                try
                {
                    serialPort.Open();     //打开串口
                    btn.Text = "关闭串口";
                    cb1.Enabled = false;//关闭使能
                    cb2.Enabled = false;
                    isOpened = true;
                   // serialPort.DataReceived += new SerialDataReceivedEventHandler(post_DataReceived);//串口接收处理函数
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
                    btn.Text = "打开串口";
                    cb1.Enabled = true;//打开使能
                    cb2.Enabled = true;
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
            ProcessShowData(ReDatas, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void ProcessShowData(byte[] data, Form form)
        {
            form.BeginInvoke(new MethodInvoker(delegate
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
                                  // kls2.show(data);
                                }
                            }
                            break;
                    }
                }
            }));
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
    }
}
