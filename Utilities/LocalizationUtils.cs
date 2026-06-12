using CalamityMod.Enums;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod
{
    public partial class CalamityUtils
    {
        /// <param name="key">The language key. This will have "Mods.CalamityMod." appended behind it.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance found using the provided key with "Mods.CalamityMod." appended behind it. 
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetText(string key)
        {
            return Language.GetOrRegister("Mods.CalamityMod." + key);
        }

        /// <param name="key">The language key. This will have "Mods.CalamityMod." appended behind it.</param>
        /// <returns>
        /// A <see cref="string"/> instance found using the provided key with "Mods.CalamityMod." appended behind it.
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static string GetTextValue(string key)
        {
            return Language.GetTextValue("Mods.CalamityMod." + key);
        }

        /// <param name="itemID">The item's ID.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance for an item's name. 
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetItemName(int itemID)
        {
            if (itemID < ItemID.Count)
            {
                return Language.GetText("ItemName." + ItemID.Search.GetName(itemID));
            }
            return GetTextFromModItem(itemID, "DisplayName");
        }

        /// <returns>
        /// A <see cref="LocalizedText"/> instance which will have the item's translated name.
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetItemName<T>() where T : ModItem => GetTextFromModItem(ModContent.ItemType<T>(), "DisplayName");

        /// <param name="itemID">The item's ID.</param>
        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetTextFromModItem(int itemID, string suffix)
        {
            var modItem = ItemLoader.GetItem(itemID);
            return modItem.GetLocalization(suffix);
        }

        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetTextFromModItem<T>(string suffix) where T : ModItem => GetTextFromModItem(ModContent.ItemType<T>(), suffix);

        /// <param name="itemID">The item's ID.</param>
        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="string"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static string GetTextValueFromModItem(int itemID, string suffix) => GetTextFromModItem(itemID, suffix).ToString();

        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="string"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static string GetTextValueFromModItem<T>(string suffix) where T : ModItem => GetTextFromModItem(ModContent.ItemType<T>(), suffix).ToString();

        #region Tooltip Format Helper
        public static string EmbedItemIcon(this int itemID) => $"[i:{itemID}] " + GetItemName(itemID);

        public static string FramesToSeconds(this int frame) => Round(frame / 60f, "N2");

        public static string ToMph(this float velocity) => Round(velocity * 216000f / 42240f, "N0");
        public static string ToMphps(this float velocity) => Round(velocity * 60f * 216000f / 42240f, "N2");
        public static string ToTiles(this float pixel) => Round(pixel / 16f);

        public static string ToRegenPerSecond(this float partialRegen) => Round(partialRegen * 0.5f, "N2");
        public static string ToRegenPerSecond(this int regen, float partialRegen = 0f) => Round((regen + partialRegen) * 0.5f, "N2");
        public static string ToJumpSpeedPercent(this float boost) => Round(boost * 20f, "N2");
        public static string ToStealth(this float stealth) => Round(stealth * 100f, "N0");

        public static string GetChanceFromDenominator(this int denominator) => ToPercent(1 / (float)denominator);

        public static string ToPercent(this int percent) => (percent * 100).ToString();
        public static string ToPercent(this float percent, string precision = "N1") => Round(percent * 100f, precision);
        public static string ToPercent(this double percent, string precision = "N1") => Round(percent * 100D, precision);
        // Double-rounded for proper digit cutoffs
        public static string Round(this float number, string precision = "N4") => float.Parse((number).ToString(precision)).ToString();
        public static string Round(this double number, string precision = "N4") => float.Parse((number).ToString(precision)).ToString();
        #endregion

        public static string GetArmorSetBonusKey()
        {
            ModKeybind setBonusKey = CalamityKeybinds.ArmorSetBonusHotKey;
            bool hasHotkey = setBonusKey.GetAssignedKeysOrEmpty().Count != 0;
            string directionKey = (Main.ReversedUpDownArmorSetBonuses ? Language.GetTextValue("Key.UP") : Language.GetTextValue("Key.DOWN"));

            // Allow both
            if (hasHotkey && CalamityClientConfig.Instance.SetBonusDoubleTap == SetBonusDoubleTapOptions.On)
                return GetText("Common.BothArmorSetBonusKeys").Format(setBonusKey.TooltipHotkeyString(), directionKey);
            // Literally bind nothing (mentions the key name to you)
            else if (!hasHotkey && CalamityClientConfig.Instance.SetBonusDoubleTap == SetBonusDoubleTapOptions.Off)
                return GetTextValue("Common.NoArmorSetBonusKey");
            // Hotkey only
            else if (hasHotkey)
                return GetText("Common.ArmorSetBonusKey").Format(setBonusKey.TooltipHotkeyString());
            // Double tap only
            else
                return GetText("Common.DoubleTapDown").Format(directionKey);
        }
    }
}
