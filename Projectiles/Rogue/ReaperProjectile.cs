using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ReaperProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheReaper";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Reaper");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = projectile.MaxUpdates * 90;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 32; // can't hit too fast, but can hit many many times
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.ai[1] += 1f;

                // If the Reaper lands a hit, switch to second behavior mode immediately.
                if (projectile.ai[1] >= 60f || projectile.numHits > 0)
                {
                    projectile.localAI[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }

                // Initial homing before landing a hit.
                else
                    CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 250f, 12f, 14f);
            }

            // Homing after landing a hit. This homing repeatedly turns on and off.
            else
            {
                float homingRange = 700f;
                bool noHomingThisFrame = false;
                if (projectile.ai[0] == 1f)
                {
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] > 40f)
                    {
                        projectile.ai[1] = 1f;
                        projectile.ai[0] = 0f;
                        projectile.netUpdate = true;
                    }
                    else
                        noHomingThisFrame = true;
                }

                if (noHomingThisFrame)
                    return;

                Vector2 homingTarget = projectile.Center;
                bool foundTarget = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float npcDist = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!foundTarget)
                        {
                            homingRange = npcDist;
                            homingTarget = nPC2.Center;
                            foundTarget = true;
                            break;
                        }
                    }
                }

                if (foundTarget && projectile.ai[0] == 0f)
                {
                    Vector2 delta = homingTarget - projectile.Center;
                    float distance = delta.Length();
                    delta /= distance;

                    if (distance > 200f)
                    {
                        float homingScalar = 11f;
                        delta *= homingScalar;
                        projectile.velocity = (projectile.velocity * 40f + delta) / 41f;
                    }
                    else
                    {
                        float homingScalar = 3.6f;
                        delta *= -homingScalar; // yes this is intentionally backwards
                        projectile.velocity = (projectile.velocity * 40f + delta) / 41f;
                    }
                }

                if (projectile.ai[1] > 0f)
                {
                    projectile.ai[1] += (float)Main.rand.Next(1, 4);
                }
                if (projectile.ai[1] > 40f)
                {
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
                if (projectile.ai[0] == 0f)
                {
                    if (projectile.ai[1] == 0f && foundTarget && homingRange < 500f)
                    {
                        projectile.ai[1] += 1f;
                        if (Main.myPlayer == projectile.owner)
                        {
                            projectile.ai[0] = 1f;
                            Vector2 value20 = homingTarget - projectile.Center;
                            value20.Normalize();
                            projectile.velocity = value20 * 8f;
                            projectile.netUpdate = true;
                        }
                    }
                }
            }
            projectile.rotation += 0.07f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item21, projectile.position);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 100;
            projectile.height = 100;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 8; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}
