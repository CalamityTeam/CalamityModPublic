using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Typeless;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Tools
{
    public class BobbitHook : ModItem
    {
        public override void SetStaticDefaults()
        {
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
            item.CloneDefaults(ItemID.AmethystHook);
            item.shootSpeed = BobbitHead.LaunchSpeed; // How quickly the hook is shot.
            item.shoot = ProjectileType<BobbitHead>();
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = (CalamityRarity)13;
            item.width = 30;
            item.height = 32;
        }
    }
}
