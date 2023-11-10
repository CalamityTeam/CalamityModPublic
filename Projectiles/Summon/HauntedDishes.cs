using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HauntedDishes : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 19;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = Projectile.Calamity();
            if (dust == 0f)
            {
                int dustAmt = 36;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dusty = Dust.NewDust(rotate + faceDirection, 0, 0, 7, faceDirection.X * 1.1f, faceDirection.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = faceDirection;
                }
                dust += 1f;
            }
            bool projTypeCheck = Projectile.type == ModContent.ProjectileType<HauntedDishes>();
            player.AddBuff(ModContent.BuffType<HauntedDishesBuff>(), 3600);
            if (projTypeCheck)
            {
                if (player.dead)
                {
                    modPlayer.hauntedDishes = false;
                }
                if (modPlayer.hauntedDishes)
                {
                    Projectile.timeLeft = 2;
                }
            }
            bool minionMovingLeft = false;
            bool minionMovingRight = false;
            bool minionBelowPlayer = false;
            bool minionShouldJump = false;
            if (Projectile.lavaWet)
            {
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = 0f;
            }
            int idlePos = 40 * (Projectile.minionPos + 1) * player.direction;
            if (player.position.X + (float) (player.width / 2) < Projectile.position.X + (float) (Projectile.width / 2) - 10f + (float) idlePos)
                minionMovingLeft = true;
            else if (player.position.X + (float) (player.width / 2) > Projectile.position.X + (float) (Projectile.width / 2) + 10f + (float) idlePos)
                minionMovingRight = true;

            if (Projectile.ai[1] == 0f)
            {
                int conflict1 = 500;
                conflict1 += 40 * Projectile.minionPos;
                if (Projectile.localAI[0] > 0f)
                    conflict1 += 500;
                Vector2 idleMinionPos = new Vector2(Projectile.position.X + (float) Projectile.width * 0.5f, Projectile.position.Y + (float) Projectile.height * 0.5f);
                float playerX = player.position.X + (float) (player.width / 2) - idleMinionPos.X;
                float playerY = player.position.Y + (float) (player.height / 2) - idleMinionPos.Y;
                float playerDist = (float)Math.Sqrt(playerX * playerX + playerY * playerY);
                if (playerDist > 1500f)
                {
                    Projectile.ai[0] = 1f;
                }
                if (playerDist > 2000f) //teleport to player if too far
                {
                    Projectile.position.X = player.position.X + (float) (player.width / 2) - (float) (Projectile.width / 2);
                    Projectile.position.Y = player.position.Y + (float) (player.height / 2) - (float) (Projectile.height / 2);
                }
            }
            if (Projectile.ai[0] != 0f) //flying back to the player
            {
                Projectile.tileCollide = false;
                float npcDetectRange = 1200f;
                bool npcFound = false;
                int targetIndex = -1;
                for (int index = 0; index < Main.maxNPCs; ++index)
                {
                    NPC npc2 = Main.npc[index];
                    if (npc2.CanBeChasedBy((object) Projectile, false))
                    {
                        float npcX = npc2.position.X + (float) (npc2.width / 2);
                        float npcY = npc2.position.Y + (float) (npc2.height / 2);
                        float npcDist = (float) Math.Abs(player.position.X + (float) (player.width / 2) - npcX) + (float) Math.Abs(player.position.Y + (float) (player.height / 2) - npcY);
                        if (npcDist < npcDetectRange)
                        {
                            if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc2.position, npc2.width, npc2.height))
                                targetIndex = index;
                            npcFound = true;
                            break;
                        }
                    }
                }

                //return to normal if npc found
                if (npcFound && targetIndex >= 0)
                    Projectile.ai[0] = 0f;

                Vector2 returningMinionPos = new Vector2(Projectile.position.X + (float) Projectile.width * 0.5f, Projectile.position.Y + (float) Projectile.height * 0.5f);
                float xDist = player.position.X + (float) (player.width / 2) - returningMinionPos.X;
                xDist -= (float) (40 * player.direction);
                if (!npcFound)
                    xDist -= (float) (40 * Projectile.minionPos * player.direction);
                float yDist = player.position.Y + (float) (player.height / 2) - returningMinionPos.Y;
                yDist -= 60f;
                float playerDist2 = (float) Math.Sqrt(xDist * xDist + yDist * yDist);
                float currentReturnSpeed = 12f;
                float minionReturnSpeed = playerDist2;
                float minionReturnAccel = 0.4f;
                if (currentReturnSpeed < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                    currentReturnSpeed = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);

                //if close enough to the player and has tile to stand on, return to normal
                if (playerDist2 < 100f && player.velocity.Y == 0f && (Projectile.position.Y + (float) Projectile.height <= player.position.Y + (float) player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)))
                {
                    Projectile.ai[0] = 0f;
                    if (Projectile.velocity.Y < -6f)
                        Projectile.velocity.Y = -6f;
                }
                if (playerDist2 > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDist2 < 50f)
                {
                    if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                    {
                        Projectile.velocity *= 0.99f;
                    }
                    minionReturnAccel = 0.01f;
                }
                else
                {
                    if (playerDist2 < 100f)
                    {
                        minionReturnAccel = 0.1f;
                    }
                    if (playerDist2 > 300f)
                    {
                        minionReturnAccel = 1f;
                    }
                    playerDist2 = minionReturnSpeed / playerDist2;
                    xDist *= playerDist2;
                    yDist *= playerDist2;
                }
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + minionReturnAccel;
                    if (minionReturnAccel > 0.05f && Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + minionReturnAccel;
                    }
                }
                if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - minionReturnAccel;
                    if (minionReturnAccel > 0.05f && Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - minionReturnAccel;
                    }
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + minionReturnAccel;
                    if (minionReturnAccel> 0.05f && Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + minionReturnAccel * 2f;
                    }
                }
                if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - minionReturnAccel;
                    if (minionReturnAccel > 0.05f && Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - minionReturnAccel * 2f;
                    }
                }
                if (Projectile.frame < 15)
                {
                    Projectile.frame = 15;
                }
                else
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 3)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 19)
                    {
                        Projectile.frame = 15;
                    }
                }
                if (Projectile.velocity.X > 0.5f)
                    Projectile.spriteDirection = 1;
                else if (Projectile.velocity.X < -0.5f)
                    Projectile.spriteDirection = -1;
                Projectile.rotation = Projectile.spriteDirection != 1 ? (float) Math.Atan2((double) Projectile.velocity.Y, (double) Projectile.velocity.X) + 3.14f : (float) Math.Atan2((double) Projectile.velocity.Y, (double) Projectile.velocity.X);
            }
            else
            {
                float exaggeratedMinionPos = (float) (40 * Projectile.minionPos);
                float attackCooldown = 30f;
                --Projectile.localAI[0];
                if (Projectile.localAI[0] < 0f)
                    Projectile.localAI[0] = 0f;
                if (Projectile.ai[1] > 0f)
                {
                    --Projectile.ai[1];
                }
                else
                {
                    float minionXTarget = Projectile.position.X;
                    float minionYTarget = Projectile.position.Y;
                    float minionAttackMaxDist = 100000f;
                    float minionAttackDistance = minionAttackMaxDist;
                    int minionAttackIndex = -1;
                    NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                    if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy((object) Projectile, false))
                    {
                        float minionTargetXDist = minionAttackTargetNpc.position.X + (float) (minionAttackTargetNpc.width / 2);
                        float minionTargetYDist = minionAttackTargetNpc.position.Y + (float) (minionAttackTargetNpc.height / 2);
                        float minionTargetDist = Math.Abs(Projectile.position.X + (float) (Projectile.width / 2) - minionTargetXDist) + Math.Abs(Projectile.position.Y + (float) (Projectile.height / 2) - minionTargetYDist);
                        if (minionTargetDist < minionAttackMaxDist)
                        {
                            if (minionAttackIndex == -1 && minionTargetDist <= minionAttackDistance)
                            {
                                minionAttackDistance = minionTargetDist;
                                minionXTarget = minionTargetXDist;
                                minionYTarget = minionTargetYDist;
                            }
                            if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, minionAttackTargetNpc.position, minionAttackTargetNpc.width, minionAttackTargetNpc.height))
                            {
                                minionAttackMaxDist = minionTargetDist;
                                minionXTarget = minionTargetXDist;
                                minionYTarget = minionTargetYDist;
                                minionAttackIndex = minionAttackTargetNpc.whoAmI;
                            }
                        }
                    }
                    if (minionAttackIndex == -1)
                    {
                        for (int index = 0; index < Main.maxNPCs; ++index)
                        {
                            if (Main.npc[index].CanBeChasedBy((object) Projectile, false))
                            {
                                float minionTargetXDist = Main.npc[index].position.X + (float) (Main.npc[index].width / 2);
                                float minionTargetYDist = Main.npc[index].position.Y + (float) (Main.npc[index].height / 2);
                                float minionTargetDist = Math.Abs(Projectile.position.X + (float) (Projectile.width / 2) - minionTargetXDist) + Math.Abs(Projectile.position.Y + (float) (Projectile.height / 2) - minionTargetYDist);
                                if (minionTargetDist < minionAttackMaxDist)
                                {
                                    if (minionAttackIndex == -1 && minionTargetDist <= minionAttackDistance)
                                    {
                                        minionAttackDistance = minionTargetDist;
                                        minionXTarget = minionTargetXDist;
                                        minionYTarget = minionTargetYDist;
                                    }
                                    if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[index].position, Main.npc[index].width, Main.npc[index].height))
                                    {
                                        minionAttackMaxDist = minionTargetDist;
                                        minionXTarget = minionTargetXDist;
                                        minionYTarget = minionTargetYDist;
                                        minionAttackIndex = index;
                                    }
                                }
                            }
                        }
                    }
                    if (minionAttackIndex == -1 && minionAttackDistance < minionAttackMaxDist)
                        minionAttackMaxDist = minionAttackDistance;
                    float yDependentTargeting = 400f;
                    if ((double) Projectile.position.Y > Main.worldSurface * 16.0)
                        yDependentTargeting = 200f;
                    if (minionAttackMaxDist < yDependentTargeting + exaggeratedMinionPos && minionAttackIndex == -1)
                    {
                        float minionTargetXDist = minionXTarget - (Projectile.position.X + (float) (Projectile.width / 2));
                        if (minionTargetXDist < -5f)
                        {
                            minionMovingLeft = true;
                            minionMovingRight = false;
                        }
                        else if (minionTargetXDist > 5f)
                        {
                            minionMovingRight = true;
                            minionMovingLeft = false;
                        }
                    }
                    else if (minionAttackIndex >= 0 && minionAttackMaxDist < 800f + exaggeratedMinionPos)
                    {
                        Projectile.localAI[0] = 60f;
                        float minionTargetXDist = minionXTarget - (Projectile.position.X + (float) (Projectile.width / 2));
                        if (minionTargetXDist > 300f || minionTargetXDist < -300f)
                        {
                            if (minionTargetXDist < -50f)
                            {
                                minionMovingLeft = true;
                                minionMovingRight = false;
                            }
                            else if (minionTargetXDist > 50f)
                            {
                                minionMovingRight = true;
                                minionMovingLeft = false;
                            }
                        }
                        else if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.ai[1] = attackCooldown;
                            double plateSpeed = 12.0;
                            Vector2 projMinionPos = new Vector2(Projectile.Center.X, Projectile.Center.Y - 8f);
                            float plateTargetX = minionXTarget - projMinionPos.X + Main.rand.NextFloat(-6f, 6f);
                            float randomPlateYOffset = (float) ((double) (Math.Abs(plateTargetX) * 0.1f) * (double) Main.rand.Next(0, 100) * (1.0 / 1000.0));
                            float plateTargetY = minionYTarget - projMinionPos.Y + Main.rand.NextFloat(-6f, 6f) - randomPlateYOffset;
                            double plateTargetDist = Math.Sqrt((double) plateTargetX * (double) plateTargetX + (double) plateTargetY * (double) plateTargetY);
                            float plateVelocity = (float) (plateSpeed / plateTargetDist);
                            float SpeedX = plateTargetX * plateVelocity;
                            float SpeedY = plateTargetY * plateVelocity;
                            int damage = Projectile.damage;
                            int Type = ModContent.ProjectileType<PlateProjectile>();
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), projMinionPos.X, projMinionPos.Y, SpeedX * 2f, SpeedY * 2f, Type, damage, Projectile.knockBack, Projectile.owner);
                            if (SpeedX < 0f)
                                Projectile.direction = -1;
                            if (SpeedX > 0f)
                                Projectile.direction = 1;
                            Projectile.netUpdate = true;
                        }
                    }
                }
                if (Projectile.ai[1] != 0f)
                {
                    minionMovingLeft = false;
                    minionMovingRight = false;
                }
                else if (Projectile.localAI[0] == 0f)
                    Projectile.direction = player.direction;
                Projectile.rotation = 0f;
                Projectile.tileCollide = true;
                float groundMinionAccel = 0.08f;
                float groundMinionMaxVel = 6.5f;
                groundMinionMaxVel = 6f;
                groundMinionAccel = 0.2f;
                if (groundMinionMaxVel < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                {
                    groundMinionMaxVel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    groundMinionAccel = 0.3f;
                }
                if (minionMovingLeft)
                {
                    if (Projectile.velocity.X > -3.5f)
                        Projectile.velocity.X -= groundMinionAccel;
                    else
                        Projectile.velocity.X -= groundMinionAccel * 0.25f;
                }
                else if (minionMovingRight)
                {
                    if (Projectile.velocity.X < 3.5f)
                        Projectile.velocity.X += groundMinionAccel;
                    else
                        Projectile.velocity.X += groundMinionAccel * 0.25f;
                }
                else
                {
                    Projectile.velocity.X *= 0.9f;
                    if (Projectile.velocity.X >= -groundMinionAccel && Projectile.velocity.X <= groundMinionAccel)
                        Projectile.velocity.X = 0f;
                }
                if (minionMovingLeft | minionMovingRight)
                {
                    int minionTileX = (int) ((double) Projectile.position.X + (double) (Projectile.width / 2)) / 16;
                    int j = (int) ((double) Projectile.position.Y + (double) (Projectile.height / 2)) / 16;
                    if (minionMovingLeft)
                        --minionTileX;
                    if (minionMovingRight)
                        ++minionTileX;
                    if (WorldGen.SolidTile(minionTileX + (int) Projectile.velocity.X, j))
                        minionShouldJump = true;
                }
                if (player.position.Y + player.height - 8f > Projectile.position.Y + Projectile.height)
                    minionBelowPlayer = true;
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY, 1, false, 0);
                if (Projectile.velocity.Y == 0f)
                {
                    if (!minionBelowPlayer && ((double) Projectile.velocity.X < 0.0 || (double) Projectile.velocity.X > 0.0))
                    {
                        int i = (int) ((double) Projectile.position.X + (double) (Projectile.width / 2)) / 16;
                        int j = (int) ((double) Projectile.position.Y + (double) (Projectile.height / 2)) / 16 + 1;
                        if (minionMovingLeft)
                            --i;
                        if (minionMovingRight)
                            ++i;
                        WorldGen.SolidTile(i, j);
                    }
                    if (minionShouldJump)
                    {
                        int i1 = (int) ((double) Projectile.position.X + (double) (Projectile.width / 2)) / 16;
                        int j = (int) ((double) Projectile.position.Y + (double) Projectile.height) / 16 + 1;
                        if (WorldGen.SolidTile(i1, j) || Main.tile[i1, j].IsHalfBlock || ((int) Main.tile[i1, j].Slope > 0 || Projectile.type == 200))
                        {
                            if (Projectile.type == 200)
                            {
                                Projectile.velocity.Y = -3.1f;
                            }
                            else
                            {
                                try
                                {
                                    int minionJumpTileX = (int) ((double) Projectile.position.X + (double) (Projectile.width / 2)) / 16;
                                    int minionJumpTileY = (int) ((double) Projectile.position.Y + (double) (Projectile.height / 2)) / 16;
                                    if (minionMovingLeft)
                                        --minionJumpTileX;
                                    if (minionMovingRight)
                                        ++minionJumpTileX;
                                    int i2 = minionJumpTileX + (int) Projectile.velocity.X;
                                    if (!WorldGen.SolidTile(i2, minionJumpTileY - 1) && !WorldGen.SolidTile(i2, minionJumpTileY - 2))
                                        Projectile.velocity.Y = -5.1f;
                                    else if (!WorldGen.SolidTile(i2, minionJumpTileY - 2))
                                        Projectile.velocity.Y = -7.1f;
                                    else if (WorldGen.SolidTile(i2, minionJumpTileY - 5))
                                        Projectile.velocity.Y = -11.1f;
                                    else if (WorldGen.SolidTile(i2, minionJumpTileY - 4))
                                        Projectile.velocity.Y = -10.1f;
                                    else
                                        Projectile.velocity.Y = -9.1f;
                                }
                                catch
                                {
                                    Projectile.velocity.Y = -9.1f;
                                }
                            }
                        }
                    }
                    else if (Projectile.type == 266 && minionMovingLeft | minionMovingRight)
                        Projectile.velocity.Y -= 6f;
                }
                if (Projectile.velocity.X > groundMinionMaxVel)
                    Projectile.velocity.X = groundMinionMaxVel;
                if (Projectile.velocity.X < -groundMinionMaxVel)
                    Projectile.velocity.X = -groundMinionMaxVel;
                if (Projectile.velocity.X < 0f)
                    Projectile.direction = -1;
                if (Projectile.velocity.X > 0f)
                    Projectile.direction = 1;
                if (Projectile.velocity.X > groundMinionAccel & minionMovingRight)
                    Projectile.direction = 1;
                if (Projectile.velocity.X < -groundMinionAccel & minionMovingLeft)
                    Projectile.direction = -1;
                if (Projectile.direction == -1)
                    Projectile.spriteDirection = -1;
                if (Projectile.direction == 1)
                    Projectile.spriteDirection = 1;
                if (Projectile.ai[1] > 0f)
                {
                    if (Projectile.localAI[1] == 0f)
                    {
                        Projectile.localAI[1] = 1f;
                        Projectile.frame = 9;
                    }
                    if (Projectile.frame >= 9 && Projectile.frame <= 14)
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 8)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame == 14)
                            Projectile.frame = 9;
                    }
                }
                else if (Projectile.velocity.Y == 0f)
                {
                    Projectile.localAI[1] = 0f;
                    if (Projectile.velocity.X == 0f)
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 4)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame >= 3)
                        {
                            Projectile.frame = 0;
                        }
                    }
                    else if (Projectile.velocity.X < -0.8f || Projectile.velocity.X > 0.8f)
                    {
                        Projectile.frameCounter = Projectile.frameCounter + (int) Math.Abs(Projectile.velocity.X);
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 20)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame < 4)
                            Projectile.frame = 3;
                        if (Projectile.frame >= 9)
                            Projectile.frame = 3;
                    }
                    else
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 4)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame >= 3)
                        {
                            Projectile.frame = 0;
                        }
                    }
                }
                else if (Projectile.velocity.Y < 0f)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 3;
                }
                else if (Projectile.velocity.Y > 0f)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 3;
                }
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;
                Vector2 velocity = Projectile.velocity;
            }
        }

        public override bool? CanDamage() => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                int index = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X - (float)(Projectile.width / 2), Projectile.position.Y - (float)(Projectile.height / 2)), new Vector2(0.0f, 0.0f), Main.rand.Next(61, 64), Projectile.scale);
                Main.gore[index].velocity *= 0.1f;
            }
        }
    }
}
