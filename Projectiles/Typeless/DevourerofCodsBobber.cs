using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;

namespace CalamityMod.Projectiles.Typeless
{
	public class DevourerofCodsBobber : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Devourer of Cods Bobber");
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

		//fuck glowmasks btw

        //i second this notion -Dominic
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Typeless/DevourerofCodsGlow"), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.25f);
            CalamityUtils.DrawFishingLine(projectile, ModContent.ItemType<TheDevourerofCods>(), projectile.Calamity().lineColor == 1 ? new Color(252, 109, 202, 100) : new Color(39, 151, 171, 100));
            return false;
		}
    }
}
