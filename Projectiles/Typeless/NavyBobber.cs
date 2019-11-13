using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
    public class NavyBobber : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Navy Bobber");
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
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.25f / 255f);
            CalamityUtils.DrawFishingLine(projectile, ModContent.ItemType<NavyFishingRod>(), new Color(36, 61, 111, 100));
            return false;
		}
    }
}
