﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Iot.Device.CpuTemperature
{
    public class CpuTemperature
    {
        private bool _isAvalable = false;
        private bool _checkedIfAvailable = false;

        public double Temperature => ReadTemperature();
        public bool IsAvailable => CheckAvailable();

        private bool CheckAvailable()
        {
            if (!_checkedIfAvailable)
            {                
                _checkedIfAvailable = true;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && File.Exists("/sys/class/thermal/thermal_zone0/temp"))
                {
                    _isAvalable = true;
                }
            }

            return _isAvalable;
        }

        private double ReadTemperature()
        {
            double temperature = double.NaN;

            if (CheckAvailable())
            {
                using (FileStream fileStream = new FileStream("/sys/class/thermal/thermal_zone0/temp", FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string data = reader.ReadLine();
                    if (!string.IsNullOrEmpty(data))
                    {
                        int temp;
                        if (int.TryParse(data, out temp))
                        {
                            temperature = temp / 1000F;
                        }
                    }
                }
            }

            return temperature;
        }
    }
}
