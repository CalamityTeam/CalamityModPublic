using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class IceSentryFrostBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Magic/FrostBoltProjectile";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.SentryShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 480;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.01f / 255f, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.45f / 255f);
            for (int i = 0; i < 2; i++)
            {
                float slowVelX = Projectile.velocity.X / 3f * (float)i;
                float slowVelY = Projectile.velocity.Y / 3f * (float)i;
                int four = 4;
                int dusty = Dust.NewDust(new Vector2(Projectile.position.X + (float)four, Projectile.position.Y + (float)four), Projectile.width - four * 2, Projectile.height - four * 2, 92, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[dusty];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= slowVelX;
                dust.position.Y -= slowVelY;
            }
            if (Main.rand.NextBool(10))
            {
                int otherFour = 4;
                int dustier = Dust.NewDust(new Vector2(Projectile.position.X + (float)otherFour, Projectile.position.Y + (float)otherFour), Projectile.width - otherFour * 2, Projectile.height - otherFour * 2, 92, 0f, 0f, 100, default, 0.6f);
                Main.dust[dustier].velocity *= 0.25f;
                Main.dust[dustier].velocity += Projectile.velocity * 0.5f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 92, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn2, 60);
    }
}
