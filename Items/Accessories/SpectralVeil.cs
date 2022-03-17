using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class SpectralVeil : ModItem
    {
        public const float TeleportRange = 845f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Veil");
            Tooltip.SetDefault("The inside of the cloak is full of teeth...\n" +
                "TOOLTIP LINE HERE\n" +
                "If you dodge something while invulnerable, you instantly gain full stealth\n" +
				"Teleportation is disabled while Chaos State is active\n" +
                "Stealth generates 20% faster while moving");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 38;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.SpectralVeilHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = "Press " + hotkey + " to consume 25% of your maximum stealth to perform a mid-range teleport and render you momentarily invulnerable";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().spectralVeil = true;
            player.Calamity().stealthGenMoving += 0.2f;
        }
    }
}
