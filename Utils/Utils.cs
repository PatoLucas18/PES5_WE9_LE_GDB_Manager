using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows.Forms;

namespace PES5_WE9_LE_GDB_Manager
{
    public static class Utils
    {
        public const string APP_NAME = "PES5 / WE9 / LE GDB Manager";

        public static List<Color> GetSupporterColours()
        {

            List<Color> colourList = new List<Color>();
            string[] colours = new string[]
            {
                "Black",
                "Blue",
                "Red",
                "Pink",
                "Lime",
                "LightBlue",
                "Yellow",
                "White",
                "Gray",
                "Navy",
                "Maroon",
                "Purple",
                "Green",
                "Gold",
                "Orange",
            };
            foreach (var colour in colours)
            {
                Color color = Color.FromName(colour);
                colourList.Add(color);
            }
            return colourList;
        }

        public static uint ReadUInt32(byte[] data, uint startIndex)
        {
            uint value = (uint)(data[startIndex] | (data[startIndex + 1] << 8) | (data[startIndex + 2] << 16) | (data[startIndex + 3] << 24));
            return value;
        }
        public static uint ReadUInt16(byte[] data, uint startIndex)
        {
            uint value = (uint)(data[startIndex] | (data[startIndex + 1] << 8));
            return value;
        }
        public static uint ZeroFillRightShift(uint val, int n)
        {
            return (uint)((val % 0x100000000) >> n);
        }

