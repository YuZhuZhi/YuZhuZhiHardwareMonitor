using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hardware_Monitor.ViewModel
{
    public class SensorDataVM : INotifyPropertyChanged
    {
        private readonly SensorData _sensorData;

        public string DisplayCPUTemperature => FormatTemperatureValue(CPUTemperature);
        public string DisplayGPUTemperature => FormatTemperatureValue(GPUTemperature);
        public string DisplayCPUPower => $"{CPUPower} W";
        public string DisplayGPUPower => $"{GPUPower} W";
        public string DisplayMemoryUsed => $"{MemoryUsed:0.0} GB";

        private string FormatTemperatureValue(float value) {
            if (value < 100)
                return value.ToString("0.0"); // 一位小数
            else
                return ((int)value).ToString(); // 整数
        }

        public SensorDataVM() {
            _sensorData = new SensorData();
            Refresh();

            try {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HardwareList.log");
                using (var writer = new StreamWriter(logPath, false, Encoding.UTF8)) {
                    writer.WriteLine($"[Hardware Scan Log] Generated Time: {DateTime.Now}");
                    writer.WriteLine("------------------------------------------------------------");

                    var computerField = typeof(SensorData).GetField("computer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (computerField != null) {
                        var computer = computerField.GetValue(_sensorData) as LibreHardwareMonitor.Hardware.Computer;
                        if (computer != null) {
                            foreach (var hw in computer.Hardware) {
                                hw.Update();
                                writer.WriteLine($"[HW] {hw.HardwareType} - {hw.Name}");

                                foreach (var sensor in hw.Sensors) {
                                    writer.WriteLine($"  {sensor.SensorType,-12} | {sensor.Name,-40} | {sensor.Value,8:F2}");
                                }

                                foreach (var sub in hw.SubHardware) {
                                    sub.Update();
                                    writer.WriteLine($"  [SubHW] {sub.HardwareType} - {sub.Name}");
                                    foreach (var sensor in sub.Sensors) {
                                        writer.WriteLine($"    {sensor.SensorType,-12} | {sensor.Name,-40} | {sensor.Value,8:F2}");
                                    }
                                }

                                writer.WriteLine();
                            }
                        }
                    }

                    writer.WriteLine("------------------------------------------------------------");
                }
            } catch (Exception ex) {
                Console.WriteLine($"Generate HardwareList.log Failed: {ex.Message}");
            }
        }

        public void Refresh() {
            _sensorData.RefreshAll();

            CPUTemperature = _sensorData.CPUTemperature;
            GPUTemperature = _sensorData.GPUTemperature;
            CPULoad = (int)Math.Round(_sensorData.CPULoad);
            GPULoad = (int)Math.Round(_sensorData.GPULoad);
            CPUPower = (int)Math.Round(_sensorData.CPUPower);
            GPUPower = (int)Math.Round(_sensorData.GPUPower);
            MemoryLoad = (int)Math.Round(_sensorData.MemoryLoad);
            MemoryUsed = _sensorData.MemoryUsed;

            // 通知显示属性更新
            OnPropertyChanged(nameof(DisplayCPUTemperature));
            OnPropertyChanged(nameof(DisplayGPUTemperature));
            OnPropertyChanged(nameof(DisplayCPUPower));
            OnPropertyChanged(nameof(DisplayGPUPower));
            OnPropertyChanged(nameof(DisplayMemoryUsed));

        }

        private float _cpuTemp;
        public float CPUTemperature {
            get => _cpuTemp;
            private set { _cpuTemp = value; OnPropertyChanged(); }
        }

        private float _cpuLoad;
        public float CPULoad {
            get => _cpuLoad;
            private set { _cpuLoad = value; OnPropertyChanged(); }
        }

        private float _cpuPower;
        public float CPUPower {
            get => _cpuPower;
            private set { _cpuPower = value; OnPropertyChanged(); }
        }

        private float _gpuTemp;
        public float GPUTemperature {
            get => _gpuTemp;
            private set { _gpuTemp = value; OnPropertyChanged(); }
        }

        private float _gpuLoad;
        public float GPULoad {
            get => _gpuLoad;
            private set { _gpuLoad = value; OnPropertyChanged(); }
        }

        private float _gpuPower;
        public float GPUPower {
            get => _gpuPower;
            private set { _gpuPower = value; OnPropertyChanged(); }
        }

        private float _memoryLoad;
        public float MemoryLoad {
            get => _memoryLoad;
            private set { _memoryLoad = value; OnPropertyChanged(); }
        }

        private float _memoryUsed;
        public float MemoryUsed {
            get => _memoryUsed;
            private set { _memoryUsed = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
