using MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp.Models
{
	class ClientConnection
	{
		// Порт
		static int port = 1800;
		// Адрес
		static IPAddress ipAddress;
		// Локальная конечная точка
		IPEndPoint ipEndPoint;
		// Сокет
		Socket socket;
		//Размер буфера для изображений
		int imageBufferLength;
		//Размер буфера для метаданных
		int metaBufferLength;
		//Событие принятия команды
		ManualResetEvent commandEvent;
		//Идет загрузка изображений
		bool uploadingImages = true;
		//Ответ
		int response;
		//Список изображений
		List<ImageSource> images = new List<ImageSource>();

		public ClientConnection(int imageBufferLength, int metaBufferLength)
		{
			this.imageBufferLength = imageBufferLength;
			this.metaBufferLength = metaBufferLength;
		}

		public Task<List<ImageSource>> Connection(object message)
		{
			return null;
			//return Task.Run(() =>
			//{
			//	Configure((string)message);
			//	commandEvent = new ManualResetEvent(false);
			//	string presentationName = GetPresentationName();
			//	//Console.WriteLine(presentationName);
			//	int slidesCount = GetSlidesCount();
			//	int i = 1;
			//	while (i <= slidesCount)
			//	{
			//		if (ReceiveDistributor() == 0)
			//		{
			//			i++;
			//		}
			//	}
			//	uploadingImages = false;
			//	return images;
			//});
		}

		public void Configure(string IP)
		{
			ipAddress = IPAddress.Parse("192.168.0.2"); //присваиваем IP-адрес
														//ipAddress = IPAddress.Parse(IP); //присваиваем IP-адрес
			ipEndPoint = new IPEndPoint(ipAddress, port); // создаем локальную конечную точку
			socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // создаем основной сокет
			socket.Connect(ipEndPoint);
		}

		public string GetPresentationName()
		{
			byte[] receiveBuffer = new byte[metaBufferLength]; //буфер для метаданных
			socket.Receive(receiveBuffer); //принимаем метаданные
			int nameLength = BitConverter.ToInt32(receiveBuffer, 0) * 2; //переводим в число
			Array.Resize(ref receiveBuffer, nameLength);
			socket.Receive(receiveBuffer); //принимаем название
			return Encoding.Unicode.GetString(receiveBuffer);
		}

		public int GetSlidesCount()
		{
			byte[] receiveMetaBuffer = new byte[metaBufferLength]; //буфер для количества слайдов
			socket.Receive(receiveMetaBuffer); //записываем метаданные
			return BitConverter.ToInt32(receiveMetaBuffer, 0); //узнаем количество слайдов, которые нам придут
		}

		public int ReceiveDistributor()
		{
			byte[] receiveMetaBuffer = new byte[metaBufferLength]; //буфер для метаданных
			socket.Receive(receiveMetaBuffer); //записываем метаданные
			string strCode = Encoding.Unicode.GetString(receiveMetaBuffer);
			int intCode = BitConverter.ToInt32(receiveMetaBuffer, 0);
			if (strCode == "-1" || strCode == "-2")
			{
				response = intCode;
				commandEvent.Set();
				return -1;
			}
			else
			{
				SetImage(intCode);
				return 0;
			}
		}

		public void SetImage(int meta)
		{
			byte[] byteImage = ReceiveImage(meta, imageBufferLength);
			ImageSource image = ClientImageConverter.ByteArrayToImage(byteImage);
			images.Add(image);
		}

		public byte[] ReceiveImage(int countBytes, int bufferLength)
		{
			//Console.WriteLine("countBytes - " + countBytes);
			byte[] byteArray = new byte[countBytes]; //создаем буфер для всей картинки
			int receiveBytes = 0; //общее количество принятых байт и рулетка в одном лице
			while (receiveBytes < countBytes)
			{
				byte[] receiveBuffer = new byte[countBytes - receiveBytes >= bufferLength ? bufferLength : countBytes - receiveBytes]; //буфер, куда записываем принятые данные (кусочек картинки)
				int bytes = socket.Receive(receiveBuffer); //записываем количество принятых байт
				receiveBuffer.CopyTo(byteArray, receiveBytes); //сохраняем принятые байты в хранилище
				receiveBytes += bytes; //сдвигаем индекс и суммируем общее количество принятых байт
			}
			//Console.WriteLine("Пришло - " + receiveBytes);
			return byteArray;
		}

		public Task<int> Request(string message)
		{
			return Task.Run(() =>
			{
				SendCode(message);
				if (uploadingImages)
				{
					commandEvent.WaitOne();
					commandEvent.Reset();
				}
				else
					response = ReceiveCode();

				//Console.WriteLine("ClientTask - " + response);
				return response;
			});
		}

		public void SendCode(string message)
		{
			byte[] sendBuffer = Encoding.Unicode.GetBytes(message); // массив с данными
			socket.Send(sendBuffer);
		}

		public int ReceiveCode()
		{
			byte[] receiveBuffer = new byte[metaBufferLength];
			socket.Receive(receiveBuffer);
			return BitConverter.ToInt32(receiveBuffer, 0);
		}

		/*
                        public void Shutdown() // освобождаем сокеты
                        {
                            try
                            {
                                socket.Shutdown(SocketShutdown.Both);
                                socket.Close();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
        */
	}
}
