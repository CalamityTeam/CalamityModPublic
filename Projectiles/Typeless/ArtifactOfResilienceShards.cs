using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Damageable;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    // TODO -- Make this one projectile with multiple frames instead of multiple projectiles with one frame.
    public class ArtifactOfResilienceShard1 : ModProjectile
    {
        public int Timer = 0;
        public Vector2 StartingPosition;
        public const int MaxTimeLeft = 360;
        public const int FallTime = 240;
        public const float OutwardMovementTime = 85f;
        public const float InwardReturnMovementTime = 100f;
        public const float MaxRadius = 240f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artifact Shard");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MaxTimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.frameCounter);
            writer.WriteVector2(StartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frameCounter = reader.ReadInt32();
            StartingPosition = reader.ReadVector2();
        }

        public static void ArtifactOfResilienceShardAI(Projectile projectile, ref Vector2 StartingPosition, ref int Timer)
        {
            // Spinning
            if (projectile.localAI[1] == 0f)
            {
                if (projectile.localAI[0] == 0f)
                {
                    StartingPosition = projectile.Center;
                    projectile.netUpdate = true;
                    projectile.localAI[0] = 1f;
                }
                Timer++;
                if (Timer < OutwardMovementTime)
                {
                    projectile.ai[1] = MathHelper.Lerp(0f, MaxRadius, Timer / OutwardMovementTime);
                }
                if (Timer > MaxTimeLeft - InwardReturnMovementTime)
                {
                    projectile.ai[1] = MathHelper.Lerp(0f, MaxRadius, (Timer - MaxTimeLeft) / -InwardReturnMovementTime);
                }
                projectile.ai[0] += MathHelper.Pi / (projectile.ai[1] / MathHelper.Pi + 1);
                projectile.Center = StartingPosition + projectile.ai[0].ToRotationVector2() * projectile.ai[1];
                projectile.rotation = projectile.ai[0];
                if (projectile.timeLeft == 1)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.ProfanedFire);
                        dust.velocity = Utils.NextVector2Circular(Main.rand, 36f, 36f);
                        dust.scale = Main.rand.NextFloat(1.9f, 2.45f);
                        dust.noGravity = true;
                    }
                    projectile.timeLeft = FallTime;
                    projectile.tileCollide = true;
                    projectile.velocity = Utils.NextVector2Circular(Main.rand, 18f, 9f);
                    projectile.localAI[1] = 1;
                }
                if (projectile.modProjectile is ArtifactOfResilienceShard1)
                {
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (Main.npc[i].active &&
                            Main.npc[i].Distance(StartingPosition) < projectile.ai[1] &&
                            Main.npc[i].damage > 0)
                        {
                            if (Main.npc[i].Calamity().relicOfResilienceCooldown <= 0)
                            {
                                Main.npc[i].Calamity().relicOfResilienceCooldown = 600;
                                Main.npc[i].Calamity().relicOfResilienceWeakness = 180;
                                Main.npc[i].AddBuff(ModContent.BuffType<ProfanedWeakness>(), Main.npc[i].Calamity().relicOfResilienceWeakness);
                            }
                        }
                    }
                }
            }
            // Falling
            else
            {
                if (projectile.frameCounter <= ArtifactOfResilienceBulwark.MaxReformations)
                {
                    // Reform
                    if (projectile.modProjectile is ArtifactOfResilienceShard1 && Main.myPlayer == projectile.owner)
                    {
                        Projectile reformedBulwark = Projectile.NewProjectileDirect(projectile.Center,
                                                                                    Vector2.Zero,
                                                                                    ModContent.ProjectileType<ArtifactOfResilienceBulwark>(),
                                                                                    projectile.damage,
                                                                                    projectile.knockBack,
                                                                                    projectile.owner);
                        reformedBulwark.frameCounter = projectile.frameCounter; // Incremented on death, not reformation.
                    }
                    projectile.Kill();
                    return;
                }
                if (projectile.velocity.Y < 12f)
                {
                    projectile.velocity.Y += 0.25f;
                }
                projectile.velocity.X *= 0.95f;
                projectile.rotation = projectile.velocity.ToRotation();
                if (projectile.timeLeft < 60f)
                {
                    projectile.alpha = 255 - (int)MathHelper.Lerp(0, 255, projectile.timeLeft / 60f);
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void AI() => ArtifactOfResilienceShardAI(Projectile, ref StartingPosition, ref Timer);
    }
    public class ArtifactOfResilienceShard2 : ModProjectile
    {
        public int Timer = 0;
        public Vector2 StartingPosition;
        public const float MaxRadius = 660f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artifact Shard");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = ArtifactOfResilienceShard1.MaxTimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.frameCounter);
            writer.WriteVector2(StartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frameCounter = reader.ReadInt32();
            StartingPosition = reader.ReadVector2();
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void AI() => ArtifactOfResilienceShard1.ArtifactOfResilienceShardAI(Projectile, ref StartingPosition, ref Timer);
    }
    public class ArtifactOfResilienceShard3 : ModProjectile
    {
        public int Timer = 0;
        public Vector2 StartingPosition;
        public const float MaxRadius = 660f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artifact Shard");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = ArtifactOfResilienceShard1.MaxTimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.frameCounter);
            writer.WriteVector2(StartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frameCounter = reader.ReadInt32();
            StartingPosition = reader.ReadVector2();
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void AI() => ArtifactOfResilienceShard1.ArtifactOfResilienceShardAI(Projectile, ref StartingPosition, ref Timer);
    }
    public class ArtifactOfResilienceShard4 : ModProjectile
    {
        public int Timer = 0;
        public Vector2 StartingPosition;
        public const float MaxRadius = 660f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artifact Shard");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = ArtifactOfResilienceShard1.MaxTimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.frameCounter);
            writer.WriteVector2(StartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frameCounter = reader.ReadInt32();
            StartingPosition = reader.ReadVector2();
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void AI() => ArtifactOfResilienceShard1.ArtifactOfResilienceShardAI(Projectile, ref StartingPosition, ref Timer);
    }
    public class ArtifactOfResilienceShard5 : ModProjectile
    {
        public int Timer = 0;
        public Vector2 StartingPosition;
        public const float MaxRadius = 660f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artifact Shard");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = ArtifactOfResilienceShard1.MaxTimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.frameCounter);
            writer.WriteVector2(StartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frameCounter = reader.ReadInt32();
            StartingPosition = reader.ReadVector2();
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void AI() => ArtifactOfResilienceShard1.ArtifactOfResilienceShardAI(Projectile, ref StartingPosition, ref Timer);
    }
    public class ArtifactOfResilienceShard6 : ModProjectile
    {
        public int Timer = 0;
        public Vector2 StartingPosition;
        public const float MaxRadius = 660f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artifact Shard");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = ArtifactOfResilienceShard1.MaxTimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.frameCounter);
            writer.WriteVector2(StartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frameCounter = reader.ReadInt32();
            StartingPosition = reader.ReadVector2();
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void AI() => ArtifactOfResilienceShard1.ArtifactOfResilienceShardAI(Projectile, ref StartingPosition, ref Timer);
    }
}
