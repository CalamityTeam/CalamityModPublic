using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class FleshBallMinion : BaseMinionProjectile
    {
        public override int AssociatedProjectileTypeID => ModContent.ProjectileType<FleshBallMinion>();
        public override int AssociatedBuffTypeID => ModContent.BuffType<FleshBallBuff>();
        public override ref bool AssociatedMinionBool => ref ModdedOwner.fleshBall;
        public override bool PreHardmodeMinionTileVision => true;

        public ref float HopTimer => ref Projectile.ai[0];
        public ref float HopAmount => ref Projectile.ai[1];
        public bool SittingOnGround => Math.Abs(Projectile.velocity.X) < 1.55f && Projectile.velocity.Y == 0f;
        public const float Gravity = 0.25f;
        public const float MaxFallSpeed = 12f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 38;
            Projectile.extraUpdates = 1;
        }

        public override void MinionAI()
        {
            GenerateVisuals();

            HopTimer++;
            SufferFromSeparationAnxiety();
            if (Target is null)
                GoNearOwner();
            else
                AttackTarget(Target);
            EnforceGravity();
        }

        #region AI Methods

        internal void EnforceGravity()
        {
            if (Projectile.velocity.Y < MaxFallSpeed)
                Projectile.velocity.Y += Gravity;
        }

        internal void GenerateVisuals()
        {
            // All code within this method is visual. There is no need to waste resources executing it on the server.
            if (Main.dedServ)
                return;

            Projectile.rotation += Projectile.velocity.X * 0.05f;

            Vector2 shootOffsetDirection = -Vector2.UnitY.RotatedBy(Projectile.rotation + Projectile.direction * 0.2f);
            Dust blood = Dust.NewDustDirect(Projectile.Center + shootOffsetDirection * 10f - new Vector2(4f), 0, 0, DustID.Blood, newColor: Color.Transparent);
            blood.velocity = shootOffsetDirection.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 4f);
            blood.noGravity = true;
        }

        internal void SufferFromSeparationAnxiety()
        {
            float teleportPromptDistance = Collision.CanHitLine(Projectile.Center, 1, 1, Owner.Center, 1, 1) ? 1900f : 805f;
            if (Main.myPlayer == Projectile.owner && !Projectile.WithinRange(Owner.Center, teleportPromptDistance))
            {
                Projectile.Center = Owner.Center;
                Projectile.netUpdate = true;
            }
        }

        internal void GoNearOwner()
        {
            Projectile.tileCollide = true;

            bool closeToPlayer = Projectile.WithinRange(Owner.Center, 150f);
            if (!closeToPlayer && SittingOnGround && HopTimer % 30f == 29f)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Owner.Center) * 9f + new Vector2(Math.Sign(Projectile.velocity.X) * 2f, -9f);

                // Don't collide with tiles for 1 frame, to prevent slopes from being a nuisance.
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
        }

        internal void AttackTarget(NPC target)
        {
            Projectile.tileCollide = true;

            if (SittingOnGround && HopTimer % 20f == 19f)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 6f + new Vector2(Math.Sign(Projectile.velocity.X) * 2f, -7f);
                HopAmount++;

                // Release a bunch of blood.
                if (Main.myPlayer == Projectile.owner && HopAmount % 3f == 2f)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 shootVelocity = -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(6f, 11f);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, shootVelocity, ModContent.ProjectileType<FleshBlood>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (Main.projectile.IndexInRange(p))
                            Main.projectile[p].originalDamage = Projectile.originalDamage;
                    }
                }

                // Don't collide with tiles for 1 frame, to prevent slopes from being a nuisance.
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
        }

        #endregion

        public override void OnSpawn(IEntitySource source) => IFrames = 30;

        // Prevent immediate death on tile collision, and slow down on tile collision.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X *= 0.9f;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Projectile.Bottom.Y < Owner.Top.Y;
            return true;
        }
    }
}
