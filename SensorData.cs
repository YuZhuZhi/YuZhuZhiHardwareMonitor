using System;
using System.Collections.Generic;
using System.Text;
using LibreHardwareMonitor.Hardware;

namespace Hardware_Monitor
{
    public class SensorData : IDisposable
    {
        private float _CPUTemperature;
        public float CPUTemperature {
            get { return _CPUTemperature; }
            set { _CPUTemperature = value; }
        }

        private float _CPULoad;
        public float CPULoad {
            get { return _CPULoad; }
            set { _CPULoad = value; }
        }

        private float _CPUSpeed;
        public float CPUSpeed {
            get { return _CPUSpeed; }
            set { _CPUSpeed = value; }
        }

        private float _CPUPower;
        public float CPUPower {
            get { return _CPUPower; }
            set { _CPUPower = value; }
        }

        private float _CPUVoltage;
        public float CPUVoltage {
            get { return _CPUVoltage; }
            set { _CPUVoltage = value; }
        }

        private int _CPUFanSpeed;
        public int CPUFanSpeed {
            get { return _CPUFanSpeed; }
            set { _CPUFanSpeed = value; }
        }

        private float _GPUTemperature;
        public float GPUTemperature {
            get { return _GPUTemperature; }
            set { _GPUTemperature = value; }
        }

        private float _GPULoad;
        public float GPULoad {
            get { return _GPULoad; }
            set { _GPULoad = value; }
        }

        private float _GPUPower;
        public float GPUPower {
            get { return _GPUPower; }
            set { _GPUPower = value; }
        }

        private int _GPUFanSpeed;
        public int GPUFanSpeed {
            get { return _GPUFanSpeed; }
            set { _GPUFanSpeed = value; }
        }

        private float _MemoryUsed;
        public float MemoryUsed {
            get { return _MemoryUsed; }
            set { _MemoryUsed = value; }
        }

        private float _MemoryLoad;
        public float MemoryLoad {
            get { return _MemoryLoad; }
            set { _MemoryLoad = value; }
        }

        private float _BatteryPower;

        public float BatteryPower {
            get { return _BatteryPower; }
            set { _BatteryPower = value; }
        }

        private float _MotherBoardTemperature;
        public float MotherBoardTemperature {
            get { return _MotherBoardTemperature; }
            set { _MotherBoardTemperature = value; }
        }

        private readonly Computer computer;
        private readonly bool hasDiscreteGpu;
        private readonly bool hasCPUCoreAvgTemp;
        private readonly bool hasCPUPackageTemp;
        private readonly bool hasGPUHotSpotTemp;
        private readonly bool hasGPUCoreTemp;

        public SensorData() {
            computer = new Computer {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsBatteryEnabled = true,
                IsMotherboardEnabled = true
            };
            computer.Open();

            hasDiscreteGpu = false;
            foreach (var hw in computer.Hardware) {
                if (hw.HardwareType == HardwareType.GpuAmd || hw.HardwareType == HardwareType.GpuNvidia) {
                    hasDiscreteGpu = true;
                }

                hw.Update();
                foreach (var sensor in hw.Sensors) {
                    switch (hw.HardwareType) {
                        case HardwareType.Cpu:
                            if (sensor.SensorType == SensorType.Temperature) {
                                if (sensor.Name.Contains("Package", StringComparison.OrdinalIgnoreCase))
                                    hasCPUPackageTemp = true;
                                else if (sensor.Name.Contains("Core Average", StringComparison.OrdinalIgnoreCase))
                                    hasCPUCoreAvgTemp = true;
                            }
                            break;

                        case HardwareType.GpuAmd:
                        case HardwareType.GpuNvidia:
                        case HardwareType.GpuIntel:
                            if (sensor.SensorType == SensorType.Temperature) {
                                if (sensor.Name.Contains("Hot Spot", StringComparison.OrdinalIgnoreCase))
                                    hasGPUHotSpotTemp = true;
                                else if (sensor.Name.Contains("Core", StringComparison.OrdinalIgnoreCase))
                                    hasGPUCoreTemp = true;
                            }
                            break;
                    }
                }
            }
        }

        ~SensorData() {
            computer.Close();
        }

        public void Dispose() {
            computer.Close();
        }

