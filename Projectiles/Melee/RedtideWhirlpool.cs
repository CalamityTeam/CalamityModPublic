using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class RedtideWhirlpool : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];

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
                Vector2 dustPos = Projectile.Center + angle.ToRotationVector2() * 46f;
                Dust dust = Dust.NewDustPerfect(dustPos, 176, (angle - MathHelper.PiOver2 * Math.Sign(Projectile.velocity.X)).ToRotationVector2() * 8f + Projectile.velocity, Scale: Main.rand.NextFloat(1.6f, 3f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            SpriteEffects flip = Math.Sign(Projectile.velocity.X) < 0? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * 0.3f, Projectile.rotation * 1.2f, texture.Size() / 2f, 1.5f, flip, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, texture.Size() / 2f, 2f, flip, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);

            int dustCount = 36;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 offset = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                offset = offset.RotatedBy(((i - (dustCount / 2 - 1)) * MathHelper.TwoPi / (float)dustCount), default) + Projectile.Center;
                Vector2 dustDirection = offset - Projectile.Center;
                Dust dust = Dust.NewDustPerfect(offset + dustDirection, 172, Vector2.Zero, 100, default, 1.4f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.velocity = dustDirection;
            }
        }
    }
}
