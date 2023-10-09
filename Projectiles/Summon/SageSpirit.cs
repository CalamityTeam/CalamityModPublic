using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SageSpirit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        internal Player Owner => Main.player[Projectile.owner];
        internal ref float AttackTimer => ref Projectile.ai[0];
        internal ref float PlayerFlyTime => ref Projectile.ai[1];
        internal ref float SageSpiritIndex => ref Projectile.localAI[0];
        internal ref float GeneralTime => ref Projectile.localAI[1];
        internal bool Initialized = false;
        internal Vector2 PlayerFlyStart;
        internal Vector2 PlayerFlyOffsetAtEnds;
        internal Vector2 PlayerFlyDestination;

        internal const int ShootRate = 40;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 72;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Initialized);
            writer.Write(SageSpiritIndex);
            writer.Write(GeneralTime);
            writer.WritePackedVector2(PlayerFlyStart);
            writer.WritePackedVector2(PlayerFlyOffsetAtEnds);
            writer.WritePackedVector2(PlayerFlyDestination);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Initialized = reader.ReadBoolean();
            SageSpiritIndex = reader.ReadSingle();
            GeneralTime = reader.ReadSingle();
            PlayerFlyStart = reader.ReadPackedVector2();
            PlayerFlyOffsetAtEnds = reader.ReadPackedVector2();
            PlayerFlyDestination = reader.ReadPackedVector2();
        }

        public override void AI()
        {
            GeneralTime++;
            ProvidePlayerMinionBuffs();
            Initialize();
            DetermineFrames();
            NPC potentialTarget = Projectile.Center.MinionHoming(750f, Owner);
            if (potentialTarget is null)
                FlyNearOwner();
            else
            {
                ResetOwnerFlyValues();
                AttackTarget(potentialTarget);
            }
        }

        internal void ProvidePlayerMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<SageSpiritBuff>(), 3600);

            // Verify player/minion state integrity. The minion cannot stay alive if the
            // owner is dead or if the caller of the AI is invalid.
            if (Projectile.type != ModContent.ProjectileType<SageSpirit>())
                return;

            if (Owner.dead)
                Owner.Calamity().sageSpirit = false;
            if (Owner.Calamity().sageSpirit)
                Projectile.timeLeft = 2;
        }

        internal void Initialize()
        {
            if (Initialized)
                return;

            Initialized = true;
            PlayerFlyTime = 1f;
            PlayerFlyOffsetAtEnds = Vector2.UnitY;
            PlayerFlyStart = Projectile.Center;
            PlayerFlyDestination = Projectile.Center - Vector2.UnitY * 10f;
            Projectile.netUpdate = true;
        }

        internal void DetermineFrames()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        internal void ResetOwnerFlyValues()
        {
            PlayerFlyTime = 0f;
            PlayerFlyOffsetAtEnds = Vector2.Zero;
            PlayerFlyStart = PlayerFlyDestination = -Vector2.One;
            Projectile.netUpdate = true;
        }

        internal void FlyNearOwner()
        {
            Vector2 destination = Owner.Center + Vector2.UnitX * Owner.width * 1.6f * Owner.direction;

            int totalSageSpirits = Owner.ownedProjectileCounts[Projectile.type];
            destination += (SageSpiritIndex * MathHelper.TwoPi / totalSageSpirits).ToRotationVector2() * 70f;
            if (PlayerFlyTime >= 1f)
            {
                Projectile.Center = PlayerFlyDestination;
                ResetOwnerFlyValues();
                return;
            }

            float distanceFromDestination = Projectile.Distance(destination);
            if (float.IsNaN(distanceFromDestination))
                distanceFromDestination = 0f;

            // Determine owner values if they are still in the air.
            if (PlayerFlyTime <= 0f)
            {
                PlayerFlyStart = Projectile.Center;
                PlayerFlyOffsetAtEnds = Vector2.UnitY * distanceFromDestination;
                PlayerFlyDestination = destination;
                Projectile.netUpdate = true;
                PlayerFlyTime = 0.01f;
                return;
            }

            Projectile.position.Y += (float)Math.Cos(GeneralTime / 60f * MathHelper.TwoPi + Projectile.identity * 1.1f) * 0.5f;

            if (distanceFromDestination < 160f && PlayerFlyTime <= 0f)
                return;

            // A catmull-rom spline attempts to find a smooth arc of two points based on a 0-1 interpolant.
            // Its results are always within the bounds of the second and third term. They are intended to be the "middle" of the spline.
            // The other 2 points at the ends are intended to be used to determine how the resulting curve should arc.
            // In this context, since the offset at the start is down, it'll actually go up because it's starting at the bottom
            // and then going up, bending between the 2 middle points.
            Projectile.Center = Vector2.CatmullRom(
                PlayerFlyStart + PlayerFlyOffsetAtEnds,
                PlayerFlyStart,
                PlayerFlyDestination,
                PlayerFlyDestination + PlayerFlyOffsetAtEnds,
                PlayerFlyTime);

            PlayerFlyTime = MathHelper.Clamp(PlayerFlyTime + 0.03f, 0f, 1f);
        }

        internal void AttackTarget(NPC target)
        {
            AttackTimer++;

            int totalSageSpirits = Owner.ownedProjectileCounts[Projectile.type];
            Vector2 destinationOffsetFactor = Vector2.Max(target.Size, new Vector2(160f)) * new Vector2(0.3f, 0.2f);
            Vector2 destination = target.Center + (AttackTimer / 12f).ToRotationVector2() * destinationOffsetFactor;
            destination += (SageSpiritIndex * MathHelper.TwoPi / totalSageSpirits).ToRotationVector2() * 130f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.1f);

            // Frequently release a bunch of spikes.
            if (Projectile.owner == Main.myPlayer && AttackTimer % ShootRate == ShootRate - 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spikeVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(-0.43f, 0.43f, i / 3f)) * 6f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, spikeVelocity, ModContent.ProjectileType<SageNeedle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        // The spirit itself should not do direct damage.
        public override bool? CanDamage() => false;
    }
}
