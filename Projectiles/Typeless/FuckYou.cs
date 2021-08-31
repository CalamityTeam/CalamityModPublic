using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class FuckYou : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = false;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.9f, 0.8f, 0.6f);
            projectile.ai[1] += 0.01f;
            projectile.scale = projectile.ai[1] * 0.5f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= (float)(3 * Main.projFrames[projectile.type]))
            {
                projectile.Kill();
                return;
            }
            int incrementer = projectile.frameCounter + 1;
            projectile.frameCounter = incrementer;
            if (incrementer >= 3)
            {
                projectile.frameCounter = 0;
                incrementer = projectile.frame + 1;
                projectile.frame = incrementer;
                if (incrementer >= Main.projFrames[projectile.type])
                {
                    projectile.hide = true;
                }
            }
            projectile.alpha -= 63;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.position = projectile.Center;
                projectile.width = projectile.height = (int)(52f * projectile.scale);
                projectile.Center = projectile.position;
                Main.PlaySound(SoundID.Item14, projectile.position);
                for (int dustIndexA = 0; dustIndexA < 4; dustIndexA = incrementer + 1)
                {
                    int num992 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num992].position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    incrementer = dustIndexA;
                }
                for (int dustIndexB = 0; dustIndexB < 10; dustIndexB = incrementer + 1)
                {
                    int fireDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 200, default, 2.7f);
                    Dust dust = Main.dust[fireDust];
                    dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    fireDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                    dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 2.5f;
                    incrementer = dustIndexB;
                }
                for (int dustIndexC = 0; dustIndexC < 5; dustIndexC = incrementer + 1)
                {
                    int fireDust2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 0, default, 2.7f);
                    Dust dust = Main.dust[fireDust2];
                    dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    incrementer = dustIndexC;
                }
                for (int dustIndexD = 0; dustIndexD < 10; dustIndexD = incrementer + 1)
                {
                    int num998 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 0, default, 1.5f);
                    Dust dust = Main.dust[num998];
                    dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    incrementer = dustIndexD;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return projectile.ai[0] > 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 127);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            projectile.direction = Main.player[projectile.owner].direction;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            projectile.direction = Main.player[projectile.owner].direction;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * 0.3);
        }
    }
}
