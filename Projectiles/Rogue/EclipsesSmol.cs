using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
	public class EclipsesSmol : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Small");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 150;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            for (int i = 0; i < 2; i++)
            {
                int dustInt = Dust.NewDust(projectile.position, projectile.width, projectile.height, 138, 0f, 0f, 100, default, 1.2f);
                Main.dust[dustInt].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[dustInt].scale = 0.5f;
                    Main.dust[dustInt].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 3; j++)
            {
                int moreDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 138, 0f, 0f, 100, default, 1.7f);
                Main.dust[moreDust].noGravity = true;
                Main.dust[moreDust].velocity *= 5f;
                moreDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 138, 0f, 0f, 100, default, 1f);
                Main.dust[moreDust].velocity *= 2f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
