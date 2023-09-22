using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BelladonnaSpirit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Owner.Center.MinionHoming(BelladonnaSpiritStaff.EnemyDistanceDetection, Owner, CalamityPlayer.areThereAnyDamnBosses);
        public ref float PetalFireTimer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;

            Projectile.width = 28;
            Projectile.height = 48;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {   
            // Checks if the minion can still exist.
            CanMinionExist();

            // Does the animation of the minion.
            DoAnimation(Target);

            // Points in the right directions.
            PointInDirection(Target);

            // Vibes around the player.
            FollowPlayer();

            // The minions push eachother to not clump.
            Projectile.MinionAntiClump();

            // If there's a target, it'll shoot at it.
            if (Target is not null)
                TargetNPC();

            Projectile.netUpdate = true;
        }

        #region Methods

        public void CanMinionExist()
        {
            Owner.AddBuff(ModContent.BuffType<BelladonnaSpiritBuff>(), 1);
            if (Projectile.type == ModContent.ProjectileType<BelladonnaSpirit>())
            {
                if (Owner.dead)
                    ModdedOwner.belladonaSpirit = false;
                if (ModdedOwner.belladonaSpirit)
                    Projectile.timeLeft = 2;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            int dustAmount = 45;
            for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
            {
                float angle = MathHelper.TwoPi / 45f * dustIndex;
                Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(3f, 4.5f);
                Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 39, velocity);
                spawnDust.noGravity = true;
                spawnDust.scale = velocity.Length() * 0.1f;
                spawnDust.velocity *= 0.3f;
            }
        }

        public void DoAnimation(NPC target)
        {
            int speedModifier = (target is not null) ? 2 : 1;

            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / (Main.projFrames[Projectile.type] * speedModifier) % Main.projFrames[Projectile.type];
        }

        public void PointInDirection(NPC target)
        {
            // If there's a target look at the target.
            if (target is not null) 
                Projectile.spriteDirection = ((target.Center.X - Projectile.Center.X) < 0).ToDirectionInt();

            // If there's not a target, the minion just points where it's going.
            else
                if (Math.Abs(Projectile.velocity.X) > 0.01f)
                    Projectile.spriteDirection = -Math.Sign(Projectile.velocity.X);
        }

        public void FollowPlayer()
        {
            // If the minion starts to get far, force the minion to go to you.
            if (Projectile.WithinRange(Owner.Center, BelladonnaSpiritStaff.EnemyDistanceDetection) && !Projectile.WithinRange(Owner.Center, 300f)) 
                Projectile.velocity = (Owner.Center - Projectile.Center) / 30f;

            // The minion will change directions to you if it's going away from you, meaning it'll just hover around you.
            else if (!Projectile.WithinRange(Owner.Center, 160f)) 
                Projectile.velocity = (Projectile.velocity * 37f + Projectile.SafeDirectionTo(Owner.Center) * 17f) / 40f;

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, BelladonnaSpiritStaff.EnemyDistanceDetection))
            {
                Projectile.position = Owner.Center;
                Projectile.velocity *= 0.3f;
            }
        }

        public void TargetNPC()
        {
            // The minion will slowly go up until it throws the petal.
            // This essentially makes the minion stay above you and trigger the "Turn back to player", it'll do this continuously, giving the effect of bouncing.
            // Yes, it's very wacky. You can make it better if you wish.
            Projectile.velocity.Y -= MathHelper.Lerp(0, 0.005f, PetalFireTimer % BelladonnaSpiritStaff.FireRate);
            
            if (Main.myPlayer == Projectile.owner)
                FirePetals();

            if (PetalFireTimer < BelladonnaSpiritStaff.FireRate)
                PetalFireTimer++;
        }

        public void FirePetals()
        {
            if (PetalFireTimer == BelladonnaSpiritStaff.FireRate) // Every 75 frames, throws a petal.
            {
                // Throws the petal upwards with a random force and inherits the minion's speed.
                Vector2 petalShootVelocity = (-Vector2.UnitY * Main.rand.NextFloat(7f, 8.5f)) +
                    Vector2.UnitX * Projectile.velocity.X +
                    Vector2.UnitY * Projectile.velocity.Y * 0.35f;
                
                int petal = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    petalShootVelocity,
                    ModContent.ProjectileType<BelladonnaPetal>(), 
                    Projectile.damage, 
                    Projectile.knockBack, 
                    Projectile.owner);

                if (Main.projectile.IndexInRange(petal))
                {
                    Main.projectile[petal].rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                }

                // Resets the timer.
                PetalFireTimer = 0f;
            }
        }

        public override bool? CanDamage() => false;

        #endregion
    }
}
