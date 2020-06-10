using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ManaJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Jelly");
            Tooltip.SetDefault("+20 max mana\n" +
                "Standing still boosts mana regen");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = 1;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 20;
            if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
            {
                player.manaRegenBonus += 2;
            }
        }
    }
}
