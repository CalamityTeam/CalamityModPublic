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
            var modItem = ItemLoader.GetItem(itemID);
            return Language.GetOrRegister($"Mods.{modItem.Mod.Name}.{modItem.LocalizationCategory}.{modItem.Name}.DisplayName");
        }

        /// <returns>
        /// A <see cref="LocalizedText"/> instance which will have the item's translated name.
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetItemName<T>() where T : ModItem
        {
            return GetItemName(ModContent.ItemType<T>());
        }
    }
}