        public void RefreshAll() {
            //foreach (var hw in computer.Hardware) {
            //    hw.Update();
            //    Console.WriteLine($"[HW] {hw.HardwareType} - {hw.Name}");
            //    foreach (var sensor in hw.Sensors) {
            //        Console.WriteLine($"  {sensor.SensorType} | {sensor.Name} | {sensor.Value}");
            //    }
            //}

            foreach (var hardware in computer.Hardware) {
                hardware.Update();

                switch (hardware.HardwareType) {
                    case HardwareType.Cpu:
                        UpdateCpuSensors(hardware);
                        break;

                    case HardwareType.GpuAmd:
                    case HardwareType.GpuNvidia:
                        UpdateGpuSensors(hardware);
                        break;

                    case HardwareType.GpuIntel:
                        if (!hasDiscreteGpu) UpdateGpuSensors(hardware);
                        break;

                    case HardwareType.Memory:
                        UpdateMemorySensors(hardware);
                        break;

                    case HardwareType.Battery:
                        UpdateBatterySensors(hardware);
                        break;

                    case HardwareType.Motherboard:
                        UpdateMotherboardSensors(hardware);
                        break;
                }

                foreach (var sub in hardware.SubHardware) {
                    sub.Update();
                    if (sub.HardwareType == HardwareType.Motherboard)
                        UpdateMotherboardSensors(sub);
                }
            }
        }

        private void UpdateCpuSensors(IHardware cpu) {
            foreach (var sensor in cpu.Sensors) {
                switch (sensor.SensorType) {
                    case SensorType.Temperature:
                        if (hasCPUPackageTemp)
                            CPUTemperature = sensor.Value ?? 0;
                        else if (hasCPUCoreAvgTemp)
                            CPUTemperature = sensor.Value ?? 0;
                        else
                            CPUTemperature = 0;
                        break;

                    case SensorType.Load:
                        if (sensor.Name.Contains("Total", StringComparison.OrdinalIgnoreCase))
                            CPULoad = sensor.Value ?? 0;
                        break;

                    case SensorType.Clock:
                        if (sensor.Name.Contains("Core #1", StringComparison.OrdinalIgnoreCase))
                            CPUSpeed = sensor.Value ?? 0;
                        break;

                    case SensorType.Fan:
                        if (sensor.Name.Contains("CPU", StringComparison.OrdinalIgnoreCase))
                            CPUFanSpeed = (int)(sensor.Value ?? 0);
                        break;

                    case SensorType.Power:
                        if (sensor.Name.Contains("Package", StringComparison.OrdinalIgnoreCase))
                            CPUPower = (sensor.Value ?? 0);
                        break;

                    case SensorType.Voltage:
                        if (sensor.Name.Contains("Core", StringComparison.OrdinalIgnoreCase))
                            CPUVoltage = sensor.Value ?? 0;
                        break;
                }
            }
        }

        private void UpdateGpuSensors(IHardware gpu) {
            foreach (var sensor in gpu.Sensors) {
                switch (sensor.SensorType) {
                    case SensorType.Temperature:
                        if (hasGPUCoreTemp)
                            GPUTemperature = sensor.Value ?? 0;
                        else if (hasGPUHotSpotTemp)
                            GPUTemperature = sensor.Value ?? 0;
                        else
                            GPUTemperature = 0;
                        break;

                    case SensorType.Load:
                        if (sensor.Name.Contains("Core", StringComparison.OrdinalIgnoreCase))
                            GPULoad = sensor.Value ?? 0;
                        break;

                    case SensorType.Power:
                        if (sensor.Name.Contains("GPU Package", StringComparison.OrdinalIgnoreCase))
                            GPUPower = sensor.Value ?? 0;
                        break;

                    case SensorType.Fan:
                        if (sensor.Name.Contains("GPU", StringComparison.OrdinalIgnoreCase))
                            GPUFanSpeed = (int)(sensor.Value ?? 0);
                        break;
                }
            }
        }

        private void UpdateMotherboardSensors(IHardware board) {
            foreach (var sensor in board.Sensors) {
                if (sensor.SensorType == SensorType.Temperature &&
                    sensor.Name.Contains("Board", StringComparison.OrdinalIgnoreCase)) {
                    MotherBoardTemperature = sensor.Value ?? 0;
                }
            }
        }

        private void UpdateMemorySensors(IHardware memory) {
            foreach (var sensor in memory.Sensors) {
                switch (sensor.SensorType) {
                    case SensorType.Load:
                        if (sensor.Name.Equals("Memory", StringComparison.OrdinalIgnoreCase))
                            MemoryLoad = sensor.Value ?? 0;
                        break;

                    case SensorType.Data:
                        if (sensor.Name.Equals("Memory Used", StringComparison.OrdinalIgnoreCase))
                            MemoryUsed = sensor.Value ?? 0;
                        break;
                }
            }
        }

        private void UpdateBatterySensors(IHardware battery) {
            foreach (var sensor in battery.Sensors) {
                if (sensor.SensorType == SensorType.Power)
                    BatteryPower = sensor.Value ?? 0;
            }
        }

    }
}
