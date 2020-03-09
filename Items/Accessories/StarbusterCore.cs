using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StarbusterCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starbuster Core");
            Tooltip.SetDefault("Summons release an astral explosion on enemy hits\n" +
                               "1 extra minion slot");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions++;
            player.Calamity().starbusterCore = true;
        }
    }
}
