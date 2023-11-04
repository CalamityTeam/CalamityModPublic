using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BrittleStarMinion : BaseMinionProjectile
    {
        public override int AssociatedBuffTypeID => ModContent.BuffType<BrittleStar>();
        public override ref bool AssociatedMinionBool => ref ModdedOwner.brittleStar;
        public override int AssociatedProjectileTypeID => ModContent.ProjectileType<BrittleStarMinion>();
        public override bool PreHardmodeMinionTileVision => true;
        public int HitCounter = 0;

        public int BuffModeBuffer = 30;
        public ref bool MinionBuffMode => ref Main.player[Projectile.owner].Calamity().brittleStarBuffMode;
        public ref float AITimer => ref Projectile.ai[0];
        public ref float StarIndex => ref Projectile.ai[1];
        public float MoveWidth = 1.3f;
        public bool MoveSize = false;
        public int ReformingTimer = 25;
        public bool Reforming = false;
        public int Time = 0;
        public float ProjKnock;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 28;
            Projectile.localNPCHitCooldown = 15;
            base.SetDefaults();
        }
        public float StarPositionAngle // Calculates where each minion should be placed.
        {
            get
            {
                float starCount = Owner.ownedProjectileCounts[Type];
                if (starCount <= 1f)
                    starCount = 1f;

                return MathHelper.TwoPi * StarIndex / starCount + AITimer * 0.018f;
                // "MathHelper.TwoPi / ProbeIndex * probeCount"s the position itself.
                // "AITimer * [Modifier]"s how fast it spins.
            }
        }
        public override void MinionAI()
        {
            Time++;
            if (Time == 1)
                ProjKnock = Projectile.knockBack;
            Projectile.knockBack = ProjKnock * (MinionBuffMode ? 2.5f : 1f);
            if (ReformingTimer < 25)
            {
                Projectile.alpha = 255;
                Reforming = true;
                ReformingTimer++;
            }
            if (ReformingTimer == 24) // Dust effect and positioning after reforming
            {
                Reforming = false;
                Projectile.Center = Owner.Center + Main.rand.NextVector2Circular(160, 160);
                float numberOfDusts = 10f;
                float rotFactor = 360f / numberOfDusts;
                SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.5f, Pitch = 0.1f }, Projectile.Center);
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(1.5f, 4f), 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f));
                    Vector2 velOffset = new Vector2(Main.rand.NextFloat(1.5f, 4f), 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f));
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(3) ? 216 : 207, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = Main.rand.NextFloat(1.3f, 1.9f);
                }
            }
            if (ReformingTimer == 25)
            {
                Projectile.alpha = 0;
                Reforming = false;
            }

            if (MoveWidth <= 0.7f) // Used for the rotation and movement of stars while circling player
            {
                MoveSize = true;
            }
            if (MoveWidth >= 1.3f)
            {
                MoveSize = false;
            }
            MoveWidth += (MoveSize ? Main.rand.NextFloat(0.02f, 0.04f) : Main.rand.NextFloat(-0.02f, -0.04f));

            // Right click, only runs on the first minion spawned so that it functions propperly
            if (Projectile.ai[2] == 0 && Owner.Calamity().mouseRight && Owner.HeldItem.type == ModContent.ItemType<BrittleStarStaff>())
            {
                if (BuffModeBuffer > 0)
                {
                    BuffModeBuffer--;
                    Projectile.netUpdate = true;
                }
                if (BuffModeBuffer == 0)
                {
                    MinionBuffMode = !MinionBuffMode;
                    BuffModeBuffer = 30;
                }
            }
            else if (BuffModeBuffer < 30)
                BuffModeBuffer = 30;

            if (MinionBuffMode) // Minion when circling
            {
                Projectile.localNPCHitCooldown = 25;
                Reforming = false;
                Projectile.alpha = 0;
                HitCounter = 0;
                Projectile.velocity = Vector2.Zero;
                Vector2 idleDestination = Owner.Center + StarPositionAngle.ToRotationVector2() * (75f * MoveWidth);
                Projectile.Center = Vector2.Lerp(Projectile.Center, idleDestination, 0.15f);
                AITimer++;

                Owner.statDefense += 3;
                Projectile.rotation += MoveWidth * 0.2f;
            }
            if (!MinionBuffMode && !Reforming) // Minion when ramming
            {
                Projectile.localNPCHitCooldown = 15;
                Projectile.MinionAntiClump();

                if (Target is not null)
                {
                    Projectile.rotation += Projectile.velocity.X * 0.06f; // Spins faster the faster it moves in the X-axis.
                    Vector2 dashDirection = Projectile.SafeDirectionTo(Target.Center) * 30;
                    if (!Projectile.WithinRange(Target.Center, 160f))
                    {
                        float inertia = 7f;
                        Projectile.velocity = (Projectile.velocity * inertia + dashDirection) / (inertia + 1f);
                    }
                    // But if there was the case where the minion was already inside the range,
                    // if the velocity's not around the dash speed, make it dash.
                    else if (Projectile.velocity.Length() < 25)
                    {
                        Projectile.velocity = dashDirection;
                    }
                    if (Projectile.WithinRange(Target.Center, 160f))
                    {
                        SparkParticle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 2, -Projectile.velocity * 0.05f, false, 2, 1.2f, Color.White * 0.2f);
                        GeneralParticleHandler.SpawnParticle(spark);
                        spark.Scale -= 0.1f;
                    }
                }
                else // Idol state
                {
                    Projectile.rotation += Projectile.velocity.X * 0.06f; // Spins faster the faster it moves in the X-axis.

                    if (!Projectile.WithinRange(Owner.Center, 42f))
                    {
                        Projectile.velocity = (Projectile.velocity + Projectile.SafeDirectionTo(Owner.Center)) * 0.95f;
                    }

                    // The minion will teleport on the owner if they get far enough.
                    if (!Projectile.WithinRange(Owner.Center, 1200f))
                    {
                        Projectile.Center = Owner.Center;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!MinionBuffMode) // "Break" the minion after every 3 hits if in charging
            {
                HitCounter++;
                if (HitCounter >= 3)
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity, Main.rand.NextBool(3) ? 216 : 207, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.05f, 0.4f), 0, default, Main.rand.NextFloat(1.3f, 1.8f));
                        dust.noGravity = true;
                    }
                    SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt with { Pitch = 0.5f }, Projectile.Center);
                    Projectile.velocity = Vector2.Zero;
                    HitCounter = 0;
                    ReformingTimer = 0;
                }
            }
            else
            {
                float numberOfDusts = 5f;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(0.5f, 2.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                    Vector2 velOffset = new Vector2(Main.rand.NextFloat(0.5f, 2.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(3) ? 216 : 207, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = Main.rand.NextFloat(1.3f, 1.8f);
                }
                HitCounter = 0;
            }
        }
        public override bool? CanDamage() => Reforming ? false : null;
    }
}
