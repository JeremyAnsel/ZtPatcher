using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace JeremyAnsel.Xwa.ExePatcher
{
    public sealed class Patcher
    {
        private const string xwaExeVersion = @"X-Wing Alliance\V2.0";

        private const int xwaExeVersionOffset = 0x200E19;

        private static readonly byte[] xwaExeVersionBytes = Encoding.ASCII.GetBytes(Patcher.xwaExeVersion);

        public IList<Patch> Patches { get; } = new List<Patch>();

        public static bool IsApplied(string? exeFileName, Patch? patch)
        {
            if (exeFileName == null)
            {
                throw new ArgumentNullException(nameof(exeFileName));
            }

            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            using FileStream file = File.Open(exeFileName, FileMode.Open, FileAccess.Read);

            return IsApplied(file, patch);
        }

        public static bool IsApplied(Stream? exeFile, Patch? patch)
        {
            if (exeFile == null)
            {
                throw new ArgumentNullException(nameof(exeFile));
            }

            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            FileVerifyExeVersion(exeFile);

            foreach (var item in patch.Items)
            {
                if (!FileCheckBytes(exeFile, item.Offset, item.NewValues))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CanBeApplied(string? exeFileName, Patch? patch)
        {
            if (exeFileName == null)
            {
                throw new ArgumentNullException(nameof(exeFileName));
            }

            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            using FileStream file = File.Open(exeFileName, FileMode.Open, FileAccess.Read);

            return CanBeApplied(file, patch);
        }

        public static bool CanBeApplied(Stream? exeFile, Patch? patch)
        {
            if (exeFile == null)
            {
                throw new ArgumentNullException(nameof(exeFile));
            }

            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            FileVerifyExeVersion(exeFile);

            foreach (var item in patch.Items)
            {
                if (!FileCheckBytes(exeFile, item.Offset, item.OldValues))
                {
                    return false;
                }
            }

            return true;
        }

        public static Patcher Read(string? fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using FileStream file = File.Open(fileName, FileMode.Open, FileAccess.Read);

            return Read(file);
        }

        public static Patcher Read(Stream? file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Patcher patcher = new();

            XmlDocument document = new();
            document.Load(file);

            var nodes = document.SelectNodes("/ArrayOfPatch/Patch");

            if (nodes is null)
            {
                return patcher;
            }

            foreach (XmlNode patchNode in nodes)
            {
                Patch patch = new()
                {
                    Name = patchNode.Attributes?.GetNamedItem("Name")?.Value ?? string.Empty
                };

                XmlNode? node = patchNode.Attributes?.GetNamedItem("Description");
                patch.Description = node?.Value ?? string.Empty;

                var patchNodes = patchNode.SelectNodes("Item");

                if (patchNodes is not null)
                {
                    foreach (XmlNode itemNode in patchNodes)
                    {
                        patch.Items.Add(new PatchItem
                        {
                            OffsetString = itemNode.Attributes?.GetNamedItem("Offset")?.Value ?? "0",
                            OldValuesString = itemNode.Attributes?.GetNamedItem("From")?.Value ?? string.Empty,
                            NewValuesString = itemNode.Attributes?.GetNamedItem("To")?.Value ?? string.Empty
                        });
                    }
                }

                patcher.Patches.Add(patch);
            }

            return patcher;
        }

        public void Write(string? fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using FileStream file = File.Open(fileName, FileMode.Create, FileAccess.Write);

            Write(file);
        }

        public void Write(Stream? file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            XmlDocument document = new();
            document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement rootNode = document.CreateElement("ArrayOfPatch");

            foreach (Patch patch in this.Patches)
            {
                XmlElement patchNode = document.CreateElement("Patch");

                patchNode.SetAttribute("Name", patch.Name);

                if (!string.IsNullOrEmpty(patch.Description))
                {
                    patchNode.SetAttribute("Description", patch.Description);
                }

                foreach (PatchItem item in patch.Items)
                {
                    XmlElement itemNode = document.CreateElement("Item");

                    itemNode.SetAttribute("Offset", item.OffsetString);
                    itemNode.SetAttribute("From", item.OldValuesString);
                    itemNode.SetAttribute("To", item.NewValuesString);

                    patchNode.AppendChild(itemNode);
                }

                rootNode.AppendChild(patchNode);
            }

            document.AppendChild(rootNode);
            document.Save(file);
        }

        public void Apply(string? exeFileName, bool writeNewValues)
        {
            if (exeFileName == null)
            {
                throw new ArgumentNullException(nameof(exeFileName));
            }

            using FileStream file = File.Open(exeFileName, FileMode.Open, FileAccess.ReadWrite);

            Apply(file, writeNewValues);
        }

        public void Apply(Stream? exeFile, bool writeNewValues)
        {
            if (exeFile == null)
            {
                throw new ArgumentNullException(nameof(exeFile));
            }

            FileVerifyExeVersion(exeFile);

            foreach (var patch in this.Patches)
            {
                foreach (var item in patch.Items)
                {
                    if (writeNewValues)
                    {
                        if (!FileCheckBytes(exeFile, item.Offset, item.OldValues))
                        {
                            throw new InvalidDataException();
                        }
                    }
                    else
                    {
                        if (!FileCheckBytes(exeFile, item.Offset, item.NewValues))
                        {
                            throw new InvalidDataException();
                        }
                    }
                }

                foreach (var item in patch.Items)
                {
                    if (writeNewValues)
                    {
                        exeFile.Seek(item.Offset, SeekOrigin.Begin);
                        exeFile.Write(item.NewValues, 0, item.NewValues.Length);
                    }
                    else
                    {
                        exeFile.Seek(item.Offset, SeekOrigin.Begin);
                        exeFile.Write(item.OldValues, 0, item.OldValues.Length);
                    }
                }
            }
        }

        private static void FileVerifyExeVersion(Stream file)
        {
            bool found = false;

            try
            {
                if (FileCheckBytes(file, Patcher.xwaExeVersionOffset, Patcher.xwaExeVersionBytes))
                {
                    found = true;
                }
            }
            catch
            {
            }

            if (!found)
            {
                throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "{0} not found", Patcher.xwaExeVersion));
            }
        }

        private static bool FileCheckBytes(Stream file, int offset, byte[] bytes)
        {
            byte[] buffer = new byte[bytes.Length];

            file.Seek(offset, SeekOrigin.Begin);
            file.Read(buffer, 0, bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                if (buffer[i] != bytes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
