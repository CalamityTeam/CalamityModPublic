using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
    public class EarlyBloomBobber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Early Bloom Bobber");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 61;
            projectile.bobber = true;
            projectile.penetrate = -1;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            Lighting.AddLight(projectile.Center, 0.1f, 0.5f, 0.15f);
            return projectile.DrawFishingLine(ModContent.ItemType<EarlyBloomRod>(), new Color(190, 140, 69, 100));
        }
    }
}
