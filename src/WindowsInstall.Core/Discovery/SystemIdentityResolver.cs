namespace ComputerExtra.WindowsInstall.Core.Discovery;

public static class SystemIdentityResolver
{
    public static SystemIdentity Resolve(WindowsComputerSystemFacts facts)
    {
        ArgumentNullException.ThrowIfNull(facts);

        return SystemIdentity.Create(
            facts.ComputerName,
            facts.Manufacturer,
            facts.Model,
            facts.SerialNumber,
            ResolveDeviceClass(
                facts.PcSystemType,
                facts.PcSystemTypeEx));
    }

    public static DeviceClass ResolveDeviceClass(
        ushort pcSystemType,
        ushort pcSystemTypeEx)
    {
        if (pcSystemTypeEx == 8)
        {
            return DeviceClass.Tablet;
        }

        return pcSystemType switch
        {
            1 => DeviceClass.Desktop,
            2 => DeviceClass.Mobile,
            3 => DeviceClass.Workstation,
            6 => DeviceClass.Appliance,
            _ => DeviceClass.Unknown
        };
    }
}
