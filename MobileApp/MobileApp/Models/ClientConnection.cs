﻿using MobileApp.Services;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

//!!!ПЕРЕД ЗАВЕРШЕНИЕМ РАБОТЫ НУЖНО ВЫЗЫВАТЬ Shutdown() - закрытие сокетов

namespace MobileApp.Models
{
	public class ClientConnection
	{

		public string Title { get; private set; }
		// Порт
		private static int port = 1800;
		// Адрес
		private static IPAddress ipAddress;
		// Локальная конечная точка
		private IPEndPoint ipEndPoint;
		// Сокет
		private Socket socket;
		//Размер буфера для изображений
		private int imageBufferLength;
		//Размер буфера для метаданных
		private int metaBufferLength;
		//Событие принятия команды
		private AutoResetEvent commandEvent;
		//Идет загрузка изображений
		private bool uploadingImages = true;
		//Ответ
		private int response;
		//Список изображений
		private List<ImageSource> images = new List<ImageSource>();
		//private List<string> sources = new List<string>();

		private ClientImageConverter im = new ClientImageConverter();

		public ClientConnection(int imageBufferLength, int metaBufferLength)
		{
			this.imageBufferLength = imageBufferLength;
			this.metaBufferLength = metaBufferLength;
		}

		public Task<List<ImageSource>> Connection(object message) => Task.Run(() =>
																			   {
																				   Configure((string)message);
																				   commandEvent = new AutoResetEvent(false);
																				   Title = GetPresentationName();
																				   int slidesCount = GetSlidesCount();
																				   int i = 1;
																				   while (i <= slidesCount)
																				   {
																					   if (ReceiveDistributor(i) == 0)
																					   {
																						   i++;
																					   }
																				   }
																				   uploadingImages = false;
																				   return images;
																				   //																				   for (int i = 1; i < 10; i++)
																				   //																				   {
																				   //#pragma warning disable CS0618 // Тип или член устарел	

																				   //																					   images.Add(Device.OnPlatform(
																				   //																							  iOS: "Slides/Image_0" + i + ".png",
																				   //																							  Android: "Image_0" + i + ".png",
																				   //																							  WinPhone: "Resources/Slides/Image_0" + i + ".png"
																				   //																						  ));
																				   //																					   //using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
																				   //																					   //{

																				   //																						  // byte[] file = System.IO.File.ReadAllBytes("Resources/Slides/Image_0" + i + ".png");
																				   //																						  // file.SaveImage("file_" + i + ".png");

																				   //																					   //}
																				   //																					   //IFolder folder = FileSystem.Current.LocalStorage;
																				   //																					   //Task<IFile> fil = folder.GetFileAsync("file_" + i + ".png");
																				   //																					   ////byte[] f = System.IO.File.ReadAllBytes("file_" + i + ".png");


																				   //#pragma warning restore CS0618 // Тип или член устарел
																				   //																				   }
																				   //																				   return images;
																			   });

		public void Configure(string IP)
		{
			//ipAddress = IPAddress.Parse("10.17.2.204"); //присваиваем IP-адрес
			ipAddress = IPAddress.Parse(IP); //присваиваем IP-адрес
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

		public int ReceiveDistributor(int i)
		{
			byte[] receiveMetaBuffer = new byte[metaBufferLength]; //буфер для метаданных
			socket.Receive(receiveMetaBuffer); //записываем метаданные
			int intCode = BitConverter.ToInt32(receiveMetaBuffer, 0);
			if (intCode == -1 || intCode == -2)
			{
				response = intCode;
				commandEvent.Set();
				return -1;
			}
			else
			{
				SetImage(intCode, i);
				return 0;
			}
		}

		public void SetImage(int meta, int i)
		{
			byte[] byteImage = ReceiveImage(meta, imageBufferLength);	 
			Save(byteImage, i);
			//images.Add(image);
			
		}

		private async void Save(byte[] byteImage, int i)
		{
			await byteImage.SaveImage("slide_" + i + ".png");
			//DependencyService.Get<IFileWorker>().DeleteAsync("slide_" + i);
			//await byteImage.LoadImage("slide_" + i);
			//ImageSource image = ClientImageConverter.ByteArrayToImage(byteImage);
			//images.Add(image);
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

		public Task<int> Request(int message) => Task.Run(() =>
														   {
															   SendCode(message);
															   if (uploadingImages)
															   {
																   commandEvent.WaitOne();
															   }
															   else
															   {
																   response = ReceiveCode();
															   }

															   //Console.WriteLine("ClientTask - " + response);
															   return response;
														   });

		public void SendCode(int message)
		{
			byte[] sendBuffer = BitConverter.GetBytes(message); // массив с данными
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

	public static class WorkWithSorage
	{
		public async static Task SaveImage(this byte[] image, string fileName, IFolder rootFolder = null)
		{
			// get hold of the file system  
			IFolder folder = rootFolder ?? FileSystem.Current.LocalStorage;
			// create a file, overwriting any existing file  
			IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
			// populate the file with image data  
			using (System.IO.Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
			{
				stream.Write(image, 0, image.Length);
			}
		}

		public async static Task<byte[]> LoadImage(this byte[] image, string fileName, IFolder rootFolder = null)
		{
			// get hold of the file system  
			IFolder folder = rootFolder ?? FileSystem.Current.LocalStorage;

			//open file if exists  
			IFile file = await folder.GetFileAsync(fileName);
			//load stream to buffer  
			using (System.IO.Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
			{
				long length = stream.Length;
				byte[] streamBuffer = new byte[length];
				stream.Read(streamBuffer, 0, (int)length);
				return streamBuffer;
			}

		}
	}
}