﻿using MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(MobileApp.UWP.FileWorker))]
namespace MobileApp.UWP
{
	public class FileWorker : IFileWorker
	{
		public async Task DeleteAsync(string filename)
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFile storageFile = await localFolder.GetFileAsync(filename);
			await storageFile.DeleteAsync();
		}

		public async Task<bool> ExistsAsync(string filename)
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			try
			{
				await localFolder.GetFileAsync(filename);
			}
			catch { return false; }
			return true;
		}

		public async Task<IEnumerable<string>> GetFilesAsync()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			IEnumerable<string> filenames = from storageFile in await localFolder.GetFilesAsync()
											select storageFile.Name;
			return filenames;
		}

		public string GetLocalFolderPath()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			return localFolder.Path;
		}

		public async Task<string> LoadTextAsync(string filename)
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			// получаем файл
			StorageFile helloFile = await localFolder.GetFileAsync(filename);
			// читаем файл
			string text = await FileIO.ReadTextAsync(helloFile);
			return text;
		}

		public async Task SaveTextAsync(string filename, string text)
		{
			// получаем локальную папку
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			// создаем файл hello.txt
			StorageFile helloFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
			// запись в файл
			await FileIO.WriteTextAsync(helloFile, text);
		}
	}
}
