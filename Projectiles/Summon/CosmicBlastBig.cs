using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicBlastBig : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item9, (int)projectile.position.X, (int)projectile.position.Y);
                }
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            if (Main.rand.NextBool(8))
            {
                Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                Main.dust[num59].noGravity = true;
                Main.dust[num59].velocity = value3 * 0.66f;
                Main.dust[num59].position = projectile.Center + value3 * 12f;
            }
            if (Main.rand.NextBool(24))
            {
                int num60 = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[num60].velocity *= 0.66f;
                Main.gore[num60].velocity += projectile.velocity * 0.3f;
            }
            if (projectile.ai[1] == 1f)
            {
                if (Main.rand.NextBool(5))
                {
                    int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                    Main.dust[num59].noGravity = true;
                }
                if (Main.rand.NextBool(10))
                {
                    Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }
            }
            NPC potentialTarget = projectile.Center.MinionHoming(600f, Main.player[projectile.owner]);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 20f + projectile.SafeDirectionTo(potentialTarget.Center) * 45f) / 21f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 288;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Zombie, (int)projectile.Center.X, (int)projectile.Center.Y, 103);
            for (int num193 = 0; num193 < 3; num193++)
            {
                int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int num194 = 0; num194 < 30; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }
    }
}
