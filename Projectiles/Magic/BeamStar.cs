using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BeamStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Time++;

            if (Time < 45f)
                Projectile.velocity = Projectile.velocity.RotatedBy((Projectile.identity % 4 - 2) / 2f * 0.016f) * 1.004f;
            else
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(800f, false);
                if (potentialTarget != null)
                {
                    float destinationDeltaAngle = MathHelper.WrapAngle(Projectile.AngleTo(potentialTarget.Center) - Projectile.velocity.ToRotation());

                    // If the angle is <0, meaning it's to the left, the sine will return a negative number,
                    // and a positive number if >0, because WrapAngle constricts to angle to a bound of
                    // -pi and pi, which are where a sine's wave restarts anew.
                    float angularHomeDirection = Math.Sign(Math.Sin(destinationDeltaAngle));
                    Projectile.velocity = Projectile.velocity.RotatedBy(angularHomeDirection * 0.03f);
                }
            }

            if (Main.rand.NextBool(4))
            {
                Dust stardust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 180, 0f, 0f, 100, default, 1f);
                stardust.position = Projectile.Center;
                stardust.scale += Main.rand.NextFloat(0.5f);
                stardust.noGravity = true;
                stardust.velocity.Y -= 2f;
            }
            if (Main.rand.NextBool(6))
            {
                Dust stardust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 176, 0f, 0f, 100, default, 1f);
                stardust.position = Projectile.Center;
                stardust.scale += Main.rand.NextFloat(0.3f, 1.1f);
                stardust.noGravity = true;
                stardust.velocity *= 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition;
            Texture2D starTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float scale = Projectile.scale * MathHelper.Lerp(0.9f, 0.6f, i / (float)Projectile.oldPos.Length) * 0.56f;
                drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(starTexture, drawPosition, null, Color.White, 0f, starTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
            }

            drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(starTexture, drawPosition, null, Color.White, 0f, starTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 176, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 180, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
