using ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

namespace ComputerExtra.WindowsInstall.Core.Tests.Drivers.Wortmann;

[TestClass]
public sealed class WortmannDriverCatalogTests
{
    [TestMethod]
    public void SelectForWindows11_SelectsExpectedDriversFromRealWortmannFixture()
    {
        var packages = WortmannDriverCatalog.SelectForWindows11(
            CreateRealFixture());

        Assert.HasCount(10, packages);

        CollectionAssert.AreEquivalent(
            new[]
            {
                WortmannDriverCategory.Bluetooth,
                WortmannDriverCategory.Chipset,
                WortmannDriverCategory.DynamicTuning,
                WortmannDriverCategory.Network,
                WortmannDriverCategory.ManagementEngine,
                WortmannDriverCategory.Storage,
                WortmannDriverCategory.SystemIo,
                WortmannDriverCategory.Audio,
                WortmannDriverCategory.Wireless,
                WortmannDriverCategory.Graphics
            },
            packages.Select(package => package.Category).ToArray());

        Assert.AreEqual(
            "Intel_LAN_I226_Win11_2.1.3.15_A535C258-34B3-434E-B877-CEF249A6DB38.zip",
            packages.Single(
                package => package.Category == WortmannDriverCategory.Network)
                .Asset.FileName);

        Assert.AreEqual(
            "596.21-desktop-win10-win11-64bit-international-dch-whql_4C2B1994-E56B-49E2-9572-B1408A68D0DA.zip",
            packages.Single(
                package => package.Category == WortmannDriverCategory.Graphics)
                .Asset.FileName);
    }

    [TestMethod]
    public void SelectForWindows11_ExcludesNonDriverAssets()
    {
        var packages = WortmannDriverCatalog.SelectForWindows11(
            CreateRealFixture());

        Assert.IsFalse(
            packages.Any(
                package =>
                    package.Asset.FileName.Contains(
                        "ArmouryCrate",
                        StringComparison.OrdinalIgnoreCase)));

        Assert.IsFalse(
            packages.Any(
                package =>
                    package.Asset.FileName.Contains(
                        "Win11_25H2",
                        StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void SelectForWindows11_ReturnsEmptyForEmptyCatalog()
    {
        Assert.HasCount(
            0,
            WortmannDriverCatalog.SelectForWindows11([]));
    }

    private static IReadOnlyList<WortmannDownloadAsset> CreateRealFixture()
    {
        string[] urls =
        [
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-BT/Intel_Wireless_Bluetooth_23.10.0.2_C1225A16-55FD-4735-92E2-D2D9D903DECA.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-CHIP/Intel_Chipset_10.1.19600.8418_7FF7B0CA-4D6B-450C-8FBE-4C501A55ED65.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-DTT/Intel_DTT_9.0.11405.42569_2FE01414-02A9-477C-B5A0-58AF6C98D467.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-LAN/DRV_LAN_Intel_I225_I226_SZ-TSD_W10_64_V11443_20240924R_57C04F31-EE4F-4589-9F46-F9337A5C1529.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-LAN/Intel_LAN_I226_Win10_1.1.3.28_99C08FE5-85EC-4586-A315-C4B6B7311F11.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-LAN/Intel_LAN_I226_Win11_2.1.3.15_A535C258-34B3-434E-B877-CEF249A6DB38.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-MEI/Intel_MEI_2336.5.2.0_EA3974F9-75AD-4255-ACF6-D6DF45001712.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-RST/Intel_RST_19.5.7.1058.1_0F86FF56-61C9-4EA8-A025-91FF182A94D2.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-SIO/Intel_MEI_2336.5.2.0_16770A38-7BC8-4D2F-9769-CDF0BD483936.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-SOUND/Realtek_Audio_6.0.9418.1_DAC8430C-CF34-44F1-A25E-2BA7AFA69D32.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-WIFI/Intel_Wireless_WiFi_23.10.0.8_A915FF1D-D5B4-49D0-A080-B217A5DA32D5.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW/ArmouryCrateInstaller_60DC8592-6D5E-414A-BDB0-C028FF12BB47.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW/UM_ASUS_TUF_Gaming_Z790-PLUS_WiFi_2827EC21-4EF9-42ED-8FDF-F415B4538001.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_TERRA_ALLGEMEIN_QSG/2018-07_Benutzerhandbuch_3323BB18-B438-41E3-BF04-FB83AE2B6D6B.pdf",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_TERRA_ALLGEMEIN_QSG/Quickstart_Guide_PC_MUI_A44A0AF0-8938-4482-98B0-583FFFDC6EB7.PDF",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_TERRA_TERRA_XBOXGAMEPASS/Xbox_GamePass_Manual_DE_EN_DE78E1F9-2AFC-421D-BB43-F1D39A863F7B.pdf",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_VGA_NVIDIA_GEFORCE_RTX40/577.00-desktop-win10-win11-64bit-international-dch-whql_1C395DE3-D4C9-44E5-BDA6-030A292465DC.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/PC_VGA_NVIDIA_GEFORCE_RTX40/596.21-desktop-win10-win11-64bit-international-dch-whql_4C2B1994-E56B-49E2-9572-B1408A68D0DA.zip",
            "https://webftp.wortmann.de/dokumentenmanagement_wag/T_MS_REC_DVD_11_X64/Win11_25H2_V26031_DBAD3FCB-57B1-4784-BE66-EE833466B05E.zip"
        ];

        return urls
            .Select(url => new WortmannDownloadAsset(new Uri(url)))
            .ToArray();
    }
}
