using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 150;
            Projectile.extraUpdates = 1;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 2; i++)
            {
                int dustInt = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 138, 0f, 0f, 100, default, 1.2f);
                Main.dust[dustInt].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[dustInt].scale = 0.5f;
                    Main.dust[dustInt].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 3; j++)
            {
                int moreDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 138, 0f, 0f, 100, default, 1.7f);
                Main.dust[moreDust].noGravity = true;
                Main.dust[moreDust].velocity *= 5f;
                moreDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 138, 0f, 0f, 100, default, 1f);
                Main.dust[moreDust].velocity *= 2f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
