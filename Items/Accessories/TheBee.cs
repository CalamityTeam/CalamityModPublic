using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class TheBee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Bee");
            Tooltip.SetDefault("Causes stars to fall and releases bees when injured\n" +
                                "All projectiles gain bonus damage if you are at full HP\n" +
                                "The amount of bonus damage is based on your weapon's damage and fire rate\n" +
                                "Does not work with summons or sentries");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 4;
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.starCloak = true;
            player.bee = true;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.theBee = true;
        }
    }
}
