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
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 24;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[0] = -1f;
                Projectile.localAI[0] = 1f;
            }
            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<PowerfulRaven>();
            player.AddBuff(ModContent.BuffType<CorvidHarbringerBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.powerfulRaven = false;
                }
                if (modPlayer.powerfulRaven)
                {
                    Projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = Projectile.Center.MinionHoming(DistanceToCheck, player);

            if (potentialTarget != null)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.Distance(potentialTarget.Center) > TeleportDistance)
                {
                    Projectile.Center = potentialTarget.Center + Utils.NextVector2Unit(Main.rand) * potentialTarget.Size * 1.3f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    if (Projectile.ai[1] % 45f == 28f || Projectile.Distance(potentialTarget.Center) > 450f)
                    {
                        if (Main.rand.NextBool(6))
                        {
                            Projectile.Center = potentialTarget.Center + Utils.NextVector2Unit(Main.rand) * potentialTarget.Size * 1.3f;
                            Projectile.netUpdate = true;
                            for (int i = 0; i < 40; i++)
                            {
                                float angle = MathHelper.TwoPi / 40f * i;
                                float lerp = MathHelper.Lerp(0f, 1f, (float)Math.Sin(i / 8f * MathHelper.TwoPi) * 0.5f + 0.5f);
                                Dust dust = Dust.NewDustPerfect(Projectile.position, 6);
                                dust.velocity = Vector2.Lerp(Vector2.Zero, angle.ToRotationVector2() * 6f, lerp);
                                dust.noGravity = true;
                            }
                        }
                        Projectile.velocity = (potentialTarget.Center - Projectile.Center) / 50f;
                        if (Projectile.velocity.Length() < 34f)
                        {
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 34f;
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            float angle = MathHelper.TwoPi / 20f * i;
                            Dust dust = Dust.NewDustPerfect(Projectile.position + angle.ToRotationVector2().RotatedBy(Projectile.rotation) * new Vector2(14f, 21f), 6);
                            dust.velocity = angle.ToRotationVector2().RotatedBy(Projectile.rotation) * 2f;
                            dust.noGravity = true;
                        }
                    }
                    if (Projectile.ai[1] % 45f >= 28f)
                    {
                        Projectile.frame = Main.projFrames[Projectile.type] - 1;
                        Lighting.AddLight(Projectile.Center, 1f, 1f, 1f);
                    }
                    else
                    {
                        Projectile.velocity *= 0.95f;
                        Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.3f);
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 6)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame >= Main.projFrames[Projectile.type] - 1)
                        {
                            Projectile.frame = 0;
                        }
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.2f);
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 6)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= Main.projFrames[Projectile.type] - 1)
                {
                    Projectile.frame = 0;
                }
                if (Projectile.Distance(player.Center) > SeparationAnxietyDistance)
                {
                    Projectile.Center = player.Center;
                    Projectile.velocity = Utils.NextVector2Unit(Main.rand) * 12f;
                    Projectile.netUpdate = true;
                }
                else if (!Projectile.WithinRange(player.Center, 90f))
                    Projectile.velocity = (Projectile.velocity * 19f + Projectile.SafeDirectionTo(player.Center) * 12f) / 20f;
            }
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
        }
    }
}
