using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zt;

namespace ZtBlank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SpeciesTableSize = 232 + 1;

        private const int SpeciesTableEntrySize = 0x02;

        private const int SpeciesTableOffset = 0x005B0F70 - 0x00401400;

        private const int ObjectTableSize = 557;

        private const int ObjectTableEntrySize = 0x18;

        private const int ObjectTableOffset = 0x005FB240 - 0x00401400;

        private const int CraftTableSize = 265;

        private const int CraftTableEntrySize = 0x3DB;

        private const int CraftTableOffset = 0x005BB480 - 0x00401400;

        private int speciesSlot;

        private int objectSlot;

        private int craftSlot;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public bool SpeciesSlotSelected { get; set; } = true;

        public int SpeciesSlot
        {
            get
            {
                return this.speciesSlot;
            }

            set
            {
                if (value < 0 || value >= SpeciesTableSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.speciesSlot = value;
            }
        }

        public bool ObjectSlotSelected { get; set; } = true;

        public int ObjectSlot
        {
            get
            {
                return this.objectSlot;
            }

            set
            {
                if (value < 0 || value >= ObjectTableSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.objectSlot = value;
            }
        }

        public bool CraftSlotSelected { get; set; } = true;

        public int CraftSlot
        {
            get
            {
                return this.craftSlot;
            }

            set
            {
                if (value < 0 || value >= CraftTableSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.craftSlot = value;
            }
        }

        private string GetOpenFileName()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "XWingAlliance.exe|*.exe",
                FileName = "XWingAlliance.exe"
            };

            if (dialog.ShowDialog(this) == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private string GetSaveFileName(string name)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "202",
                FileName = name + ".202"
            };

            if (dialog.ShowDialog(this) == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private static void BlankArray(byte[] bytes, byte value)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = value;
            }
        }

        private ZtFile CreateBlankPatch(byte value)
        {
            var zt = new ZtFile();
            var sb = new StringBuilder();
            sb.AppendLine("Blank patch: " + value.ToString(CultureInfo.InvariantCulture));

            if (this.SpeciesSlotSelected)
            {
                sb.AppendLine("Species Slot: " + this.SpeciesSlot.ToString(CultureInfo.InvariantCulture));

                var bytes = new byte[SpeciesTableEntrySize];
                BlankArray(bytes, value);
                zt.Patches.Add(SpeciesTableOffset + this.SpeciesSlot * SpeciesTableEntrySize, bytes);
            }

            if (this.ObjectSlotSelected)
            {
                sb.AppendLine("Object Slot: " + this.ObjectSlot.ToString(CultureInfo.InvariantCulture));

                var bytes = new byte[ObjectTableEntrySize];
                BlankArray(bytes, value);
                zt.Patches.Add(ObjectTableOffset + this.ObjectSlot * ObjectTableEntrySize, bytes);
            }

            if (this.CraftSlotSelected)
            {
                sb.AppendLine("Craft Slot: " + this.CraftSlot.ToString(CultureInfo.InvariantCulture));

                var bytes = new byte[CraftTableEntrySize];
                BlankArray(bytes, value);
                zt.Patches.Add(CraftTableOffset + this.CraftSlot * CraftTableEntrySize, bytes);
            }

            zt.Comment = sb.ToString();

            return zt;
        }

        private ZtFile CreateFillPatch()
        {
            string exeFileName = GetOpenFileName();

            if (string.IsNullOrEmpty(exeFileName))
            {
                return null;
            }

            var exeBytes = File.ReadAllBytes(exeFileName);

            var zt = new ZtFile();
            var sb = new StringBuilder();
            sb.AppendLine("Fill patch");

            if (this.SpeciesSlotSelected)
            {
                sb.AppendLine("Species Slot: " + this.SpeciesSlot.ToString(CultureInfo.InvariantCulture));

                int start = SpeciesTableOffset + this.SpeciesSlot * SpeciesTableEntrySize;
                int size = SpeciesTableEntrySize;
                var bytes = new byte[size];
                Array.Copy(exeBytes, start, bytes, 0, size);

                zt.Patches.Add(start, bytes);
            }

            if (this.ObjectSlotSelected)
            {
                sb.AppendLine("Object Slot: " + this.ObjectSlot.ToString(CultureInfo.InvariantCulture));

                int start = ObjectTableOffset + this.ObjectSlot * ObjectTableEntrySize;
                int size = ObjectTableEntrySize;
                var bytes = new byte[size];
                Array.Copy(exeBytes, start, bytes, 0, size);

                zt.Patches.Add(start, bytes);
            }

            if (this.CraftSlotSelected)
            {
                sb.AppendLine("Craft Slot: " + this.CraftSlot.ToString(CultureInfo.InvariantCulture));

                int start = CraftTableOffset + this.CraftSlot * CraftTableEntrySize;
                int size = CraftTableEntrySize;
                var bytes = new byte[size];
                Array.Copy(exeBytes, start, bytes, 0, size);

                zt.Patches.Add(start, bytes);
            }

            zt.Comment = sb.ToString();

            return zt;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ZtFile zt;
            string name;

            if (this.selectBlank0Button.IsChecked == true)
            {
                zt = this.CreateBlankPatch(0);
                name = "Blank0";
            }
            else if (this.selectBlank9Button.IsChecked == true)
            {
                zt = this.CreateBlankPatch(9);
                name = "Blank9";
            }
            else if (this.selectFillButton.IsChecked == true)
            {
                zt = this.CreateFillPatch();
                name = "Fill";
            }
            else
            {
                return;
            }

            if (zt == null)
            {
                return;
            }

            string fileName = GetSaveFileName(name);

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            zt.Save(fileName);

            MessageBox.Show("Saved");
        }
    }
}
