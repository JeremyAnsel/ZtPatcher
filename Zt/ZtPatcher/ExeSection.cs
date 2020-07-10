using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZtPatcher
{
    class ExeSection
    {
        public ExeSection(string name, int tableSize, int entrySize, int offset)
        {
            this.Name = name;
            this.TableSize = tableSize;
            this.EntrySize = entrySize;
            this.Offset = offset;
        }

        public string Name { get; }

        public int TableSize { get; }

        public int EntrySize { get; }

        public int Offset { get; }
    }
}
