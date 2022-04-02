using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ShadowflameFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Fireball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.light = 0.8f;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 360;
            projectile.scale = 1.25f;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }

            // Main chunky dark purple dust at the front of the fireball
            for(int i = 0; i < 2; ++i)
            {
                int dustType = 27;
                float dustScale = Main.rand.NextFloat(1.4f, 2.4f);
                int dustID = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustType);
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].velocity = projectile.velocity;
                Main.dust[dustID].scale = dustScale;
            }

            // Trailing brighter purple fire trail dust
            {
                int dustType = 70;
                float velMult = Main.rand.NextFloat(0.05f, 0.6f);
                float dustScale = Main.rand.NextFloat(1.2f, 1.8f);
                int dustID = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].velocity *= 0.1f;
                Main.dust[dustID].velocity += projectile.velocity * velMult;
                Main.dust[dustID].scale = dustScale;
            }

            projectile.rotation += 0.3f * (float)projectile.direction;

            if (projectile.ai[1] == 1f)
            {
                int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
                Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                projectile.ai[0] += 1f;
                if (projectile.ai[0] >= 60f)
                {
                    if (projectile.ai[0] < 240f)
                    {
                        float scaleFactor2 = projectile.velocity.Length();
                        vector11.Normalize();
                        vector11 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;
                    }
                    else if (projectile.velocity.Length() < 18f)
                    {
                        projectile.tileCollide = true;
                        projectile.velocity *= 1.02f;
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 180, true);
            else if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 120, true);
            else if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 60, true);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            int killDust = 20;
            for (int i = 0; i < killDust; ++i)
            {
                int dustType = Main.rand.NextBool() ? 70 : 27;
                float dustScale = Main.rand.NextFloat(1f, 1.6f);
                int dustID = Dust.NewDust(projectile.Center, 1, 1, dustType);
                Main.dust[dustID].velocity *= 4f;
                Main.dust[dustID].scale = dustScale;
            }
        }
    }
}
