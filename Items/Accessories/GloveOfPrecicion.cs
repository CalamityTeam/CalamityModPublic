using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class GloveOfPrecicion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glove Of Precision");
            Tooltip.SetDefault("Decreases rogue speed by 20% but increases damage and crit by 12% and velocity by 25%");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 40;
            item.value = Item.buyPrice(0, 50, 0, 0);
            item.accessory = true;
            item.rare = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gloveOfPrecision = true;
        }
    }
}
