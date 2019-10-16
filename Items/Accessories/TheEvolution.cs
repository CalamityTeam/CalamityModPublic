using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class TheEvolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Evolution");
            Tooltip.SetDefault("You have a 50% chance to reflect projectiles when they hit you back at the enemy for 1000% their original damage\n" +
                                "If this effect triggers you get a health regeneration boost for a short time\n" +
                                "If the same enemy projectile type hits you again you will resist its damage by 15%");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.projRefRare = true;
        }
    }
}
