using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace Qosmetics.Core
{
    public static class AdbUtils
    {
        static QosmeticsProjectSettings _projectSettings;
        public static void EnsureSettings()
        {
            if (_projectSettings == null)
                _projectSettings = QosmeticsProjectSettings.GetOrCreateSettings();
        }
        public static bool ValidateAdb()
        {
            EnsureSettings();
            return _projectSettings.IsAdbValid();
        }

        public static bool IsConnected()
        {
            EnsureSettings();
            var process = new Process();
            process.StartInfo.FileName = _projectSettings.AdbExecutable;
            process.StartInfo.Arguments = "devices";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();

            int idx = output.LastIndexOf("device");
            if (idx < 0) return false;
            if (output[idx + 6] == 's')
                return false;
            return true;
        }

        public static void Push(string from, string to)
        {
            EnsureSettings();
            var process = new Process();
            process.StartInfo.FileName = _projectSettings.AdbExecutable;
            process.StartInfo.Arguments = $"push {from} {to}";
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }

        public static void Connect()
        {
            if (!IsConnected())
            {
                if (_projectSettings.Wireless)
                {
                    var process = new Process();
                    process.StartInfo.FileName = _projectSettings.AdbExecutable;
                    process.StartInfo.Arguments = $"tcpip 5555";
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process = new Process();
                    process.StartInfo.FileName = _projectSettings.AdbExecutable;
                    process.StartInfo.Arguments = $"connect {_projectSettings.QuestIPAddress}:5555";
                    process.StartInfo.UseShellExecute = false;
                }
            }
        }
    }
}