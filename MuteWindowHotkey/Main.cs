using System.Diagnostics;
using System.Text.Json;
using CSCore.CoreAudioAPI;

namespace MuteWindowHotkey
{
    public partial class Main : Form
    {
        public static bool isClosed { get; set; } = false;
        public XInputController Joy { get; set; } = new XInputController();
        public Dictionary<string, string> Mappings { get; set; } = new Dictionary<string, string>();
        Thread ControllerThread { get; set; }
        public Main()
        {
            InitializeComponent();
        }
        private void ControllerThreadFunc()
        {
            var strippedMappings = new Dictionary<string, string>();
            var flags = new Dictionary<string, bool>();
            foreach (var mapping in Mappings)
            {
                if (mapping.Value != "")
                {
                    strippedMappings.Add(mapping.Key.Substring(8), mapping.Value);
                    flags.Add(mapping.Key.Substring(8), false);
                }
            }
            while (true)
            {
                Joy.Update();
                CheckMappings(strippedMappings, flags);
                if (isClosed)
                    break;
            }
        }
        void CheckMappings(Dictionary<string, string> strippedMappings, Dictionary<string, bool> flags)
        {
            foreach (var mapping in strippedMappings)
            {
                if (Joy.Buttons[mapping.Key] != flags[mapping.Key])
                {
                    flags[mapping.Key] = Joy.Buttons[mapping.Key];
                    MuteWindow(mapping.Value, Joy.Buttons[mapping.Key]);
                }
            }
        }
        private void MainLoad(object sender, EventArgs e)
        {
            FormClosing += (sender, e) => isClosed = true;
            UpdateProcessSelection();
            WriteConfigToBoxes();
            RestartControllerThread();
        }
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateProcessSelection();
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            UpdateConfig();
            RestartControllerThread();
        }
        private void buttonHide_Click(object sender, EventArgs e)
        {
            HideToTray();
        }
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon.Visible = false;
        }
        void UpdateConfig()
        {
            Mappings = new Dictionary<string, string>();
            foreach (var control in Controls)
            {
                if (control is ComboBox)
                {
                    var comboBox = (ComboBox)control;
                    Mappings.Add(comboBox.Name, comboBox.GetItemText(comboBox.SelectedItem ?? ""));
                }
            }
            File.WriteAllText("mappings.json", JsonSerializer.Serialize(Mappings, new JsonSerializerOptions() { WriteIndented = true }));
        }
        public void LoadConfig()
        {
            // Make file if not exists
            if (!File.Exists("mappings.json"))
                UpdateConfig();

            Mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("mappings.json"));
        }
        void WriteConfigToBoxes()
        {
            foreach (var mapping in Mappings)
            {
                var box = Controls.Find(mapping.Key, false)[0] as ComboBox;
                box.SelectedItem = mapping.Value;
            }
        }
        public void HideToTray()
        {
            notifyIcon.Visible = true;
            Hide();
        }
        public void RestartControllerThread()
        {
            ControllerThread = new Thread(ControllerThreadFunc);
            ControllerThread.IsBackground = true;
            ControllerThread.Start();
        }
        void UpdateProcessSelection()
        {
            comboBoxA.Items.Clear();
            comboBoxB.Items.Clear();
            comboBoxX.Items.Clear();
            comboBoxY.Items.Clear();
            comboBoxStart.Items.Clear();
            comboBoxSelect.Items.Clear();
            comboBoxRB.Items.Clear();
            comboBoxLB.Items.Clear();
            comboBoxDPadUp.Items.Clear();
            comboBoxDPadDown.Items.Clear();
            comboBoxDPadLeft.Items.Clear();
            comboBoxDPadRight.Items.Clear();
            comboBoxLeftStick.Items.Clear();
            comboBoxRightStick.Items.Clear();
            comboBoxLT.Items.Clear();
            comboBoxRT.Items.Clear();
            foreach (var process in Process.GetProcesses())
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    comboBoxA.Items.Add(process.ProcessName);
                    comboBoxB.Items.Add(process.ProcessName);
                    comboBoxX.Items.Add(process.ProcessName);
                    comboBoxY.Items.Add(process.ProcessName);
                    comboBoxStart.Items.Add(process.ProcessName);
                    comboBoxSelect.Items.Add(process.ProcessName);
                    comboBoxRB.Items.Add(process.ProcessName);
                    comboBoxLB.Items.Add(process.ProcessName);
                    comboBoxDPadUp.Items.Add(process.ProcessName);
                    comboBoxDPadDown.Items.Add(process.ProcessName);
                    comboBoxDPadLeft.Items.Add(process.ProcessName);
                    comboBoxDPadRight.Items.Add(process.ProcessName);
                    comboBoxLeftStick.Items.Add(process.ProcessName);
                    comboBoxRightStick.Items.Add(process.ProcessName);
                    comboBoxLT.Items.Add(process.ProcessName);
                    comboBoxRT.Items.Add(process.ProcessName);
                }
            }
        }
        void MuteWindow(string processName, bool isPressed)
        {
            foreach (AudioSessionManager2 sessionManager in GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (sessionManager)
                {
                    using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                    {
                        foreach (var session in sessionEnumerator)
                        {
                            using var simpleVolume = session.QueryInterface<SimpleAudioVolume>();
                            using var sessionControl = session.QueryInterface<AudioSessionControl2>();
                            Console.WriteLine((sessionControl.Process.ProcessName, sessionControl.SessionIdentifier));
                            if (Process.GetProcessById(sessionControl.ProcessID).ProcessName.Equals(processName))
                            {
                                simpleVolume.IsMuted = isPressed;
                            }
                        }
                    }
                }
            }
        }
        private static IEnumerable<AudioSessionManager2> GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using var enumerator = new MMDeviceEnumerator();
            using var devices = enumerator.EnumAudioEndpoints(dataFlow, DeviceState.Active);
            foreach (var device in devices)
            {
                Console.WriteLine("Device: " + device.FriendlyName);
                var sessionManager = AudioSessionManager2.FromMMDevice(device);
                yield return sessionManager;
            }
        }
    }
}