        static public void CheckForUpdates()
        {
            try
            {
                GithubApi githubApi = GetGithubApi();
                if (Application.ProductVersion != githubApi.TagName)
                {
                    DialogResult result = MessageBox.Show($"There's a new update version: {githubApi.TagName}, would you like to download it?", $"GDB Manager 5 Updater", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start($"{githubApi.Assets.First().DownloadUrl}");
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error has ocurred while trying to check for updates\nError: {ex.Message}", $"GDB Manager 5 Updater");
            }
        }

        static private GithubApi GetGithubApi()
        {
            string url = "https://api.github.com/repos/moth1995/PES5_WE9_LE_GDB_Manager/releases/latest";
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            WebRequest webRequest = WebRequest.Create(url);

            HttpWebRequest request = (HttpWebRequest)webRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(GithubApi));
                GithubApi githubApiResponse = (GithubApi)serializer.ReadObject(responseStream);
                Console.WriteLine(githubApiResponse.TagName);
                Console.WriteLine(githubApiResponse.Assets.First().DownloadUrl);
                return githubApiResponse;
            }
        }
        static public GDBManagerConfig LoadConfiguration(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                var serializer = new DataContractJsonSerializer(typeof(GDBManagerConfig));
                return (GDBManagerConfig)serializer.ReadObject(fs);
            }
        }
        static public void SaveConfig(GDBManagerConfig config)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PES5_WE9_LE_GDB_Manager.json");
            using (FileStream fs = new FileStream(configPath, FileMode.Create))
            {
                var serializer = new DataContractJsonSerializer(typeof(GDBManagerConfig));
                serializer.WriteObject(fs, config);
            }
        }

        public static bool IsSubPath(string basePath, string subPath)
        {
            string absoluteBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string absoluteSubPath = Path.GetFullPath(subPath);

            return absoluteSubPath.StartsWith(absoluteBasePath, StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsSubDirectory(string baseDir, string subDir)
        {
            string absoluteBaseDir = Path.GetFullPath(baseDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string absoluteSubDir = Path.GetFullPath(subDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            return absoluteSubDir.StartsWith(absoluteBaseDir, StringComparison.OrdinalIgnoreCase);
        }
        public static string GetStringInsideQuotes(string str)
        {
            int firstQuoteIndex = str.IndexOf('"');
            if (firstQuoteIndex == -1)
                return "";

            int secondQuoteIndex = str.IndexOf('"', firstQuoteIndex + 1);
            if (secondQuoteIndex == -1)
                return "";
            return str.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);

        }
        [DllImport("zlib1.dll", EntryPoint = "compress", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CompressByteArray(byte[] dest, ref int destLen, byte[] src, int srcLen);

        [DllImport("zlib1.dll", EntryPoint = "uncompress", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int UncompressByteArray(byte[] dest, ref uint destLen, byte[] src, uint srcLen);

        public static byte[] Unzlib(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                uint magicNumber = reader.ReadUInt32();
                if (magicNumber != 0x00010600)
                {
                    throw new ArgumentException($"{filePath} is not a zlib file!");
                }
                uint compressSize = reader.ReadUInt32();
                uint decompressSize = reader.ReadUInt32();
                reader.ReadBytes(20);
                byte[] compressData = reader.ReadBytes((int)compressSize);
                byte[] decompressData = new byte[decompressSize];
                int result = UncompressByteArray(decompressData, ref decompressSize, compressData, compressSize);
                if (result != 0 && result != -5)
                {
                    throw new Exception($"An error has ocurred while trying to decompress {filePath}");
                }
                return decompressData;
            }
        }
        public static PESTexture ReadPESTexture(byte[] byteArray, int fileIndex, bool readFileIndexes)
        {
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                uint fileOffset = 0;
                if (readFileIndexes)
                {
                    uint fileCount = reader.ReadUInt32();

                    uint offsetsOffset = reader.ReadUInt32();

                    reader.BaseStream.Seek(offsetsOffset, SeekOrigin.Begin);

                    uint[] fileOffsets = new uint[fileCount];
                    for (int i = 0; i < fileCount; i++)
                    {
                        fileOffsets[i] = reader.ReadUInt32();
                    }

                    if (fileCount < fileIndex + 1)
                    {
                        throw new Exception("Not enough files!");
                    }
                    fileOffset = fileOffsets[fileIndex];
                    reader.BaseStream.Seek(fileOffset, SeekOrigin.Begin);

                }
                uint validationValue = reader.ReadUInt32();

                if (validationValue != 0x29857294)
                {
                    throw new Exception("Not a PES Texture!");
                }

                reader.BaseStream.Seek(fileOffset + 36, SeekOrigin.Begin);
                uint bpp = reader.ReadUInt32();

                if(bpp != 0x40)
                {
                    throw new Exception("Not a 8bpp image");
                }

                reader.BaseStream.Seek(fileOffset + 8, SeekOrigin.Begin);

                uint fileSize = reader.ReadUInt32();

                reader.BaseStream.Seek(fileOffset + 16, SeekOrigin.Begin);

                ushort pixelsOffset = reader.ReadUInt16();
                ushort paletteOffset = reader.ReadUInt16();

                PESTexture pesTexture = new PESTexture();

                pesTexture.Width = reader.ReadUInt16();
                pesTexture.Height = reader.ReadUInt16();

                reader.BaseStream.Seek(fileOffset + pixelsOffset, SeekOrigin.Begin);
                pesTexture.Pixels = reader.ReadBytes((int)(pesTexture.Width * pesTexture.Height));

                reader.BaseStream.Seek(fileOffset + paletteOffset, SeekOrigin.Begin);
                pesTexture.Palette = reader.ReadBytes((int)(pixelsOffset - paletteOffset));

                return pesTexture;
            }
        }

        public static Bitmap CreateImageFromPaletteAndPixels(byte[] palette, byte[] pixels, int width, int height)
        {
            // Crear un bitmap con las dimensiones especificadas
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // Verificar que la paleta tenga 256 colores (256 * 4 bytes para BGRA)
            if (palette.Length != 256 * 4)
            {
                throw new ArgumentException("The palette is invalid.");
            }

            // Verificar que el número de píxeles coincide con las dimensiones de la imagen
            if (pixels.Length != width * height)
            {
                throw new ArgumentException("Invalid pixel data.");
            }

            // Obtener la paleta del bitmap
            ColorPalette bmpPalette = bitmap.Palette;

            // Rellenar la paleta del bitmap
            for (int i = 0; i < 256; i++)
            {
                int paletteIndex = i * 4;
                Color color = Color.FromArgb(
                    palette[paletteIndex + 3],  // A
                    palette[paletteIndex],      // R
                    palette[paletteIndex + 1],  // G
                    palette[paletteIndex + 2]   // B
                );
                bmpPalette.Entries[i] = color;
            }

            // Asignar la paleta al bitmap
            bitmap.Palette = bmpPalette;

            // Bloquear los bits del bitmap para un acceso directo a la memoria
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            // Obtener el puntero al primer píxel del bitmap
            IntPtr ptr = bitmapData.Scan0;

            // Copiar los valores del array de píxeles al bitmap
            Marshal.Copy(pixels, 0, ptr, pixels.Length);

            // Desbloquear los bits del bitmap
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public static string CopyToFolder(string dstDirectory, string filePath)
        {
            // Verificar si el archivo de origen existe
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} dont exist.");
            }

            // Obtener el nombre del archivo
            string fileName = Path.GetFileName(filePath);

            // Combinar la ruta de destino con el nombre del archivo para obtener el nuevo camino
            string newFilePath = Path.Combine(dstDirectory, fileName);

            // Copiar el archivo al directorio de destino
            File.Copy(filePath, newFilePath);

            // Devolver el nuevo camino del archivo
            return newFilePath;
        }
        public static string CopyFolder(string sourceDirectory, string destDirectory)
        {
            // Verificar si el directorio de origen existe
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException($"The directory {sourceDirectory} does not exist.");
            }

            // Obtener información del directorio de origen
            DirectoryInfo dir = new DirectoryInfo(sourceDirectory);
            Console.WriteLine(dir.Name);
            // Obtener todos los archivos del directorio de origen y sus subdirectorios
            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
            destDirectory = Path.Combine(destDirectory, dir.Name);
            // Crear el directorio de destino si no existe
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            // Copiar todos los archivos y subdirectorios
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.FullName);
                Console.WriteLine(file.FullName.Substring(sourceDirectory.Length + 1));
                // Crear la ruta completa del archivo de destino
                string newFilePath = Path.Combine(destDirectory, file.FullName.Substring(sourceDirectory.Length + 1));

                // Crear el directorio de destino si no existe
                string directoryPath = Path.GetDirectoryName(newFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Copiar el archivo
                file.CopyTo(newFilePath, true);
            }

            // Devolver la ruta del directorio de destino
            return destDirectory;
        }

        public static string GetRelativePath(string fullPath, string prefixToRemove)
        {
            if (fullPath.StartsWith(prefixToRemove, StringComparison.OrdinalIgnoreCase))
            {
                return fullPath.Substring(prefixToRemove.Length);
            }
            else
            {
                throw new ArgumentException("The full path does not start with the specified prefix.");
            }
        }
        public static int ReadUInt32BE(byte[] buffer, int offset)
        {
            return (buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3];
        }

        public static int ReadUInt16BE(byte[] buffer, int offset)
        {
            return (buffer[offset] << 8) | buffer[offset + 1];
        }

        public static void WriteUInt32BE(byte[] p, int offset, long d)
        {
            p[offset] = (byte)(d >> 24);
            p[offset + 1] = (byte)(d >> 16);
            p[offset + 2] = (byte)(d >> 8);
            p[offset + 3] = (byte)d;
        }
        public static void WriteUInt16BE(byte[] p, int offset, short d)
        {
            p[offset] = (byte)(d >> 8);
            p[offset + 1] = (byte)d;
        }
        public static int ReadShorts(BinaryReader stream, short[] buffer, int offset, int count)
        {
            byte[] byteBuffer = new byte[count * sizeof(short)];
            int bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length);
            Buffer.BlockCopy(byteBuffer, 0, buffer, offset * sizeof(short), bytesRead);
            return bytesRead / sizeof(short);
        }
        public static byte[] ShortArrayToByteArray(short[] array, int length)
        {
            byte[] byteArray = new byte[length * 2];
            Buffer.BlockCopy(array, 0, byteArray, 0, length * 2);
            return byteArray;
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, $"{APP_NAME} Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, $"{APP_NAME}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, $"{APP_NAME} Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

    }
}
