using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
    public class ChaoticSpreadBobber : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chaotic Bobber");
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
            Lighting.AddLight(projectile.Center, 0.4f, 0f, 0f);
            CalamityUtils.DrawFishingLine(projectile, ModContent.ItemType<ChaoticSpreadRod>(), new Color(138, 50, 73, 100), 30f);
            return false;
		}
    }
}
