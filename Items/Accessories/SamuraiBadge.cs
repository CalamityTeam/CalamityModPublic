using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class SamuraiBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Samurai Badge");
            Tooltip.SetDefault("20% increased melee damage and speed\n" +
                                "Reduces max life by 25%");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.value = Item.buyPrice(0, 21, 0, 0);
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.badgeOfBraveryRare = true;
        }
    }
}
