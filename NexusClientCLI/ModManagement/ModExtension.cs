using Nexus.Client.Mods;
//using Nexus.Client.Mods.Formats.FOMod;
//using Nexus.Client.Mods.Formats.OMod;
using Nexus.Client.Util;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Extensions
{
	public static class Extension
	{
		public static T GetPrivateField<T>(this object obj, string name)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
			Type type = obj.GetType();
			FieldInfo field = type.GetField(name, flags);
			return (T)field.GetValue(obj);
		}


        public static T GetPrivateProperty<T>(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            PropertyInfo field = type.GetProperty(name, flags);
            return (T)field.GetValue(obj, null);
        }

		public static T CallPrivateMethod<T>(this object obj, string name, params object[] param)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
			Type type = obj.GetType();
			MethodInfo method = type.GetMethod(name, flags);
			return (T)method.Invoke(obj, param);
		}


        public static void ExtractFileContents(this Archive archive, string p_strPath, Stream p_outStream)
        {
			string strPath = p_strPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var fileInfo = archive.GetPrivateField<Dictionary<string, ArchiveFileInfo>>("m_dicFileInfo");
            if (!fileInfo.ContainsKey(strPath))
				throw new FileNotFoundException("The requested file does not exist in the archive.", p_strPath);

			ArchiveFileInfo afiFile = fileInfo[strPath];

            //check to see if we are on the same thread as the extractor
            // if not, then marshall the call to the extractor's thread.
            // this needs to be done as the 7zip dll cannot handle calls from other
            // threads.
            if (archive.GetPrivateProperty<bool>("IsReadonly"))
            {
                var roExtractor = archive.GetPrivateField<ThreadSafeSevenZipExtractor>("m_szeReadOnlyExtractor");
                if (roExtractor == null) {
                    string tempDir = archive.GetPrivateField<string>("m_strReadOnlyTempDirectory");
                    using (FileStream inStream = new FileStream(Path.Combine(tempDir, strPath), FileMode.Open, FileAccess.Read)) {
                        int size = 1024 * 1024;
                        byte[] buffer = new byte[size];
                        int read = 0;
                        while ((read = inStream.Read(buffer, 0, size)) > 0) {
                            p_outStream.Write(buffer, 0, read);
                        }
                    }
                }
                else
                {
                    roExtractor.ExtractFile(afiFile.Index, p_outStream);
                }
            }
            else
            {
                using (SevenZipExtractor szeExtractor = archive.CallPrivateMethod<SevenZipExtractor>("GetExtractor", archive.GetPrivateField<string>("m_strPath")))
                    szeExtractor.ExtractFile(afiFile.Index, p_outStream);
            }
        }

		/// <summary>
		/// write the specified file to a stream
		/// </summary>
		/// <param name="p_strFile">The file to retrieve.</param>
		/// <param name="p_outStream">The stream to write to.</param>
		/// <exception cref="System.IO.FileNotFoundException">Thrown if the specified file
		/// is not in the mod.</exception>
/*		public static void ExtractFileTo(this OMod mod, string p_strFile, Stream p_outStream)
		{
            byte[] data = mod.GetFile(p_strFile);
            p_outStream.Write(data, 0, data.Length);
		} */

		/// <summary>
		/// write the specified file to a stream
		/// </summary>
		/// <param name="p_strFile">The file to retrieve.</param>
		/// <param name="p_outStream">The stream to write to.</param>
		/// <exception cref="System.IO.FileNotFoundException">Thrown if the specified file
		/// is not in the mod.</exception>
		public static void ExtractFileTo<T>(this T mod, string p_strFile, Stream p_outStream)
		{
            string realPath = (string) mod.CallPrivateMethod<string>("GetRealPath", new object[] { p_strFile });
			if (!(bool) mod.GetType().GetMethod("ContainsFile").Invoke(mod, new object[] { realPath }))
			{
				if (Path.GetFileNameWithoutExtension(p_strFile).ToLower() == "screenshot")
				{
					byte[] data = (byte[])(new ImageConverter().ConvertTo(new Bitmap(1, 1), typeof(byte[])));
					p_outStream.Write(data, 0, data.Length);
					return;
				}
				else
					throw new FileNotFoundException("File doesn't exist in FOMod", p_strFile);
			}

            mod.GetPrivateField<Archive>("m_arcFile").ExtractFileContents(realPath, p_outStream);
		} 
	}
	class ModExtension
	{

   }
}


