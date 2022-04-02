using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.Projectiles.Summon
{
    public class DaedalusGolem : ModProjectile
    {
        public int AttackTimer;
        public bool UsingChargedLaserAttack;
        public const int ChargedPelletAttackTime = 30;
        public const int ChargedLaserAttackTime = 120;
        public const float Gravity = 0.35f;
        public bool Stuck => StuckWalkThroughWallsTimer >= 40f || Collision.SolidCollision(projectile.Center, 2, 2);
        public Vector2 ArmPosition => projectile.Center + new Vector2(projectile.spriteDirection == 1 ? -4f : 32f, 0f);
        public Player Owner => Main.player[projectile.owner];
        public ref float StuckWalkThroughWallsTimer => ref projectile.ai[0];
        public ref float StuckJumpSpeed => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Golem");
            Main.projFrames[projectile.type] = 18;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 58;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.minion = true;
            projectile.tileCollide = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackTimer);
            writer.Write(UsingChargedLaserAttack);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackTimer = reader.ReadInt32();
            UsingChargedLaserAttack = reader.ReadBoolean();
        }

        #region AI
        public override void AI()
        {
            Main.projFrames[projectile.type] = 16;
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<DaedalusGolem>();
            Owner.AddBuff(ModContent.BuffType<DaedalusGolemBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (Owner.dead)
                    Owner.Calamity().daedalusGolem = false;

                if (Owner.Calamity().daedalusGolem)
                    projectile.timeLeft = 2;
            }

            AdjustMinionDamage();

            // Fall down.
            if (projectile.velocity.Y < 15f)
                projectile.velocity.Y += Gravity;

            Vector2 destination;
            NPC potentialTarget = projectile.Center.MinionHoming(900f, Owner);
            if (potentialTarget is null)
                destination = Owner.Center - Vector2.UnitX * (80f + (projectile.identity * 28f) % 560f) * Owner.direction;
            else
            {
                Vector2 destA = potentialTarget.Center + Vector2.UnitX * (130f + (projectile.identity * 28f) % 560f);
                Vector2 destB = potentialTarget.Center - Vector2.UnitX * (130f + (projectile.identity * 28f) % 560f);
                if ((projectile.Center - destA).Length() < (projectile.Center - destB).Length())
                    destination = destA;
                else
                    destination = destB;
            }

            // Go upwards, and check down again to discover any height differences before deciding where to move.
            // There's a chance that this will encounter a null tile. If it us, just skip this step.
            try
            {
                Vector2 upwardCheck = destination - Vector2.UnitY * 2400f;
                upwardCheck.Y = Utils.Clamp(upwardCheck.Y, 0f, Main.maxTilesY * 16f - 100f);
                WorldUtils.Find(upwardCheck.ToTileCoordinates(), Searches.Chain(new Searches.Down(200), new Conditions.IsSolid()), out Point loweredPoint);
                destination = loweredPoint.ToWorldCoordinates();
            }
            catch (NullReferenceException) { }

            StuckWalkThroughWallsTimer = Utils.Clamp(StuckWalkThroughWallsTimer, 0, 160);

            if (projectile.Distance(Owner.Center) > 3500f)
            {
                projectile.Center = Owner.Center;
                StuckWalkThroughWallsTimer = 0;
                projectile.netImportant = true;
            }
            if ((MoveToDestination(destination) || AttackTimer > 0) && potentialTarget != null)
            {
                AttackTimer++;
                if (AttackTimer == 1)
                {
                    UsingChargedLaserAttack = Main.rand.NextBool(7);
                    projectile.netUpdate = true;
                }

                if (AttackTimer >= (UsingChargedLaserAttack ? ChargedLaserAttackTime : ChargedPelletAttackTime))
                {
                    AttackTimer = 0;
                    projectile.netUpdate = true;
                }

                if (MathHelper.Distance(potentialTarget.Center.X, projectile.Center.X) > 30f)
                    projectile.spriteDirection = (potentialTarget.Center.X - projectile.Center.X < 0).ToDirectionInt();

                if (UsingChargedLaserAttack)
                {
                    if (AttackTimer >= 45 && AttackTimer < ChargedLaserAttackTime / 2 && !Main.dedServ)
                    {
                        Vector2 drawOffset = Main.rand.NextVector2CircularEdge(12f, 12f);
                        Dust light = Dust.NewDustPerfect(ArmPosition + drawOffset, 261);
                        light.velocity = drawOffset.SafeNormalize(Vector2.Zero) * -2.5f;
                        light.color = Color.Lerp(Color.HotPink, Color.LightPink, Main.rand.NextFloat());
                        light.scale = Main.rand.NextFloat(1.2f, 1.45f);
                        light.noGravity = true;
                    }
                    else if (AttackTimer >= ChargedLaserAttackTime / 2 && AttackTimer <= ChargedLaserAttackTime / 2 + 60 && AttackTimer % 16 == 15)
                    {
                        Main.PlaySound(SoundID.Item122, ArmPosition);
                        if (Main.myPlayer == projectile.owner)
                        {
                            Vector2 initialVelocity = projectile.SafeDirectionTo(potentialTarget.Center) * 2f;
                            if (Main.rand.NextBool(2))
                                initialVelocity = initialVelocity.RotatedByRandom(0.4f);
                            float initialAngle = initialVelocity.ToRotation();
                            Projectile.NewProjectile(ArmPosition, initialVelocity, ModContent.ProjectileType<DaedalusLightning>(), projectile.damage, projectile.knockBack, projectile.owner, initialAngle, Main.rand.Next(100));
                        }
                    }
                }
                else if (!UsingChargedLaserAttack && AttackTimer == ChargedPelletAttackTime / 2 && Main.myPlayer == projectile.owner)
                {
                    Vector2 initialVelocity = projectile.SafeDirectionTo(potentialTarget.Center + potentialTarget.velocity * 15f) * 19f;
                    Projectile.NewProjectile(ArmPosition, initialVelocity, ModContent.ProjectileType<DaedalusPellet>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
            else if (potentialTarget is null && AttackTimer != 0)
            {
                AttackTimer = 0;
                projectile.netUpdate = true;
            }
        }

        public void AdjustMinionDamage()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int newDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Owner.MinionDamage());
                projectile.damage = newDamage;
            }
        }

        public bool MoveToDestination(Vector2 destination)
        {
            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(projectile.Bottom.X / 16), (int)(projectile.Bottom.Y / 16));

            // Float to the destination if stuck.
            if (Stuck)
            {
                StuckJumpSpeed = 0f;
                projectile.tileCollide = false;

                if (projectile.DistanceSQ(destination - Vector2.UnitY * 16f) > 10f * 10f)
                    projectile.velocity = projectile.SafeDirectionTo(destination - Vector2.UnitY * 16f) * 6f;
                else
                    StuckWalkThroughWallsTimer = 0;

                StuckWalkThroughWallsTimer -= 4;
                return false;
            }

            projectile.tileCollide = true;
            // Don't bother moving any more if super close to the destination.
            // Just slow down and face the player.
            if (Math.Abs(projectile.Center.X - destination.X) < 55 + Math.Abs(projectile.velocity.X))
            {
                StuckJumpSpeed = 0f;
                projectile.velocity.X *= 0.8f;
                return true;
            }

            int currentWalkDirection = Math.Sign(projectile.velocity.X);
            int tilesSearchedAhead = 0;

            Tile tileBelowAhead;

            do
            {
                tileBelowAhead = CalamityUtils.ParanoidTileRetrieval((int)(projectile.Bottom.X / 16) + currentWalkDirection, (int)(projectile.Bottom.Y / 16));

                if (tileBelowAhead.IsTileSolidGround())
                    break;

                tilesSearchedAhead++;
            }
            while (tilesSearchedAhead < 4);

            int directionToWalk = Math.Sign(destination.X - projectile.Center.X);
            float idealWalkSpeed = 10f * directionToWalk;
            float walkAcceleration = directionToWalk != currentWalkDirection ? 0.325f : 0.2f;
            projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, idealWalkSpeed, walkAcceleration);

            // Jump if there's a gap or a wall.
            if (tileBelow.IsTileSolidGround() || Collision.SolidCollision(projectile.Center, 10, 10))
            {
                if (Math.Abs(projectile.oldPosition.X - projectile.position.X) < 2f ||
                    Collision.SolidCollision(projectile.Center, 2, 2) ||
                    !Collision.CanHitLine(projectile.position, projectile.width, projectile.height, Owner.position, Owner.width, Owner.height))
                {
                    projectile.velocity.Y = -12f - StuckJumpSpeed;
                    projectile.netSpam -= 10;
                    StuckJumpSpeed += 3.5f;
                    StuckJumpSpeed = Utils.Clamp(StuckJumpSpeed, 0f, 14f);

                    StuckWalkThroughWallsTimer += 10f;

                    projectile.netUpdate = true;
                }
                else if (tilesSearchedAhead > 0)
                {
                    projectile.velocity.X = 7f;

                    projectile.velocity.Y = -(5f + tilesSearchedAhead * 2f);
                    projectile.netSpam -= 10;
                    projectile.netUpdate = true;
                }
                else
                {
                    StuckJumpSpeed = 0f;
                    StuckWalkThroughWallsTimer -= 5f;
                }
            }

            projectile.spriteDirection = (Owner.Center.X - projectile.Center.X < 0).ToDirectionInt();
            return false;
        }
        #endregion

        // Prevent on-tile collision death.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = projectile.Bottom.Y < Owner.Top.Y;
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int startingWalkFrame = 6;
            int endingWalkFrame = 10;

            int startingPelletFrame = 1;
            int endingPelletFrame = 4;

            int startingBeamFrame = 11;
            int endingBeamFrame = 15;

            // Walking frames. Moves more quickly the faster the walk.
            projectile.frameCounter++;
            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(projectile.Bottom.X / 16), (int)(projectile.Bottom.Y / 16));

            if (Stuck)
            {
                projectile.frame = 5;
            }
            else if (AttackTimer > 0)
            {
                if (UsingChargedLaserAttack)
                    projectile.frame = (int)MathHelper.Lerp(startingBeamFrame, endingBeamFrame + 1, Utils.InverseLerp(0f, ChargedLaserAttackTime, AttackTimer, true));
                else
                    projectile.frame = (int)MathHelper.Lerp(startingPelletFrame, endingPelletFrame + 1, Utils.InverseLerp(0f, ChargedPelletAttackTime, AttackTimer, true));
            }
            else if (Math.Abs(projectile.velocity.X) > 5f && Math.Abs(projectile.velocity.Y) < 2f && tileBelow.IsTileSolidGround())
            {
                if (projectile.frameCounter >= Utils.Clamp(2, 6, (int)Math.Abs(projectile.velocity.X * 0.8)))
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame >= endingWalkFrame)
                        projectile.frame = startingWalkFrame;
                }
                projectile.frame = Utils.Clamp(projectile.frame, startingWalkFrame, endingWalkFrame);
            }
            else if (Math.Abs(projectile.velocity.X) <= 1f)
            {
                projectile.frame = 0;
            }
        }

        public override bool CanDamage() => false;
    }
}
