using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
    public class VerstaltiteBobber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Verstaltite Bobber");
        }

        public override void SetDefaults()
        {
            //projectile.CloneDefaults(360); //Wooden Bobber
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 61;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDrawExtras()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0f, 0.4f);
            CalamityUtils.DrawFishingLine(Projectile, ModContent.ItemType<VerstaltiteFishingRod>(), new Color(95, 158, 160, 100));
            return false;
        }
    }
}
