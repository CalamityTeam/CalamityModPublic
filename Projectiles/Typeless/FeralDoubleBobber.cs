using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
    public class FeralDoubleBobber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feral Bobber");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 61;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDrawExtras()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 0f);
            return Projectile.DrawFishingLine(ModContent.ItemType<FeralDoubleRod>(), new Color(220, 20, 60, 100));
        }
    }
}
