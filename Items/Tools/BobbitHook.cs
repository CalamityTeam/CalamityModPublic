using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Tools
{
    public class BobbitHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Bobbit Hook");
            Tooltip.SetDefault($@"Retracts upon attaching to a tile with extreme speeds
Reach: {BobbitHead.GrappleRangInTiles}
Launch Velocity: {BobbitHead.LaunchSpeed}
Reelback Velocity: {BobbitHead.ReelbackSpeed}
Pull Velocity: {BobbitHead.PullSpeed}");
        }

        public override void SetDefaults()
        {
            // Instead of copying these values, we can clone and modify the ones we want to copy
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = BobbitHead.LaunchSpeed; // How quickly the hook is shot.
            Item.shoot = ProjectileType<BobbitHead>();
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.width = 30;
            Item.height = 32;
        }
    }
}
