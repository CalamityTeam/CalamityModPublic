using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class DaedalusGolem : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int AttackTimer;
        public bool UsingChargedLaserAttack;
        public const int ChargedPelletAttackTime = 30;
        public const int ChargedLaserAttackTime = 120;
        public const float Gravity = 0.35f;
        public bool Stuck => StuckWalkThroughWallsTimer >= 40f || Collision.SolidCollision(Projectile.Center, 2, 2);
        public Vector2 ArmPosition => Projectile.Center + new Vector2(Projectile.spriteDirection == 1 ? -4f : 32f, 0f);
        public Player Owner => Main.player[Projectile.owner];
        public ref float StuckWalkThroughWallsTimer => ref Projectile.ai[0];
        public ref float StuckJumpSpeed => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 18;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 58;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Summon;
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
            Main.projFrames[Projectile.type] = 16;
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<DaedalusGolem>();
            Owner.AddBuff(ModContent.BuffType<DaedalusGolemBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (Owner.dead)
                    Owner.Calamity().daedalusGolem = false;

                if (Owner.Calamity().daedalusGolem)
                    Projectile.timeLeft = 2;
            }

            // Fall down.
            if (Projectile.velocity.Y < 15f)
                Projectile.velocity.Y += Gravity;

            Vector2 destination;
            NPC potentialTarget = Projectile.Center.MinionHoming(900f, Owner);
            if (potentialTarget is null)
                destination = Owner.Center - Vector2.UnitX * (80f + (Projectile.identity * 28f) % 560f) * Owner.direction;
            else
            {
                Vector2 destA = potentialTarget.Center + Vector2.UnitX * (130f + (Projectile.identity * 28f) % 560f);
                Vector2 destB = potentialTarget.Center - Vector2.UnitX * (130f + (Projectile.identity * 28f) % 560f);
                if ((Projectile.Center - destA).Length() < (Projectile.Center - destB).Length())
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

            if (Projectile.Distance(Owner.Center) > 3500f)
            {
                Projectile.Center = Owner.Center;
                StuckWalkThroughWallsTimer = 0;
                Projectile.netImportant = true;
            }
            if ((MoveToDestination(destination) || AttackTimer > 0) && potentialTarget != null)
            {
                AttackTimer++;
                if (AttackTimer == 1)
                {
                    UsingChargedLaserAttack = Main.rand.NextBool(7);
                    Projectile.netUpdate = true;
                }

                if (AttackTimer >= (UsingChargedLaserAttack ? ChargedLaserAttackTime : ChargedPelletAttackTime))
                {
                    AttackTimer = 0;
                    Projectile.netUpdate = true;
                }

                if (MathHelper.Distance(potentialTarget.Center.X, Projectile.Center.X) > 30f)
                    Projectile.spriteDirection = (potentialTarget.Center.X - Projectile.Center.X < 0).ToDirectionInt();

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
                        SoundEngine.PlaySound(SoundID.Item122, ArmPosition);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 initialVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 2f;
                            if (Main.rand.NextBool())
                                initialVelocity = initialVelocity.RotatedByRandom(0.4f);
                            float initialAngle = initialVelocity.ToRotation();
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), ArmPosition, initialVelocity, ModContent.ProjectileType<DaedalusLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner, initialAngle, Main.rand.Next(100));
                        }
                    }
                }
                else if (!UsingChargedLaserAttack && AttackTimer == ChargedPelletAttackTime / 2 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 initialVelocity = Projectile.SafeDirectionTo(potentialTarget.Center + potentialTarget.velocity * 15f) * 19f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), ArmPosition, initialVelocity, ModContent.ProjectileType<DaedalusPellet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            else if (potentialTarget is null && AttackTimer != 0)
            {
                AttackTimer = 0;
                Projectile.netUpdate = true;
            }
        }

        public bool MoveToDestination(Vector2 destination)
        {
            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(Projectile.Bottom.X / 16), (int)(Projectile.Bottom.Y / 16));

            // Float to the destination if stuck.
            if (Stuck)
            {
                StuckJumpSpeed = 0f;
                Projectile.tileCollide = false;

                if (Projectile.DistanceSQ(destination - Vector2.UnitY * 16f) > 10f * 10f)
                    Projectile.velocity = Projectile.SafeDirectionTo(destination - Vector2.UnitY * 16f) * 6f;
                else
                    StuckWalkThroughWallsTimer = 0;

                StuckWalkThroughWallsTimer -= 4;
                return false;
            }

            Projectile.tileCollide = true;
            // Don't bother moving any more if super close to the destination.
            // Just slow down and face the player.
            if (Math.Abs(Projectile.Center.X - destination.X) < 55 + Math.Abs(Projectile.velocity.X))
            {
                StuckJumpSpeed = 0f;
                Projectile.velocity.X *= 0.8f;
                return true;
            }

            int currentWalkDirection = Math.Sign(Projectile.velocity.X);
            int tilesSearchedAhead = 0;

            Tile tileBelowAhead;

            do
            {
                tileBelowAhead = CalamityUtils.ParanoidTileRetrieval((int)(Projectile.Bottom.X / 16) + currentWalkDirection, (int)(Projectile.Bottom.Y / 16));

                if (tileBelowAhead.IsTileSolidGround())
                    break;

                tilesSearchedAhead++;
            }
            while (tilesSearchedAhead < 4);

            int directionToWalk = Math.Sign(destination.X - Projectile.Center.X);
            float idealWalkSpeed = 10f * directionToWalk;
            float walkAcceleration = directionToWalk != currentWalkDirection ? 0.325f : 0.2f;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, idealWalkSpeed, walkAcceleration);

            // Jump if there's a gap or a wall.
            if (tileBelow.IsTileSolidGround() || Collision.SolidCollision(Projectile.Center, 10, 10))
            {
                if (Math.Abs(Projectile.oldPosition.X - Projectile.position.X) < 2f ||
                    Collision.SolidCollision(Projectile.Center, 2, 2) ||
                    !Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, Owner.position, Owner.width, Owner.height))
                {
                    Projectile.velocity.Y = -12f - StuckJumpSpeed;
                    Projectile.netSpam -= 10;
                    StuckJumpSpeed += 3.5f;
                    StuckJumpSpeed = Utils.Clamp(StuckJumpSpeed, 0f, 14f);

                    StuckWalkThroughWallsTimer += 10f;

                    Projectile.netUpdate = true;
                }
                else if (tilesSearchedAhead > 0)
                {
                    Projectile.velocity.X = 7f;

                    Projectile.velocity.Y = -(5f + tilesSearchedAhead * 2f);
                    Projectile.netSpam -= 10;
                    Projectile.netUpdate = true;
                }
                else
                {
                    StuckJumpSpeed = 0f;
                    StuckWalkThroughWallsTimer -= 5f;
                }
            }

            Projectile.spriteDirection = (Owner.Center.X - Projectile.Center.X < 0).ToDirectionInt();
            return false;
        }
        #endregion

        // Prevent on-tile collision death.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Projectile.Bottom.Y < Owner.Top.Y;
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            int startingWalkFrame = 6;
            int endingWalkFrame = 10;

            int startingPelletFrame = 1;
            int endingPelletFrame = 4;

            int startingBeamFrame = 11;
            int endingBeamFrame = 15;

            // Walking frames. Moves more quickly the faster the walk.
            Projectile.frameCounter++;
            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(Projectile.Bottom.X / 16), (int)(Projectile.Bottom.Y / 16));

            if (Stuck)
            {
                Projectile.frame = 5;
            }
            else if (AttackTimer > 0)
            {
                if (UsingChargedLaserAttack)
                    Projectile.frame = (int)MathHelper.Lerp(startingBeamFrame, endingBeamFrame + 1, Utils.GetLerpValue(0f, ChargedLaserAttackTime, AttackTimer, true));
                else
                    Projectile.frame = (int)MathHelper.Lerp(startingPelletFrame, endingPelletFrame + 1, Utils.GetLerpValue(0f, ChargedPelletAttackTime, AttackTimer, true));
            }
            else if (Math.Abs(Projectile.velocity.X) > 5f && Math.Abs(Projectile.velocity.Y) < 2f && tileBelow.IsTileSolidGround())
            {
                if (Projectile.frameCounter >= Utils.Clamp(2, 6, (int)Math.Abs(Projectile.velocity.X * 0.8)))
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= endingWalkFrame)
                        Projectile.frame = startingWalkFrame;
                }
                Projectile.frame = Utils.Clamp(Projectile.frame, startingWalkFrame, endingWalkFrame);
            }
            else if (Math.Abs(Projectile.velocity.X) <= 1f)
            {
                Projectile.frame = 0;
            }
        }

        public override bool? CanDamage() => false;
    }
}
