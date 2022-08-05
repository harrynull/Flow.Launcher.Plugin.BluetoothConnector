using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;

namespace Flow.Launcher.Plugin.BluetoothConnector
{
    public class BluetoothConnector : IPlugin
    {
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var client = new BluetoothClient();
            var searchTerm = query.Search.ToLower();
            // find paired devices
            var devices = client.DiscoverDevices(255, authenticated: true, remembered: true, unknown: false);
            return devices
                .Where(device => searchTerm.Length == 0 || device.DeviceName.ToLower().Contains(searchTerm))
                .OrderBy(device => device.Connected)
                .Select(device => new Result {
                    Title = device.DeviceName,
                    SubTitle = device.Connected ? "Connected" : "Disconnected",
                    Action = e =>
                    {
                        if (!device.Connected) {
                            client.Connect(device.DeviceAddress, device.InstalledServices[0]);
                        }
                        return true;
                    }
                }).ToList();
        }
    }
}