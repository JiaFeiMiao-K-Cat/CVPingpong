using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVPingpong
{
    public static class SerialPorts
    {
        static SerialPort _port;
        static SerialPort port 
        { 
            get { 
                if (_port == null)
                {
                    _port = new SerialPort(SerialPort.GetPortNames().Last(), 115200, Parity.None, 8, StopBits.One);
                    if (!_port.IsOpen)
                    {
                        _port.DataReceived += new SerialDataReceivedEventHandler(ReceiveData);
                        _port.Open();
                    }
                }
                return _port;
            }
            set { 
                _port = value;
            } 
        }
        public static void SetSerialPort(string portName) 
        {
            port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
            if (!port.IsOpen)
            {
                port.DataReceived += new SerialDataReceivedEventHandler(ReceiveData);
                port.Open();
            }
        }
        public static void ReceiveData(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort _SerialPort = (SerialPort)sender;

            int _bytesToRead = _SerialPort.BytesToRead;
            byte[] recvData = new byte[_bytesToRead];

            _SerialPort.Read(recvData, 0, _bytesToRead);

            string recv = Encoding.ASCII.GetString(recvData);

            Console.WriteLine("收到数据：" + recv);
        }

        public static bool SendData(byte[] data)
        {
            if (port != null && port.IsOpen)
            {
                port.Write(data, 0, data.Length);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
