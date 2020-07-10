using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ZtPatcher
{
    class PatchItemConverter : BaseConverter, IValueConverter
    {
        private static readonly List<ExeSection> exeSections = new List<ExeSection>
        {
            new ExeSection("Species", 232 + 1, 0x02, 0x005B0F70 - 0x00401400),
            new ExeSection("Object", 557, 0x18, 0x005FB240 - 0x00401400),
            new ExeSection("Craft", 265, 0x3DB, 0x005BB480 - 0x00401400),
            new ExeSection("HullIcon", 72, 0x02, 0x005A9608 - 0x00401000),
            new ExeSection("MapIcon", 222, 0x02, 0x005A9AD8 - 0x00401000),
        };

        public PatchItemConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (KeyValuePair<int, byte[]>)value;

            return string.Format(CultureInfo.InvariantCulture, "{0:X8} {1} V={2}", item.Key, GetExeSectionString(item.Key), BitConverter.ToString(item.Value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetExeSectionString(int offset)
        {
            foreach (ExeSection section in exeSections)
            {
                if (offset >= section.Offset && offset < (section.Offset + section.TableSize * section.EntrySize))
                {
                    int delta = offset - section.Offset;

                    int entryIndex = delta / section.EntrySize;
                    int entryOffset = delta % section.EntrySize;

                    return string.Format(CultureInfo.InvariantCulture, "{0} I={1} O={2}", section.Name, entryIndex, entryOffset);
                }
            }

            return string.Empty;
        }
    }
}
