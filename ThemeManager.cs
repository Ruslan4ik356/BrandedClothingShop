using System;
using System.Drawing;

namespace BrandedClothingShop
{
    public static class ThemeManager
    {
        public enum Theme { Light, Dark }
        public static Theme CurrentTheme { get; set; } = Theme.Light;

        public static Color GetBackgroundColor() => CurrentTheme == Theme.Light ? Color.White : Color.FromArgb(30, 30, 30);
        public static Color GetForegroundColor() => CurrentTheme == Theme.Light ? Color.Black : Color.White;
        public static Color GetPanelColor() => CurrentTheme == Theme.Light ? Color.FromArgb(250, 250, 250) : Color.FromArgb(45, 45, 45);
        public static Color GetTextColor() => CurrentTheme == Theme.Light ? Color.FromArgb(51, 51, 51) : Color.FromArgb(200, 200, 200);
        public static Color GetBorderColor() => CurrentTheme == Theme.Light ? Color.FromArgb(200, 200, 200) : Color.FromArgb(60, 60, 60);
    }
}
