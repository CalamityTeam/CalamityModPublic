using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class FuckYou : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60; // Lasts so long due to visuals.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 600; // Under absolutely no circumstances should this explosion hit more than once.
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.9f, 0.8f, 0.6f);
            Projectile.ai[1] += 0.01f;
            Projectile.scale = Projectile.ai[1] * 0.5f;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= (float)(3 * Main.projFrames[Projectile.type]))
            {
                Projectile.Kill();
                return;
            }
            int incrementer = Projectile.frameCounter + 1;
            Projectile.frameCounter = incrementer;
            if (incrementer >= 3)
            {
                Projectile.frameCounter = 0;
                incrementer = Projectile.frame + 1;
                Projectile.frame = incrementer;
                if (incrementer >= Main.projFrames[Projectile.type])
                {
                    Projectile.hide = true;
                }
            }
            Projectile.alpha -= 63;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.ai[0] == 1f)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (int)(52f * Projectile.scale);
                Projectile.Center = Projectile.position;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                for (int dustIndexA = 0; dustIndexA < 4; dustIndexA = incrementer + 1)
                {
                    int smoky = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1.5f);
                    Main.dust[smoky].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    incrementer = dustIndexA;
                }
                for (int dustIndexB = 0; dustIndexB < 10; dustIndexB = incrementer + 1)
                {
                    int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 200, default, 2.7f);
                    Dust dust = Main.dust[fireDust];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 2.5f;
                    incrementer = dustIndexB;
                }
                for (int dustIndexC = 0; dustIndexC < 5; dustIndexC = incrementer + 1)
                {
                    int fireDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 0, default, 2.7f);
                    Dust dust = Main.dust[fireDust2];
                    dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    incrementer = dustIndexC;
                }
                for (int dustIndexD = 0; dustIndexD < 10; dustIndexD = incrementer + 1)
                {
                    int smokier = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 0, default, 1.5f);
                    Dust dust = Main.dust[smokier];
                    dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    incrementer = dustIndexD;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.ai[0] > 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 127);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.direction = Main.player[Projectile.owner].direction;
    }
}
