using MudBlazor;

namespace DgsTool.Themes;

public static class CorporateTheme
{
    public static readonly MudTheme Instance = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1B6B93",
            Secondary = "#F0955F",
            Tertiary = "#2E8B57",
            AppbarBackground = "#164A6E",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#14344B",
            DrawerText = "#D7E4EC",
            DrawerIcon = "#D7E4EC",
            Background = "#F7F9FC",
            Surface = "#FFFFFF",
            TableStriped = "#F7F9FC",
            Info = "#2F86B0",
            Success = "#2E8B57",
            Warning = "#D97706",
            Error = "#C0392B"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#4FA3D1",
            Secondary = "#F7A97A",
            AppbarBackground = "#14344B",
            DrawerBackground = "#0F2937",
            Background = "#0E1621",
            Surface = "#16212E"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "260px",
            AppbarHeight = "64px"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif" },
                FontSize = "0.875rem",
                LineHeight = "1.5"
            },
            H1 = new H1Typography { FontSize = "2.0rem", FontWeight = "600" },
            H2 = new H2Typography { FontSize = "1.6rem", FontWeight = "600" },
            H3 = new H3Typography { FontSize = "1.3rem", FontWeight = "600" },
            H4 = new H4Typography { FontSize = "1.1rem", FontWeight = "600" },
            H5 = new H5Typography { FontSize = "1.0rem", FontWeight = "600" },
            H6 = new H6Typography { FontSize = "0.95rem", FontWeight = "600" }
        }
    };
}
