using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HauntedDishes : ModProjectile
    {
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HauntedDishes");
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
                for (int num227 = 0; num227 < dustAmt; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, 7, vector7.X * 1.1f, vector7.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = vector7;
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
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            int num1 = 60 + 30 * Projectile.minionPos;
            if (Projectile.lavaWet)
            {
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = 0f;
            }
            int num2 = 10;
            int num3 = 40 * (Projectile.minionPos + 1) * player.direction;
            if (player.position.X + (float) (player.width / 2) < Projectile.position.X + (float) (Projectile.width / 2) - (float) num2 + (float) num3)
                flag1 = true;
            else if (player.position.X + (float) (player.width / 2) > Projectile.position.X + (float) (Projectile.width / 2) + (float) num2 + (float) num3)
                flag2 = true;

            if (Projectile.ai[1] == 0f)
            {
                int conflict1 = 500;
                conflict1 += 40 * Projectile.minionPos;
                if (Projectile.localAI[0] > 0f)
                    conflict1 += 500;
                Vector2 vector2 = new Vector2(Projectile.position.X + (float) Projectile.width * 0.5f, Projectile.position.Y + (float) Projectile.height * 0.5f);
                float playerX = player.position.X + (float) (player.width / 2) - vector2.X;
                float playerY = player.position.Y + (float) (player.height / 2) - vector2.Y;
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
                int num6 = -1;
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
                                num6 = index;
                            npcFound = true;
                            break;
                        }
                    }
                }

                //return to normal if npc found
                if (npcFound && num6 >= 0)
                    Projectile.ai[0] = 0f;

                Vector2 vector2 = new Vector2(Projectile.position.X + (float) Projectile.width * 0.5f, Projectile.position.Y + (float) Projectile.height * 0.5f);
                float xDist = player.position.X + (float) (player.width / 2) - vector2.X;
                xDist -= (float) (40 * player.direction);
                if (!npcFound)
                    xDist -= (float) (40 * Projectile.minionPos * player.direction);
                float yDist = player.position.Y + (float) (player.height / 2) - vector2.Y;
                yDist -= 60f;
                float playerDist2 = (float) Math.Sqrt(xDist * xDist + yDist * yDist);
                float num11 = 12f;
                float num12 = playerDist2;
                float conflict2 = 0.4f;
                if (num11 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                    num11 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);

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
                    conflict2 = 0.01f;
                }
                else
                {
                    if (playerDist2 < 100f)
                    {
                        conflict2 = 0.1f;
                    }
                    if (playerDist2 > 300f)
                    {
                        conflict2 = 1f;
                    }
                    playerDist2 = num12 / playerDist2;
                    xDist *= playerDist2;
                    yDist *= playerDist2;
                }
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + conflict2;
                    if (conflict2 > 0.05f && Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + conflict2;
                    }
                }
                if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - conflict2;
                    if (conflict2 > 0.05f && Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - conflict2;
                    }
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + conflict2;
                    if (conflict2> 0.05f && Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + conflict2 * 2f;
                    }
                }
                if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - conflict2;
                    if (conflict2 > 0.05f && Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - conflict2 * 2f;
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
                float conflict3 = (float) (40 * Projectile.minionPos);
                float attackCooldown = 30f;
                int num4 = 60;
                --Projectile.localAI[0];
                if (Projectile.localAI[0] < 0f)
                    Projectile.localAI[0] = 0f;
                if (Projectile.ai[1] > 0f)
                {
                    --Projectile.ai[1];
                }
                else
                {
                    float num5 = Projectile.position.X;
                    float num6 = Projectile.position.Y;
                    float num7 = 100000f;
                    float num8 = num7;
                    int num9 = -1;
                    NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                    if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy((object) Projectile, false))
                    {
                        float num10 = minionAttackTargetNpc.position.X + (float) (minionAttackTargetNpc.width / 2);
                        float num11 = minionAttackTargetNpc.position.Y + (float) (minionAttackTargetNpc.height / 2);
                        float num12 = Math.Abs(Projectile.position.X + (float) (Projectile.width / 2) - num10) + Math.Abs(Projectile.position.Y + (float) (Projectile.height / 2) - num11);
                        if (num12 < num7)
                        {
                            if (num9 == -1 && num12 <= num8)
                            {
                                num8 = num12;
                                num5 = num10;
                                num6 = num11;
                            }
                            if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, minionAttackTargetNpc.position, minionAttackTargetNpc.width, minionAttackTargetNpc.height))
                            {
                                num7 = num12;
                                num5 = num10;
                                num6 = num11;
                                num9 = minionAttackTargetNpc.whoAmI;
                            }
                        }
                    }
                    if (num9 == -1)
                    {
                        for (int index = 0; index < Main.maxNPCs; ++index)
                        {
                            if (Main.npc[index].CanBeChasedBy((object) Projectile, false))
                            {
                                float num10 = Main.npc[index].position.X + (float) (Main.npc[index].width / 2);
                                float num11 = Main.npc[index].position.Y + (float) (Main.npc[index].height / 2);
                                float num12 = Math.Abs(Projectile.position.X + (float) (Projectile.width / 2) - num10) + Math.Abs(Projectile.position.Y + (float) (Projectile.height / 2) - num11);
                                if (num12 < num7)
                                {
                                    if (num9 == -1 && num12 <= num8)
                                    {
                                        num8 = num12;
                                        num5 = num10;
                                        num6 = num11;
                                    }
                                    if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[index].position, Main.npc[index].width, Main.npc[index].height))
                                    {
                                        num7 = num12;
                                        num5 = num10;
                                        num6 = num11;
                                        num9 = index;
                                    }
                                }
                            }
                        }
                    }
                    if (num9 == -1 && num8 < num7)
                        num7 = num8;
                    float num13 = 400f;
                    if ((double) Projectile.position.Y > Main.worldSurface * 16.0)
                        num13 = 200f;
                    if (num7 < num13 + conflict3 && num9 == -1)
                    {
                        float num10 = num5 - (Projectile.position.X + (float) (Projectile.width / 2));
                        if (num10 < -5f)
                        {
                            flag1 = true;
                            flag2 = false;
                        }
                        else if (num10 > 5f)
                        {
                            flag2 = true;
                            flag1 = false;
                        }
                    }
                    else if (num9 >= 0 && num7 < 800f + conflict3)
                    {
                        Projectile.localAI[0] = (float) num4;
                        float num10 = num5 - (Projectile.position.X + (float) (Projectile.width / 2));
                        if (num10 > 300f || num10 < -300f)
                        {
                            if (num10 < -50f)
                            {
                                flag1 = true;
                                flag2 = false;
                            }
                            else if (num10 > 50f)
                            {
                                flag2 = true;
                                flag1 = false;
                            }
                        }
                        else if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.ai[1] = attackCooldown;
                            double num11 = 12.0;
                            Vector2 vector2 = new Vector2(Projectile.Center.X, Projectile.Center.Y - 8f);
                            float num12 = num5 - vector2.X + Main.rand.NextFloat(-6f, 6f);
                            float num14 = (float) ((double) (Math.Abs(num12) * 0.1f) * (double) Main.rand.Next(0, 100) * (1.0 / 1000.0));
                            float num15 = num6 - vector2.Y + Main.rand.NextFloat(-6f, 6f) - num14;
                            double num16 = Math.Sqrt((double) num12 * (double) num12 + (double) num15 * (double) num15);
                            float num17 = (float) (num11 / num16);
                            float SpeedX = num12 * num17;
                            float SpeedY = num15 * num17;
                            int damage = Projectile.damage;
                            int Type = ModContent.ProjectileType<PlateProjectile>();
                            int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector2.X, vector2.Y, SpeedX * 2f, SpeedY * 2f, Type, damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                            if (SpeedX < 0f)
                                Projectile.direction = -1;
                            if (SpeedX > 0f)
                                Projectile.direction = 1;
                            if (Main.projectile.IndexInRange(index))
                                Main.projectile[index].originalDamage = Projectile.originalDamage;
                            Projectile.netUpdate = true;
                        }
                    }
                }
                Vector2 vector2_1 = Vector2.Zero;
                bool flag7 = false;
                if (Projectile.ai[1] != 0f)
                {
                    flag1 = false;
                    flag2 = false;
                }
                else if (Projectile.localAI[0] == 0f)
                    Projectile.direction = player.direction;
                if (!flag7)
                    Projectile.rotation = 0f;
                Projectile.tileCollide = true;
                float num18 = 0.08f;
                float num19 = 6.5f;
                num19 = 6f;
                num18 = 0.2f;
                if (num19 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                {
                    num19 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    num18 = 0.3f;
                }
                if (flag1)
                {
                    if (Projectile.velocity.X > -3.5f)
                        Projectile.velocity.X -= num18;
                    else
                        Projectile.velocity.X -= num18 * 0.25f;
                }
                else if (flag2)
                {
                    if (Projectile.velocity.X < 3.5f)
                        Projectile.velocity.X += num18;
                    else
                        Projectile.velocity.X += num18 * 0.25f;
                }
                else
                {
                    Projectile.velocity.X *= 0.9f;
                    if (Projectile.velocity.X >= -num18 && Projectile.velocity.X <= num18)
                        Projectile.velocity.X = 0f;
                }
                if (flag1 | flag2)
                {
                    int conflict4 = (int) ((double) Projectile.position.X + (double) (Projectile.width / 2)) / 16;
                    int j = (int) ((double) Projectile.position.Y + (double) (Projectile.height / 2)) / 16;
                    if (flag1)
                        --conflict4;
                    if (flag2)
                        ++conflict4;
                    if (WorldGen.SolidTile(conflict4 + (int) Projectile.velocity.X, j))
                        flag4 = true;
                }
                if (player.position.Y + player.height - 8f > Projectile.position.Y + Projectile.height)
                    flag3 = true;
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY, 1, false, 0);
                if (Projectile.velocity.Y == 0f)
                {
                    if (!flag3 && ((double) Projectile.velocity.X < 0.0 || (double) Projectile.velocity.X > 0.0))
                    {
                        int i = (int) ((double) Projectile.position.X + (double) (Projectile.width / 2)) / 16;
                        int j = (int) ((double) Projectile.position.Y + (double) (Projectile.height / 2)) / 16 + 1;
                        if (flag1)
                            --i;
                        if (flag2)
                            ++i;
                        WorldGen.SolidTile(i, j);
                    }
                    if (flag4)
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
                                    int conflict5 = (int) ((double) Projectile.position.X + (double) (Projectile.width / 2)) / 16;
                                    int conflict7 = (int) ((double) Projectile.position.Y + (double) (Projectile.height / 2)) / 16;
                                    if (flag1)
                                        --conflict5;
                                    if (flag2)
                                        ++conflict5;
                                    int i2 = conflict5 + (int) Projectile.velocity.X;
                                    if (!WorldGen.SolidTile(i2, conflict7 - 1) && !WorldGen.SolidTile(i2, conflict7 - 2))
                                        Projectile.velocity.Y = -5.1f;
                                    else if (!WorldGen.SolidTile(i2, conflict7 - 2))
                                        Projectile.velocity.Y = -7.1f;
                                    else if (WorldGen.SolidTile(i2, conflict7 - 5))
                                        Projectile.velocity.Y = -11.1f;
                                    else if (WorldGen.SolidTile(i2, conflict7 - 4))
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
                    else if (Projectile.type == 266 && flag1 | flag2)
                        Projectile.velocity.Y -= 6f;
                }
                if (Projectile.velocity.X > num19)
                    Projectile.velocity.X = num19;
                if (Projectile.velocity.X < -num19)
                    Projectile.velocity.X = -num19;
                if (Projectile.velocity.X < 0f)
                    Projectile.direction = -1;
                if (Projectile.velocity.X > 0f)
                    Projectile.direction = 1;
                if (Projectile.velocity.X > num18 & flag2)
                    Projectile.direction = 1;
                if (Projectile.velocity.X < -num18 & flag1)
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

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                int index = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X - (float)(Projectile.width / 2), Projectile.position.Y - (float)(Projectile.height / 2)), new Vector2(0.0f, 0.0f), Main.rand.Next(61, 64), Projectile.scale);
                Main.gore[index].velocity *= 0.1f;
            }
        }
    }
}
