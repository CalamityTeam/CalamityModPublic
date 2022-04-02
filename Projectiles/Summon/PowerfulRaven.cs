using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PowerfulRaven : ModProjectile
    {
        public const float DistanceToCheck = 3200f;
        public const float TeleportDistance = 2700f;
        public const float SeparationAnxietyDistance = 2000f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Raven");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 24;
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
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.ai[0] = -1f;
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<PowerfulRaven>();
            player.AddBuff(ModContent.BuffType<CorvidHarbringerBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.powerfulRaven = false;
                }
                if (modPlayer.powerfulRaven)
                {
                    projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);

            if (potentialTarget != null)
            {
                projectile.ai[1] += 1f;
                if (projectile.Distance(potentialTarget.Center) > TeleportDistance)
                {
                    projectile.Center = potentialTarget.Center + Utils.NextVector2Unit(Main.rand) * potentialTarget.Size * 1.3f;
                    projectile.netUpdate = true;
                }
                else
                {
                    if (projectile.ai[1] % 45f == 28f || projectile.Distance(potentialTarget.Center) > 450f)
                    {
                        if (Main.rand.NextBool(6))
                        {
                            projectile.Center = potentialTarget.Center + Utils.NextVector2Unit(Main.rand) * potentialTarget.Size * 1.3f;
                            projectile.netUpdate = true;
                            for (int i = 0; i < 40; i++)
                            {
                                float angle = MathHelper.TwoPi / 40f * i;
                                float lerp = MathHelper.Lerp(0f, 1f, (float)Math.Sin(i / 8f * MathHelper.TwoPi) * 0.5f + 0.5f);
                                Dust dust = Dust.NewDustPerfect(projectile.position, DustID.Fire);
                                dust.velocity = Vector2.Lerp(Vector2.Zero, angle.ToRotationVector2() * 6f, lerp);
                                dust.noGravity = true;
                            }
                        }
                        projectile.velocity = (potentialTarget.Center - projectile.Center) / 50f;
                        if (projectile.velocity.Length() < 34f)
                        {
                            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitX) * 34f;
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            float angle = MathHelper.TwoPi / 20f * i;
                            Dust dust = Dust.NewDustPerfect(projectile.position + angle.ToRotationVector2().RotatedBy(projectile.rotation) * new Vector2(14f, 21f), DustID.Fire);
                            dust.velocity = angle.ToRotationVector2().RotatedBy(projectile.rotation) * 2f;
                            dust.noGravity = true;
                        }
                    }
                    if (projectile.ai[1] % 45f >= 28f)
                    {
                        projectile.frame = Main.projFrames[projectile.type] - 1;
                        Lighting.AddLight(projectile.Center, 1f, 1f, 1f);
                    }
                    else
                    {
                        projectile.velocity *= 0.95f;
                        projectile.rotation = projectile.rotation.AngleTowards(0f, 0.3f);
                        projectile.frameCounter++;
                        if (projectile.frameCounter > 6)
                        {
                            projectile.frame++;
                            projectile.frameCounter = 0;
                        }
                        if (projectile.frame >= Main.projFrames[projectile.type] - 1)
                        {
                            projectile.frame = 0;
                        }
                    }
                }
            }
            else
            {
                projectile.rotation = projectile.rotation.AngleTowards(0f, 0.2f);
                projectile.frameCounter++;
                if (projectile.frameCounter > 6)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= Main.projFrames[projectile.type] - 1)
                {
                    projectile.frame = 0;
                }
                if (projectile.Distance(player.Center) > SeparationAnxietyDistance)
                {
                    projectile.Center = player.Center;
                    projectile.velocity = Utils.NextVector2Unit(Main.rand) * 12f;
                    projectile.netUpdate = true;
                }
                else if (!projectile.WithinRange(player.Center, 90f))
                    projectile.velocity = (projectile.velocity * 19f + projectile.SafeDirectionTo(player.Center) * 12f) / 20f;
            }
            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
        }
    }
}
