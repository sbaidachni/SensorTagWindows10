using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace SensorTagWindows.Code
{
    public class DeviceInfo
    {
        private GattDeviceService deviceService;

        public DeviceInfo(GattDeviceService service) { deviceService = service; }

        public async Task Initialize()
        {
            SystemId = await ReadSystemId();
        }

        public string SystemId { get; private set; }
        private async Task<string> ReadSystemId()
        {
            return await ReadSystemId(":");
        }

        private async Task<string> ReadSystemId(string separator)
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_SYSID);

            return sdata[0].ToString("X2") + separator + sdata[1].ToString("X2") + separator +
                sdata[2].ToString("X2") + separator + sdata[3].ToString("X2") + separator +
                sdata[4].ToString("X2") + separator + sdata[5].ToString("X2") + separator +
                sdata[6].ToString("X2") + separator + sdata[7].ToString("X2");
        }

        public string ModelNumber { get; private set; }

        private async Task<string> ReadModelNumber()
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_MODEL_NR);
            return ConvertToString(sdata);
        }

        public string SerialNumber { get; private set; }
        private async Task<string> ReadSerialNumber()
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_SERIAL_NR);
            return ConvertToString(sdata);
        }

        public string FirmwareRevision { get; private set; }
        private async Task<string> ReadFirmwareRevision()
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_FW_NR);
            return ConvertToString(sdata);
        }

        public string HardwareRevision { get; private set; }
        private async Task<string> ReadHardwareRevision()
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_HW_NR);
            return ConvertToString(sdata);
        }

        public string SoftwareRevision { get; private set; }

        private async Task<string> ReadSoftwareRevision()
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_SW_NR);
            return ConvertToString(sdata);
        }

        public string ManufacturerName { get; private set; }
        private async Task<string> ReadManufacturerName()
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_MANUF_NR);
            return ConvertToString(sdata);
        }

        public string Cert { get; private set; }
        private async Task<string> ReadCert()
        {
            return await ReadCert(" ");
        }

        private async Task<string> ReadCert(string separator)
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_CERT);

            return sdata[0].ToString("X2") + separator + sdata[1].ToString("X2") + separator +
                sdata[2].ToString("X2") + separator + sdata[3].ToString("X2") + separator +
                sdata[4].ToString("X2") + separator + sdata[5].ToString("X2") + separator +
                sdata[6].ToString("X2") + separator + sdata[7].ToString("X2") + separator +
                sdata[8].ToString("X2") + separator + sdata[9].ToString("X2") + separator +
                sdata[10].ToString("X2") + separator + sdata[11].ToString("X2") + separator +
                sdata[12].ToString("X2") + separator + sdata[13].ToString("X2");
        }

        public string PnPId { get; private set; }
        private async Task<string> ReadPnpId()
        {
            return await ReadPnpId(" ");
        }

        private async Task<string> ReadPnpId(string separator)
        {
            byte[] sdata = await ReadValue(SensorsUId.UUID_INF_PNP_ID);

            return sdata[0].ToString("X2") + separator + sdata[1].ToString("X2") + separator +
                sdata[2].ToString("X2") + separator + sdata[3].ToString("X2") + separator +
                sdata[4].ToString("X2") + separator + sdata[5].ToString("X2") + separator +
                sdata[6].ToString("X2");
        }

        private async Task<byte[]> ReadValue(string Uuid)
        {
            GattCharacteristic sidCharacteristic = deviceService.GetCharacteristics(new Guid(Uuid))[0];

            GattReadResult res = await sidCharacteristic.ReadValueAsync(Windows.Devices.Bluetooth.BluetoothCacheMode.Uncached);

             byte[] data = new byte[res.Value.Length];

            DataReader.FromBuffer(res.Value).ReadBytes(data);

            return data;
        }


        private string ConvertToString(byte[] dataBytes)
        {
            return Encoding.UTF8.GetString(dataBytes, 0, dataBytes.Length);
        }


    }
}
