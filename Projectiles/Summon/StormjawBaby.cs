using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class StormjawBaby : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public float dust = 0f;
        private int sparkCounter = 0;
        private int targetIndex = -1;
        private Vector2 idlePos = Vector2.Zero;
        public Player player => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 38;
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
            OnSpawn();
            SummonChecks();

            idlePos = player.Center;
            idlePos.X -= (15f + player.width / 2f) * player.direction;
            idlePos.X -= Projectile.minionPos * 40f * player.direction;

            FindTarget();
            FlyBackToPlayer();
            AttackTarget();
            GoToTarget();
            IdleBehavior();
        }

        private void OnSpawn()
        {
            // Handle stuff on spawn & variable damage
            if (dust == 0f)
            {
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;
                    int spark = Dust.NewDust(source + dustVel, 0, 0, 132, dustVel.X * 1.1f, dustVel.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[spark].noGravity = true;
                    Main.dust[spark].noLight = true;
                    Main.dust[spark].velocity = dustVel;
                }
                dust += 1f;
            }
        }

        private void SummonChecks()
        {
            bool projTypeCheck = Projectile.type == ModContent.ProjectileType<StormjawBaby>();
            player.AddBuff(ModContent.BuffType<BabyStormlionBuff>(), 3600);
            if (projTypeCheck)
            {
                if (player.dead)
                {
                    player.Calamity().stormjaw = false;
                }
                if (player.Calamity().stormjaw)
                {
                    Projectile.timeLeft = 2;
                }
            }
        }

        private void FindTarget()
        {
            targetIndex = -1;
            float maxDistance = 800f;
            if (Projectile.ai[0] == 0f) //find target
            {
                NPC targetedNPC = Projectile.OwnerMinionAttackTargetNPC;
                if (targetedNPC != null && targetedNPC.CanBeChasedBy(Projectile, false))
                {
                    float num1 = (targetedNPC.Center - Projectile.Center).Length();
                    if (num1 < maxDistance)
                    {
                        targetIndex = targetedNPC.whoAmI;
                        maxDistance = num1;
                    }
                }
                if (targetIndex < 0)
                {
                    for (int i = 0; i < Main.maxNPCs; ++i)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float num1 = (npc.Center - Projectile.Center).Length();
                            if (num1 < maxDistance)
                            {
                                targetIndex = i;
                                maxDistance = num1;
                            }
                        }
                    }
                }
            }
        }

        private void FlyBackToPlayer()
        {
            if (Projectile.ai[0] == 1f) //returning to player
            {
                Projectile.tileCollide = false;
                Vector2 returnPos = player.Center - Projectile.Center;
                returnPos.X -= (float) (40 * player.direction);
                returnPos.X -= (float) (40 * Projectile.minionPos * player.direction);
                returnPos.Y -= 60f;
                float playerDist = returnPos.Length();
                float returnSpeed = 12f;
                float acceleration = 0.4f;
                if (returnSpeed < Projectile.velocity.Length())
                    returnSpeed = Projectile.velocity.Length();

                //if close enough to the player and has tile to stand on, return to normal
                if (playerDist < 100f && player.velocity.Y == 0f && (Projectile.Bottom.Y <= player.Bottom.Y && !Collision.SolidCollision(Projectile.Center, Projectile.width, Projectile.height)))
                {
                    Projectile.ai[0] = 0f;
                    if (Projectile.velocity.Y < -6f)
                        Projectile.velocity.Y = -6f;
                }
                if (playerDist > 2000f)
                {
                    Projectile.position = player.Center - Projectile.Size / 2f;
                    Projectile.netUpdate = true;
                }
                if (playerDist < 50f)
                {
                    if (Projectile.velocity.Length() > 2f)
                    {
                        Projectile.velocity *= 0.99f;
                    }
                    acceleration = 0.01f;
                }
                else
                {
                    if (playerDist < 100f)
                    {
                        acceleration = 0.1f;
                    }
                    if (playerDist > 300f)
                    {
                        acceleration = 1f;
                    }
                    playerDist = returnSpeed / playerDist;
                    returnPos *= playerDist;
                }
                if (Projectile.velocity.X < returnPos.X)
                {
                    Projectile.velocity.X += acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X += acceleration;
                    }
                }
                if (Projectile.velocity.X > returnPos.X)
                {
                    Projectile.velocity.X -= acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X -= acceleration;
                    }
                }
                if (Projectile.velocity.Y < returnPos.Y)
                {
                    Projectile.velocity.Y += acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y += acceleration * 2f;
                    }
                }
                if (Projectile.velocity.Y > returnPos.Y)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y -= acceleration * 2f;
                    }
                }
                if (Projectile.frame < 6 || Projectile.frame > 9)
                {
                    Projectile.frame = 6;
                }
                else
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 3)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 10)
                    {
                        Projectile.frame = 6;
                    }
                }
                if (Projectile.velocity.X > 0.5f)
                    Projectile.spriteDirection = 1;
                else if (Projectile.velocity.X < -0.5f)
                    Projectile.spriteDirection = -1;
                Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.spriteDirection != 1 ? MathHelper.Pi : 0f;
            }
        }

        private void AttackTarget()
        {
            if (Projectile.ai[0] == 2f) //attack target
            {
                Projectile.spriteDirection = -Projectile.direction;
                Projectile.rotation = 0f;
                if (Projectile.velocity.Y == 0f)
                {
                    if (Projectile.velocity.X == 0f)
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                    else if (Math.Abs(Projectile.velocity.X) >= 0.5f)
                    {
                        Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X);
                        Projectile.frameCounter += 1;
                        if (Projectile.frameCounter > 10)
                        {
                            Projectile.frame += 1;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame >= 6)
                            Projectile.frame = 0;
                    }
                    else
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                }
                else if (Projectile.velocity.Y != 0f)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 0;
                }
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;

                sparkCounter += Main.rand.Next(1,4);
                if (sparkCounter >= 20)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < Main.rand.Next(1,4); i++)
                        {
                            Vector2 sparkS = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                            int spark = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkS, ModContent.ProjectileType<Spark>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            if (spark.WithinBounds(Main.maxProjectiles))
                            {
                                Main.projectile[spark].DamageType = DamageClass.Summon;
                                Main.projectile[spark].originalDamage = Projectile.originalDamage;
                                Main.projectile[spark].timeLeft = 120;
                                Main.projectile[spark].penetrate = 3;
                                Main.projectile[spark].usesIDStaticNPCImmunity = true;
                                Main.projectile[spark].idStaticNPCHitCooldown = 10;
                                Main.projectile[spark].usesLocalNPCImmunity = false;
                            }
                        }
                        sparkCounter = 0;
                    }
                }

                Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] <= 0f)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            else
            {
                Rectangle rectangle = new Rectangle((int)(Projectile.position.X + Projectile.velocity.X * 0.5f - 4f), (int)(Projectile.position.Y + Projectile.velocity.Y * 0.5f - 4f), Projectile.width + 8, Projectile.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile, false) && npc.immune[Projectile.owner] <= 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            sparkCounter += Main.rand.Next(1,3);
                            if (sparkCounter >= 20)
                            {
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    for (int j = 0; j < Main.rand.Next(1,4); j++)
                                    {
                                        Vector2 sparkS = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                                        int spark = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkS, ModContent.ProjectileType<Spark>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                        if (spark.WithinBounds(Main.maxProjectiles))
                                        {
                                            Main.projectile[spark].DamageType = DamageClass.Summon;
                                            Main.projectile[spark].originalDamage = Projectile.originalDamage;
                                            Main.projectile[spark].timeLeft = 120;
                                            Main.projectile[spark].penetrate = 3;
                                            Main.projectile[spark].usesIDStaticNPCImmunity = true;
                                            Main.projectile[spark].idStaticNPCHitCooldown = 10;
                                            Main.projectile[spark].usesLocalNPCImmunity = false;
                                        }
                                    }
                                    sparkCounter = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GoToTarget()
        {
            if (targetIndex >= 0) //go to target
            {
                float rangeofSight = 700f;
                float attackZone = 20f;
                if (Projectile.position.Y > Main.worldSurface * 16f)
                    rangeofSight *= 0.7f;
                NPC npc = Main.npc[targetIndex];
                float targetDist = (npc.Center - Projectile.Center).Length();
                Collision.CanHit(Projectile.Center, Projectile.width, Projectile.height, npc.Center, npc.width, npc.height);
                if (targetDist < rangeofSight)
                {
                    idlePos = npc.Center;
                    if (npc.Center.Y < Projectile.Center.Y - 30f && Projectile.velocity.Y == 0f)
                    {
                        float targetYDist = Math.Abs(npc.Center.Y - Projectile.Center.Y);
                        if (targetYDist < 120f)
                            Projectile.velocity.Y = -10f;
                        else if (targetYDist < 210f)
                            Projectile.velocity.Y = -13f;
                        else if (targetYDist < 270f)
                            Projectile.velocity.Y = -15f;
                        else if (targetYDist < 310f)
                            Projectile.velocity.Y = -17f;
                        else if (targetYDist < 380f)
                            Projectile.velocity.Y = -18f;
                    }
                }
                if (targetDist < attackZone)
                {
                    Projectile.ai[0] = 2f;
                    Projectile.ai[1] = 15f;
                    Projectile.netUpdate = true;
                }
            }
        }

        private void IdleBehavior()
        {
            if (Projectile.ai[0] == 0f && targetIndex < 0) //passive AI
            {
                if (sparkCounter > 0)
                    sparkCounter--;
                if (sparkCounter < 0)
                    sparkCounter = 0;

                float sepAnxietyDist = 1000f;
                Vector2 playerDist = player.Center - Projectile.Center;
                // Teleport to the player if too far
                if (playerDist.Length() > 2000f)
                    Projectile.position = player.Center - Projectile.Size / 2f;
                // Fly back if too far from the player
                else if (playerDist.Length() > sepAnxietyDist || Math.Abs(playerDist.Y) > 300f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                    if (Projectile.velocity.Y > 0f && playerDist.Y < 0f)
                        Projectile.velocity.Y = 0f;
                    if (Projectile.velocity.Y < 0f && playerDist.Y > 0f)
                        Projectile.velocity.Y = 0f;
                }
            }

            if (Projectile.ai[0] == 0f)
            {
                Projectile.tileCollide = true;
                float accelFast = 1f;
                float maxSpeed = 8f;
                float xVel = 8f;
                float accelSlow = 0.2f;
                if (xVel < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                {
                    xVel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    accelFast = 1.4f;
                }
                int direction = 0;
                bool flag3 = false;
                float idleDist = idlePos.X - Projectile.Center.X;
                if (Math.Abs(idleDist) > 5f)
                {
                    if (idleDist < 0f)
                    {
                        direction = -1;
                        if (Projectile.velocity.X > -maxSpeed)
                        {
                            Projectile.velocity.X -= accelFast;
                        }
                        else
                        {
                            Projectile.velocity.X -= accelSlow;
                        }
                    }
                    else
                    {
                        direction = 1;
                        if (Projectile.velocity.X < maxSpeed)
                        {
                            Projectile.velocity.X += accelFast;
                        }
                        else
                        {
                            Projectile.velocity.X += accelSlow;
                        }
                    }
                }
                else
                {
                    Projectile.velocity.X *= 0.9f;
                    if (Math.Abs(Projectile.velocity.X) < accelFast * 2f)
                        Projectile.velocity.X = 0.0f;
                }
                if (direction != 0)
                {
                    int xPos = (int)Projectile.Center.X / 16;
                    int yPos = (int)Projectile.position.Y / 16;
                    int x = xPos + direction + (int) Projectile.velocity.X;
                    for (int y = yPos; y < yPos + Projectile.height / 16 + 1; ++y)
                    {
                        if (WorldGen.InWorld(x, y) && WorldGen.SolidTile(x, y))
                            flag3 = true;
                    }
                }
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY, 1, false, 0);
                if (Projectile.velocity.Y == 0f && flag3)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        int x = (int)Projectile.Center.X / 16;
                        if (i == 0)
                            x = (int)Projectile.Left.X / 16;
                        if (i == 2)
                            x = (int)Projectile.Right.X / 16;
                        int y = (int)Projectile.Bottom.Y / 16;
                        Tile tile = Main.tile[x, y];
                        if ((WorldGen.InWorld(x, y) && WorldGen.SolidTile(x, y)) || tile.IsHalfBlock || tile.Slope > 0 || TileID.Sets.Platforms[tile.TileType] && tile.HasTile && !tile.HasActuator)
                        {
                            try
                            {
                                int xPos = (int)Projectile.Center.X / 16;
                                int yPos = (int)Projectile.Center.Y / 16;
                                int i2 = xPos + direction + (int)Projectile.velocity.X;
                                if (!WorldGen.SolidTile(i2, yPos - 1) && !WorldGen.SolidTile(i2, yPos - 2))
                                    Projectile.velocity.Y = -5.1f;
                                else if (!WorldGen.SolidTile(i2, yPos - 2))
                                    Projectile.velocity.Y = -7.1f;
                                else if (WorldGen.SolidTile(i2, yPos - 5))
                                    Projectile.velocity.Y = -11.1f;
                                else if (WorldGen.SolidTile(i2, yPos - 4))
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
                if (Projectile.velocity.X > xVel)
                    Projectile.velocity.X = xVel;
                if (Projectile.velocity.X < -xVel)
                    Projectile.velocity.X = -xVel;
                if (Projectile.velocity.X < 0f)
                    Projectile.direction = -1;
                if (Projectile.velocity.X > 0f)
                    Projectile.direction = 1;
                if (Projectile.velocity.X > accelFast && direction == 1)
                    Projectile.direction = 1;
                if (Projectile.velocity.X < -accelFast && direction == -1)
                    Projectile.direction = -1;
                Projectile.spriteDirection = -Projectile.direction;
                Projectile.rotation = 0f;
                if (Projectile.velocity.Y == 0f)
                {
                    if (Projectile.velocity.X == 0f)
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                        float xDist = player.Center.X - Projectile.Center.X;
                        xDist -= 40f * player.direction;
                        xDist -= 40f * Projectile.minionPos * player.direction;
                        if (xDist > 0)
                            Projectile.spriteDirection = Projectile.direction;
                    }
                    else if (Math.Abs(Projectile.velocity.X) >= 0.5f)
                    {
                        Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X);
                        Projectile.frameCounter += 1;
                        if (Projectile.frameCounter > 10)
                        {
                            Projectile.frame += 1;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame >= 6)
                            Projectile.frame = 0;
                    }
                    else
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                }
                else if (Projectile.velocity.Y != 0f)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 0;
                }
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;
            }            
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (targetIndex < 0)
                fallThrough = Projectile.Bottom.Y < player.Top.Y;
            else
                fallThrough = Projectile.Bottom.Y < Main.npc[targetIndex].Top.Y;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                int index = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(0f, 0f), Main.rand.Next(61, 64), Projectile.scale);
                Main.gore[index].velocity *= 0.1f;
            }
        }

        public override bool? CanDamage() => false;
    }
}
