using CalamityMod.Items.Fishing.FishingRods;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class HeronBobber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heron Bobber");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = ProjAIStyleID.Bobber;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDrawExtras()
        {
            return Projectile.DrawFishingLine(ModContent.ItemType<HeronRod>(), new Color(101, 149, 154, 100));
        }
    }
}
