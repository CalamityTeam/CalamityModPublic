using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
    public class SlurperBobber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slurper Bobber");
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
            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0f);
            return projectile.DrawFishingLine(ModContent.ItemType<SlurperPole>(), new Color(227, 79, 79, 100));
        }
    }
}
