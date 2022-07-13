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
        public Player Owner => Main.player[Projectile.owner];
        public bool SittingOnGround => Math.Abs(Projectile.velocity.X) < 1.55f && Projectile.velocity.Y == 0f;
        public ref float HopTimer => ref Projectile.ai[0];
        public ref float HopAmount => ref Projectile.ai[1];
        public const float Gravity = 0.25f;
        public const float MaxFallSpeed = 12f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh Ball");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            ProvidePlayerMinionBuffs();
            DetermineFrames();
            GenerateVisuals();

            HopTimer++;
            SufferFromSeparationAnxiety();
            NPC potentialTarget = Projectile.Center.MinionHoming(950f, Owner, false);
            if (potentialTarget is null)
                GoNearOwner();
            else
                AttackTarget(potentialTarget);
            EnforceGravity();
        }

        internal void ProvidePlayerMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<FleshBallBuff>(), 3600);

            // Verify player/minion state integrity. The minion cannot stay alive if the
            // owner is dead or if the caller of the AI is invalid.
            if (Projectile.type != ModContent.ProjectileType<FleshBallMinion>())
                return;

            if (Owner.dead)
                Owner.Calamity().fleshBall = false;
            if (Owner.Calamity().fleshBall)
                Projectile.timeLeft = 2;
        }

        internal void DetermineFrames()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

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
