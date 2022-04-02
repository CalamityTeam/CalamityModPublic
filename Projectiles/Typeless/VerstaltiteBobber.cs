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
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 61;
            projectile.bobber = true;
            projectile.penetrate = -1;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            Lighting.AddLight(projectile.Center, 0.4f, 0f, 0.4f);
            CalamityUtils.DrawFishingLine(projectile, ModContent.ItemType<VerstaltiteFishingRod>(), new Color(95, 158, 160, 100));
            return false;
        }
    }
}
