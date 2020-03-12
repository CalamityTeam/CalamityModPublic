using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DazzlingStabber : ModProjectile
    {
        public const float DistanceToCheck = 1500f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dazzling Stabber");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 58;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 9;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3());
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<DazzlingStabber>();
            player.AddBuff(ModContent.BuffType<DazzlingStabberBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.providenceStabber = false;
                }
                if (modPlayer.providenceStabber)
                {
                    projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            if (potentialTarget != null)
            {
                projectile.ai[0] += 1f;
                // Alternate between normal charge and slower charge/knife summon
                if (projectile.ai[0] % 160f < 100f)
                {
                    if (projectile.ai[0] % 90f < 30f)
                    {
                        projectile.velocity *= 0.99f;
                        projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(potentialTarget.Center) + MathHelper.PiOver2, 0.15f);
                    }
                    else if (projectile.ai[0] % 90f == 30f)
                    {
                        projectile.velocity = projectile.DirectionTo(potentialTarget.Center) * 20f;
                    }

                    else if (projectile.ai[0] % 90f < 60f)
                    {
                        projectile.velocity *= 0.99f;
                        projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(potentialTarget.Center) + MathHelper.PiOver2, 0.15f);
                    }
                    else if (projectile.ai[0] % 90f == 60f)
                    {
                        projectile.velocity = projectile.DirectionTo(potentialTarget.Center) * 16f;
                        for (int i = 0; i < 3; i++)
                        {
                            float angle = MathHelper.Lerp(-0.3f, 0.3f, i / 3f);
                            Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(angle), ModContent.ProjectileType<DazzlingStabberKnife>(), (int)(projectile.damage * 0.4), 1f, projectile.owner);
                        }
                    }
                    else projectile.velocity *= 0.99f;
                }
                else
                {
                    float angleMax = MathHelper.ToRadians(23f);
                    if (projectile.ai[0] % 160f == 120f)
                    {
                        float angleStart = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < 30; i++)
                        {
                            float angle = MathHelper.TwoPi / 30f * i + angleStart;
                            Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2() * 10f, (int)CalamityDusts.ProfanedFire);
                            dust.velocity = angle.ToRotationVector2() * 5f * MathHelper.Lerp(1f, 0f, (i % 6f) / 6f);
                        }
                        Vector2 positionDelta = Utils.RandomVector2(Main.rand, -12f, 12f);

                        while (positionDelta == Vector2.Zero)
                            positionDelta = Utils.RandomVector2(Main.rand, -12f, 12f);

                        projectile.Center = potentialTarget.Center + positionDelta;
                        projectile.rotation = projectile.AngleTo(potentialTarget.Center) + MathHelper.PiOver2 + angleMax;
                        projectile.velocity = Vector2.Zero;
                        projectile.netUpdate = true;
                    }
                    if (projectile.ai[0] % 160f > 120f)
                    {
                        projectile.rotation -= angleMax * 2f / 40f;
                    }
                }
            }
            else
            {
                projectile.rotation = projectile.rotation.AngleTowards(projectile.ai[1], 0.05f);
                Vector2 destination = player.Center + new Vector2(0f, -120f).RotatedBy(projectile.ai[1]);
                projectile.velocity = (destination - projectile.Center) / 40f;
            }
            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
        }
    }
}
