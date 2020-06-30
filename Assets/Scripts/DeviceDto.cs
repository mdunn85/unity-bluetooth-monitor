using System;
using System.Text;

namespace DefaultNamespace
{
    [Serializable]
    public class Device
    {
        public string name;
        public string id;
    }
    
    [Serializable]
    public class DevicesDto
    {
        public Device[] devices;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var device in devices)
            {
                sb.AppendLine($"{device.name} : + {device.id}");
            }
            return sb.ToString();
        }
    }
}