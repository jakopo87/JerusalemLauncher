using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace JerusalemLauncher
{
	static class Program
	{
		private static readonly string ExeName = "Jerusalem.exe";
		private static readonly string SavePath = @"data\Save";
		private static readonly string SaveFilter = "SaveGame.ars";
		private static readonly string SaveEntries = @"data\save\user.aba";
		private static FileSystemWatcher watcher;
		private static List<SaveEntry> SaveList;

		[STAThread]
		static void Main()
		{
			if (File.Exists(ExeName) == false)
			{
				MessageBox.Show(string.Format("{0} not found in the current folder", ExeName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				SetupWacher();
				ReadSaveEntries();
				LaunchGame();
				WriteSaveEntries();
			}
		}

		private static void ReadSaveEntries()
		{
			SaveList = new List<SaveEntry>();
			using (var reader = new BinaryReader(File.OpenRead(SaveEntries)))
			{
				uint n = reader.ReadUInt32();
				for (int i = 0; i < n; i++)
				{
					// Display name is duplicated, skipping it
					reader.ReadChars(reader.ReadInt32());

					var save = new SaveEntry()
					{
						DisplayName = reader.ReadChars(reader.ReadInt32()),
						Filename = reader.ReadChars(reader.ReadInt32()),
					};
					SaveList.Add(save);
				}
			}
		}

		private static void LaunchGame()
		{
			var process = Process.Start(ExeName);
			process.WaitForExit();
		}

		private static void SetupWacher()
		{
			watcher = new FileSystemWatcher(SavePath, SaveFilter)
			{
				EnableRaisingEvents = true,
				NotifyFilter = NotifyFilters.LastWrite
			};
			watcher.Changed += Watcher_Changed;
		}

		private static void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			var n = Directory.GetFiles(SavePath, "*.ars").Length;
			var filename = string.Format("1_ArSa{0}", n);

			var source = SavePath + "\\" + SaveFilter;
			var dest = SavePath + "\\" + filename + ".ars";

			try
			{
				File.Copy(source, dest);
				Console.WriteLine(string.Format("{0} created", dest));

				var save = new SaveEntry
				{
					DisplayName = (DateTime.Now.ToString() + '\0').ToCharArray(),
					Filename = (filename + '\0').ToCharArray()
				};

				SaveList.Add(save);
			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void WriteSaveEntries()
		{
			using (var writer = new BinaryWriter(File.OpenWrite(SaveEntries), Encoding.ASCII))
			{
				writer.Write((uint)SaveList.Count);

				foreach (var save in SaveList)
				{
					// Display name is duplicated inside the file
					writer.Write(save.DisplayName.Length);
					writer.Write(save.DisplayName);
					writer.Write(save.DisplayName.Length);
					writer.Write(save.DisplayName);
					writer.Write(save.Filename.Length);
					writer.Write(save.Filename);
				}
			}
		}
	}

	struct SaveEntry
	{
		public char[] DisplayName;
		public char[] Filename;
	}
}
