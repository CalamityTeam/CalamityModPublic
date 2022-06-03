using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace CalamityMod.Projectiles.Melee
{
    public class RedtideWhirlpool : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whirlpool");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            Projectile.rotation += MathHelper.PiOver4 / 2.4f * Math.Sign(Projectile.velocity.X);

            int dustCount = Main.rand.Next(4);
            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < dustCount; i++)
            {
                float angle = i / (float)dustCount * MathHelper.TwoPi + offset;
                Vector2 dustPos = Projectile.Center + angle.ToRotationVector2() * 40f;
                Dust dust = Dust.NewDustPerfect(dustPos, 176, (angle - MathHelper.PiOver2 * Math.Sign(Projectile.velocity.X)).ToRotationVector2() * 5f + Projectile.velocity, Scale: Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            SpriteEffects flip = Math.Sign(Projectile.velocity.X) < 0? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, texture.Size() / 2f, 2f, flip, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 172, vector7.X * 2f, vector7.Y * 2f, 100, default, 1.4f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}
