using System;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace CalamityMod.Projectiles.Ranged
{
    public class StarmageddonBinaryStarCenter : ModProjectile, ILocalizedModType
    {
        private const float TimeBeforeHoming = 30f;
        private const float FlamethrowerSoundFrequency = 30f;
        public const int StarDistanceFromCenter = 32;
        public const float SuckedProjectileDistanceFromStars = 600f;
        public const float SuckedProjectileSpawnRate = 15f;
        public const float StarRotationRate = 2f;
        public const float DustCloudSpawnRate = 16f;
        public const float DustCloudVelocityMax = 8f;
        public const float DustCloudSpreadMax = 2f;
        public const int ParticleStreamsPerStar = 2;
        public const float ParticleSpawnRate = 4f;
        public const float ParticleSpawnOffset = 19f;
        public const float ParticleVelocityMax = DustCloudVelocityMax * 4f;
        public const float ParticleSpreadMax = DustCloudSpreadMax * 4f;

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile hostProjectile = Main.projectile[(int)Projectile.ai[0]];
            if (Projectile.type != ModContent.ProjectileType<StarmageddonBinaryStarCenter>() || !hostProjectile.active || hostProjectile.type != ModContent.ProjectileType<StarmageddonHeld>())
            {
                Projectile.Kill();
                return;
            }

            Player player = Main.player[Projectile.owner];

            // Spawn the binary star system.
            if (Projectile.localAI[0] == 0f && Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[0] = 1f;
                int starAmount = 2;
                int starSpread = 360 / starAmount;
                int starDistance = StarDistanceFromCenter;
                for (int i = 0; i < starAmount; i++)
                {
                    Vector2 starSpawnPosition = new Vector2(Projectile.Center.X + (float)(Math.Sin(i * starSpread) * starDistance), Projectile.Center.Y + (float)(Math.Cos(i * starSpread) * starDistance));
                    int projectileType = i == 0 ? ModContent.ProjectileType<StarmageddonStar>() : ModContent.ProjectileType<StarmageddonStar2>();
                    int star = Projectile.NewProjectile(Projectile.GetSource_FromAI(), starSpawnPosition, Vector2.Zero, projectileType, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));
                    Main.projectile[star].ai[1] = i * starSpread;
                }
            }

            if (Projectile.ai[1] == 1f)
            {
                Projectile.localAI[1] += 1f;
                if (Projectile.localAI[1] % FlamethrowerSoundFrequency == 0f)
                    SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);

                int npcIndex = (int)Projectile.ai[2];
                NPC npc = Main.npc[npcIndex];
                bool findNewTarget = false;

                if (!npcIndex.WithinBounds(Main.maxNPCs))
                {
                    findNewTarget = true;
                }
                else if (npc.active && !npc.dontTakeDamage)
                {
                    Projectile.Center = npc.Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = npc.gfxOffY;
                }
                else
                    findNewTarget = true;

                if (findNewTarget)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.ai[2] = 0f;
                }
            }
            else
            {
                if (Projectile.localAI[1] < TimeBeforeHoming)
                {
                    Projectile.localAI[1] += 1f;
                }
                else
                {
                    NPC target = Projectile.FindTargetWithinRange(1600f);
                    if (target != null)
                        Projectile.velocity = Projectile.SuperhomeTowardsTarget(target, 24f, 12f);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            Rectangle myRect = Projectile.Hitbox;

            if (Projectile.owner == Main.myPlayer)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.active && !npc.dontTakeDamage &&
                    ((Projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && Projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && Projectile.owner < Main.maxPlayers && player.killClothier))) ||
                        (Projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (Projectile.owner < 0 || npc.immune[Projectile.owner] == 0 || Projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !Projectile.ownerHitCheck)
                        {
                            bool stickingToNPC;
                            if (npc.type == NPCID.SolarCrawltipedeTail)
                            {
                                Rectangle rect = npc.Hitbox;
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                stickingToNPC = Projectile.Colliding(myRect, rect);
                            }
                            else
                                stickingToNPC = Projectile.Colliding(myRect, npc.Hitbox);

                            if (stickingToNPC)
                            {
                                if (npc.reflectsProjectiles && Projectile.CanBeReflected())
                                {
                                    npc.ReflectProjectile(Projectile);
                                    return;
                                }

                                Projectile.ai[1] = 1f;
                                Projectile.ai[2] = (float)npcIndex;

                                Projectile.velocity = (npc.Center - Projectile.Center) * 0.75f;

                                Projectile.netUpdate = true;

                                Point[] array2 = new Point[5];
                                int projCount = 0;
                                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                                {
                                    Projectile proj = Main.projectile[projIndex];
                                    if (projIndex != Projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == Projectile.type && proj.ai[0] == 1f && proj.ai[1] == (float)npcIndex)
                                    {
                                        array2[projCount++] = new Point(projIndex, proj.timeLeft);
                                        if (projCount >= array2.Length)
                                            break;
                                    }
                                }

                                if (projCount >= array2.Length)
                                {
                                    int num30 = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[num30].Y)
                                            num30 = m;
                                    }
                                    Main.projectile[array2[num30].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
