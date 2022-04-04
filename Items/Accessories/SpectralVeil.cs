using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

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
            Item.width = 30;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.SpectralVeilHotKey.GetAssignedKeys().Aggregate((x, y) => x + ", " + y);
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "Tooltip1")
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
