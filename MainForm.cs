using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EEPROM_Gen
{
    public partial class MainForm : Form
    {
        public static ListBoxLog _listBoxLog;
        public MainForm()
        {
            InitializeComponent();
            _listBoxLog = new ListBoxLog(listBox1);
        }

        private void buttonGenSerial_Click(object sender, EventArgs e)
        {
            textBoxSerial.Text  = DateTime.Now.ToString("yyMMddHHmmss");
        }

        private void buttonSigFile_Click(object sender, EventArgs e)
        {

        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogZIP.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowserDialogZIP.SelectedPath;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                string folder = textBoxFolder.Text;
                if (!Directory.Exists(folder))
                {
                    MessageBox.Show("Folder Doesn't Exist: " + folder);
                    return;
                }
                TarUtils.CreateTar("cape-info.tgz", folder);

                EEPROMData eeprom = new EEPROMData();
                eeprom.TARFilePath = folder + "\\" + "cape-info.tgz";
                eeprom.Name = textBoxName.Text;
                eeprom.SerialNumber = textBoxSerial.Text;
                eeprom.Version = Decimal.ToDouble(numericUpDownVersion.Value);

                eeprom.WriteFile(textBoxName.Text +"_"+ numericUpDownVersion.Value.ToString("N1") +  ".bin");
            }
            catch (Exception ex)
            {
                LogLine(Level.Error, "Error: " + ex.Message);
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Serial = textBoxSerial.Text;
            Properties.Settings.Default.Name = textBoxName.Text;
            Properties.Settings.Default.Folder = textBoxFolder.Text;
            Properties.Settings.Default.Version = Decimal.ToDouble(numericUpDownVersion.Value);
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            textBoxSerial.Text = Properties.Settings.Default.Serial;
            textBoxName.Text = Properties.Settings.Default.Name;
            textBoxFolder.Text = Properties.Settings.Default.Folder;
            numericUpDownVersion.Value = (decimal)Properties.Settings.Default.Version;
        }
        private void LogLine(Level lvl, string line)
        {
            _listBoxLog.Log(lvl, line);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }
    }
}
