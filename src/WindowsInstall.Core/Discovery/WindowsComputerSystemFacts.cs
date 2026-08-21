namespace ComputerExtra.WindowsInstall.Core.Discovery;

public sealed record WindowsComputerSystemFacts(
    string ComputerName,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    ushort PcSystemType,
    ushort PcSystemTypeEx);
