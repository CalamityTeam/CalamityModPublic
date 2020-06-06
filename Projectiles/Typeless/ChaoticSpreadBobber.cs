using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
			if (projectile.Calamity().lineColor == 1)
				Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);
			else
				Lighting.AddLight(projectile.Center, 0f, 0.45f, 0.46f);
            CalamityUtils.DrawFishingLine(projectile, ModContent.ItemType<ChaoticSpreadRod>(), projectile.Calamity().lineColor == 1 ? new Color(255, 165, 0, 100) : new Color(0, 206, 209, 100), 65, 30f);
            return false;
		}
    }
}
