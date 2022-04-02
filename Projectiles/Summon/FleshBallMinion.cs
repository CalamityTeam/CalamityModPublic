using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class FleshBallMinion : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public bool SittingOnGround => Math.Abs(projectile.velocity.X) < 1.55f && projectile.velocity.Y == 0f;
        public ref float HopTimer => ref projectile.ai[0];
        public ref float HopAmount => ref projectile.ai[1];
        public const float Gravity = 0.25f;
        public const float MaxFallSpeed = 12f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh Ball");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            ProvidePlayerMinionBuffs();
            DetermineFrames();
            GenerateVisuals();

            HopTimer++;
            SufferFromSeparationAnxiety();
            NPC potentialTarget = projectile.Center.MinionHoming(950f, Owner, false);
            if (potentialTarget is null)
                GoNearOwner();
            else
                AttackTarget(potentialTarget);
            EnforceGravity();
        }

        internal void ProvidePlayerMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<FleshOfInfidelityBuff>(), 3600);

            // Verify player/minion state integrity. The minion cannot stay alive if the
            // owner is dead or if the caller of the AI is invalid.
            if (projectile.type != ModContent.ProjectileType<FleshBallMinion>())
                return;

            if (Owner.dead)
                Owner.Calamity().fleshBall = false;
            if (Owner.Calamity().fleshBall)
                projectile.timeLeft = 2;
        }

        internal void DetermineFrames()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 5 == 4)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }

        internal void EnforceGravity()
        {
            if (projectile.velocity.Y < MaxFallSpeed)
                projectile.velocity.Y += Gravity;
        }

        internal void GenerateVisuals()
        {
            // All code within this method is visual. There is no need to waste resources executing it on the server.
            if (Main.dedServ)
                return;

            projectile.rotation += projectile.velocity.X * 0.05f;

            Vector2 shootOffsetDirection = -Vector2.UnitY.RotatedBy(projectile.rotation + projectile.direction * 0.2f);
            Dust blood = Dust.NewDustDirect(projectile.Center + shootOffsetDirection * 10f - new Vector2(4f), 0, 0, DustID.Blood, newColor: Color.Transparent);
            blood.velocity = shootOffsetDirection.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 4f);
            blood.noGravity = true;
        }

        internal void SufferFromSeparationAnxiety()
        {
            float teleportPromptDistance = Collision.CanHitLine(projectile.Center, 1, 1, Owner.Center, 1, 1) ? 1900f : 805f;
            if (Main.myPlayer == projectile.owner && !projectile.WithinRange(Owner.Center, teleportPromptDistance))
            {
                projectile.Center = Owner.Center;
                projectile.netUpdate = true;
            }
        }

        internal void GoNearOwner()
        {
            projectile.tileCollide = true;

            bool closeToPlayer = projectile.WithinRange(Owner.Center, 150f);
            if (!closeToPlayer && SittingOnGround && HopTimer % 30f == 29f)
            {
                projectile.velocity = projectile.SafeDirectionTo(Owner.Center) * 9f + new Vector2(Math.Sign(projectile.velocity.X) * 2f, -9f);

                // Don't collide with tiles for 1 frame, to prevent slopes from being a nuisance.
                projectile.tileCollide = false;
                projectile.netUpdate = true;
            }
        }

        internal void AttackTarget(NPC target)
        {
            projectile.tileCollide = true;

            if (SittingOnGround && HopTimer % 20f == 19f)
            {
                projectile.velocity = projectile.SafeDirectionTo(target.Center) * 6f + new Vector2(Math.Sign(projectile.velocity.X) * 2f, -7f);
                HopAmount++;

                // Release a bunch of blood.
                if (Main.myPlayer == projectile.owner && HopAmount % 3f == 2f)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 shootVelocity = -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(6f, 11f);
                        int blood = Projectile.NewProjectile(projectile.Top, shootVelocity, ModContent.ProjectileType<FleshBlood>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }

                // Don't collide with tiles for 1 frame, to prevent slopes from being a nuisance.
                projectile.tileCollide = false;
                projectile.netUpdate = true;
            }
        }

        // Prevent immediate death on tile collision, and slow down on tile collision.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity.X *= 0.9f;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = projectile.Bottom.Y < Owner.Top.Y;
            return true;
        }
    }
}
