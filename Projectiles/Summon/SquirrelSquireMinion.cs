using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SquirrelSquireMinion : ModProjectile
    {
        public int StuckTimer;
        public bool TryingToGetCloseToOwner
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value.ToInt();
        }
        public ref float AttackTimer => ref projectile.ai[1];
        public bool OnSolidGround
        {
            get
            {
                bool groundSolid = false;
                for (int i = (int)projectile.Left.X / 16 - 1; i < (int)projectile.Right.X / 16 + 1; i++)
                {
                    bool bottomTileSolid = CalamityUtils.ParanoidTileRetrieval(i, (int)projectile.Bottom.Y / 16).IsTileSolidGround();
                    bool firstTileDownSolid = CalamityUtils.ParanoidTileRetrieval(i, (int)projectile.Bottom.Y / 16 + 1).IsTileSolidGround();
                    bool secondTileDownSolid = CalamityUtils.ParanoidTileRetrieval(i, (int)projectile.Bottom.Y / 16 + 2).IsTileSolidGround();
                    groundSolid |= bottomTileSolid || firstTileDownSolid || secondTileDownSolid;
                }
                return projectile.velocity.Y == 0f && groundSolid;
            }
        }
        public bool Attacking
        {
            get => projectile.localAI[1] == 1f;
            set => projectile.localAI[1] = value.ToInt();
        }
        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squirrel Squire");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 17;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 64;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(StuckTimer);

        public override void ReceiveExtraAI(BinaryReader reader) => StuckTimer = reader.ReadInt32();

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                DoInitializationEffects();
                projectile.localAI[0] = 1f;
            }

            // Re-adjust damage as needed.
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue / projectile.Calamity().spawnedPlayerMinionDamageValue * Owner.MinionDamage());
                projectile.damage = trueDamage;
            }

            projectile.frameCounter++;

            VerifyIdentity();
            if (!projectile.WithinRange(Owner.Center, 1000f) || TryingToGetCloseToOwner || Collision.SolidCollision(projectile.position + Vector2.One * 4f, projectile.width - 8, projectile.height - 8))
            {
                // Once far off, don't go back to normal until 350 pixels away from the owner or less.
                TryingToGetCloseToOwner = !projectile.WithinRange(Owner.Center, 350f) && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height);

                ReturnToOwner();
                if (!projectile.WithinRange(Owner.Center, 2000f))
                {
                    projectile.Center = Owner.Center;
                    projectile.netUpdate = true;
                }
                projectile.tileCollide = false;
                return;
            }

            Attacking = false;
            NPC potentialTarget = projectile.Center.MinionHoming(800f, Owner, false);
            if (potentialTarget is null)
                FollowOwner();
            else
            {
                Attacking = true;
                AttackTarget(potentialTarget);
            }

            projectile.rotation = 0f;
            projectile.tileCollide = true;
        }

        public void DoInitializationEffects()
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

            int dustQuantity = 36;
            for (int d = 0; d < dustQuantity; d++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, 7);
                dust.scale = 1.4f;
                dust.velocity = (MathHelper.TwoPi * d / dustQuantity).ToRotationVector2() * 4f;
                dust.noGravity = true;
                dust.noLight = true;
            }
        }

        public void VerifyIdentity()
        {
            Owner.AddBuff(ModContent.BuffType<SquirrelSquireBuff>(), 3600);
            if (projectile.type == ModContent.ProjectileType<SquirrelSquireMinion>())
            {
                if (Owner.dead)
                    Owner.Calamity().squirrel = false;
                if (Owner.Calamity().squirrel)
                    projectile.timeLeft = 2;
            }
        }

        public void ReturnToOwner()
        {
            projectile.frame = 12 + projectile.frameCounter / 8 % 4;
            projectile.velocity = projectile.velocity.MoveTowards(projectile.SafeDirectionTo(Owner.Top) * 8f, 0.8f);
            projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
            projectile.rotation = projectile.velocity.X * 0.025f;
        }

        public void AttemptToJumpToDestination(Vector2 destination)
        {
            if (OnSolidGround)
            {
                float jumpSpeed = MathHelper.Lerp(8f, 13f, Utils.InverseLerp(100f, 250f, MathHelper.Distance(projectile.Center.X, destination.X), true));
                projectile.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(projectile.Center, Owner.Center, 0.4f, jumpSpeed);
                projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
                projectile.netUpdate = true;
                projectile.frame = 4;
            }
            else
            {
                if (projectile.frameCounter % 5 == 4 && projectile.frame < 7)
                    projectile.frame++;
            }
        }

        public void FollowOwner()
        {
            projectile.rotation = 0f;

            // Slow down and just idly vibe if close enough to the owner.
            float horizontalDistanceFromOwner = MathHelper.Distance(projectile.Center.X, Owner.Center.X);
            float verticalDistanceFromOwner = MathHelper.Distance(projectile.Center.Y, Owner.Center.Y);
            if (horizontalDistanceFromOwner < 100f && verticalDistanceFromOwner < 450f)
            {
                projectile.velocity.X *= 0.94f;
                if (OnSolidGround)
                {
                    projectile.frame = projectile.frameCounter / 5 % 4;
                    projectile.velocity *= 0.9f;
                }
                StuckTimer = 0;
            }

            // Otherwise try to catch up with them on foot.
            else
            {
                AttemptToJumpToDestination(Owner.Center);
                // Determine if there's something in the way of this minion and its owner.
                if (!Collision.CanHit(projectile.position, projectile.width, projectile.height, Owner.Center, 2, 2) || MathHelper.Distance(projectile.position.X, projectile.oldPosition.X) < 1f)
                {
                    StuckTimer++;

                    // If stuck for a sufficiently long time fly to the owner again.
                    if (StuckTimer > 180)
                    {
                        TryingToGetCloseToOwner = true;
                        StuckTimer = 0;
                        projectile.netUpdate = true;
                    }
                }
                else if (StuckTimer > 0)
                    StuckTimer--;
            }

            // Enforce gravity.
            projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y + 0.4f, -12f, 12f);
        }

        public void AttackTarget(NPC target)
        {
            float horizontalDistanceFromTarget = MathHelper.Distance(projectile.Center.X, target.Center.X);

            // Determine if the minion is stuck.
            if (MathHelper.Distance(projectile.position.X, projectile.oldPosition.X) < 1f)
            {
                StuckTimer++;

                // If stuck for a sufficiently long time fly to the owner again.
                if (StuckTimer > 180)
                {
                    TryingToGetCloseToOwner = true;
                    StuckTimer = 0;
                    projectile.netUpdate = true;
                }
            }
            else if (StuckTimer > 0)
                StuckTimer--;

            if (horizontalDistanceFromTarget > 350f)
                AttemptToJumpToDestination(target.Center);
            else if (OnSolidGround)
            {
                projectile.velocity.X *= 0.94f;
                if (OnSolidGround)
                {
                    projectile.frame = projectile.frameCounter / 5 % 4;
                    projectile.velocity *= 0.9f;
                }
                AttackTimer++;

                // Pelt the target with acorns.
                projectile.frame = 8 + (int)(AttackTimer / 6.4) % 4;
                if (Main.myPlayer == projectile.owner && AttackTimer % 30f == 27f)
                {
                    projectile.spriteDirection = (target.Center.X > projectile.Center.X).ToDirectionInt();
                    Vector2 acornSpawnPosition = projectile.Center + new Vector2(projectile.spriteDirection * 6f, 10f);
                    float acornShootSpeed = MathHelper.Lerp(15f, 32f, projectile.Distance(target.Center) / 800f);
                    Vector2 acornShootVelocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(acornSpawnPosition, target.Top + target.velocity * 25f, SquirrelSquireAcorn.Gravity, acornShootSpeed);

                    if (projectile.WithinRange(target.Center, 200f))
                        acornShootVelocity = (target.Center - acornSpawnPosition).SafeNormalize(-Vector2.UnitY) * acornShootSpeed;

                    Projectile.NewProjectile(acornSpawnPosition, acornShootVelocity, ModContent.ProjectileType<SquirrelSquireAcorn>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
            projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y + 0.4f, -12f, 12f);
        }

        public override bool CanDamage() => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = !Attacking && projectile.Bottom.Y < Owner.Top.Y;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void Kill(int timeLeft)
        {
            int index = Gore.NewGore(projectile.Center, Vector2.Zero, Main.rand.Next(61, 64), projectile.scale);
            Main.gore[index].velocity *= 0.1f;
        }
    }
}
