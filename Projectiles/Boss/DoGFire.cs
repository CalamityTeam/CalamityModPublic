using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Boss
{
    public class DoGFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f && Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
                Projectile.frame = 0;

            int dust = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, 173, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 0.8f);
            Main.dust[dust].noGravity = true;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            if (Projectile.ai[1] > 0f)
            {
                if (Projectile.velocity.Length() < Projectile.ai[1])
                    Projectile.velocity *= Projectile.ai[0];
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 160);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
            for (int i = 0; i < 5; i++)
            {
                int godSlay = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[godSlay].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[godSlay].scale = 0.5f;
                    Main.dust[godSlay].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                int godSlay2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 3f);
                Main.dust[godSlay2].noGravity = true;
                Main.dust[godSlay2].velocity *= 5f;
                godSlay2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[godSlay2].velocity *= 2f;
            }
            for (int k = 0; k < 200; k++)
            {
                float dustScale = 12f;
                if (k < 150)
                    dustScale = 9f;
                if (k < 100)
                    dustScale = 6f;
                if (k < 50)
                    dustScale = 3f;

                int scalingDust = Dust.NewDust(Projectile.Center, 6, 6, 173, 0f, 0f, 100, default, 1f);
                float scalingDustVelX = Main.dust[scalingDust].velocity.X;
                float scalingDustVelY = Main.dust[scalingDust].velocity.Y;

                if (scalingDustVelX == 0f && scalingDustVelY == 0f)
                    scalingDustVelX = 1f;

                float scalingDustVelocity = (float)Math.Sqrt(scalingDustVelX * scalingDustVelX + scalingDustVelY * scalingDustVelY);
                scalingDustVelocity = dustScale / scalingDustVelocity;
                scalingDustVelX *= scalingDustVelocity;
                scalingDustVelY *= scalingDustVelocity;

                float scale = 1f;
                switch ((int)dustScale)
                {
                    case 4:
                        scale = 1.2f;
                        break;
                    case 8:
                        scale = 1.1f;
                        break;
                    case 12:
                        scale = 1f;
                        break;
                    case 16:
                        scale = 0.9f;
                        break;
                    default:
                        break;
                }

                Dust dust = Main.dust[scalingDust];
                dust.velocity *= 0.5f;
                dust.velocity.X += scalingDustVelX;
                dust.velocity.Y += scalingDustVelY;
                dust.scale = scale;
                dust.noGravity = true;
            }
            Projectile.Damage();
        }
    }
}
