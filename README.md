# YuZhuZhi Hardware Monitor

一款基于 C# 和 WPF 开发的轻量级现代化界面硬件监控工具，由 YuZhuZhi 开发。

## 功能特点——核心监控指标

- CPU 监控：温度、负载、功耗
- GPU 监控：温度、负载、功耗
- 内存监控：使用率、已用容量
- 时间显示：实时时钟

## 安装使用

在右侧 Release 页面下载 .zip 压缩包，解压后运行 `HardwareMonitor.exe` 即可。

由于设备适配不足，可能在部分设备上无法正常显示温度。请多提 Issue ，包含您的 CPU 和 GPU 型号，以提交 .exe 所在目录下的 `HardwareList.log`为宜，以便后续改进。 

目前已知问题：

1. 对于使用 Intel 核显的设备，由于不存在相关传感器，因此只能显示为 CPU 温度。
2. 如果 CPU 温度为 0℃，请重启软件，选择以管理员身份运行。
