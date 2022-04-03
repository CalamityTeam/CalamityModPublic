using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TerraSpear : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 1f / 255f, 0f);
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;
            if (Projectile.localAI[1] == 0f)
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 15;
                if (Projectile.alpha >= 125)
                {
                    Projectile.alpha = 130;
                    Projectile.localAI[1] = 1f;
                }
            }
            else if (Projectile.localAI[1] == 1f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 15;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.localAI[1] = 0f;
                }
            }
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 107, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            int num3;
            for (int num795 = 4; num795 < 31; num795 = num3 + 1)
            {
                float num796 = Projectile.oldVelocity.X * (30f / (float)num795);
                float num797 = Projectile.oldVelocity.Y * (30f / (float)num795);
                int num798 = Dust.NewDust(new Vector2(Projectile.oldPosition.X - num796, Projectile.oldPosition.Y - num797), 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                Main.dust[num798].noGravity = true;
                Dust dust = Main.dust[num798];
                dust.velocity *= 0.5f;
                num798 = Dust.NewDust(new Vector2(Projectile.oldPosition.X - num796, Projectile.oldPosition.Y - num797), 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                dust = Main.dust[num798];
                dust.velocity *= 0.05f;
                num3 = num795;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Projectile.timeLeft > 175)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5;
        }
    }
}
