using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        public static void ChargingMinionAI(this Projectile projectile, float range, float maxPlayerDist, float extraMaxPlayerDist, float safeDist, int initialUpdates, float chargeDelayTime, float goToSpeed, float goBackSpeed, Vector2 returnOffset, float chargeCounterMax, float chargeSpeed, bool tileVision, bool ignoreTilesWhenCharging, int updateDifference = 1)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //Anti sticky movement to prevent stacking
            projectile.MinionAntiClump();

            //Breather time between charges as like a reset
            bool chargeDelay = false;
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1] += 1f;
                projectile.extraUpdates = initialUpdates + updateDifference;
                if (projectile.ai[1] > chargeDelayTime)
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.extraUpdates = initialUpdates;
                    projectile.numUpdates = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    chargeDelay = true;
                }
            }
            if (chargeDelay)
            {
                return;
            }

            //Find a target
            float maxDist = range;
            Vector2 targetVec = projectile.position;
            bool foundTarget = false;
            bool isButterfly = projectile.type == ProjectileType<PurpleButterfly>();
            //Prioritize the targeted enemy if possible
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                bool fishronCheck = npc.type == NPCID.DukeFishron && npc.active && isButterfly;
                if (npc.CanBeChasedBy(projectile, false) || fishronCheck)
                {
                    //Check the size of the target to make it easier to hit fat targets like Levi
                    float extraDist = (npc.width / 2) + (npc.height / 2);

                    float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                    //Some minions will ignore tiles when choosing a target like Ice Claspers, others will not
                    bool canHit = true;
                    if (extraDist < maxDist && !tileVision)
                        canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
                    if (!foundTarget && targetDist < (maxDist + extraDist) && canHit)
                    {
                        maxDist = targetDist;
                        targetVec = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            //If no npc is specifically targetted or the selected enemy can't be found, check through the entire array
            if (!foundTarget)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    bool fishronCheck = npc.type == NPCID.DukeFishron && npc.active && isButterfly;
                    if (npc.CanBeChasedBy(projectile, false) || fishronCheck)
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                        bool canHit = true;
                        if (extraDist < maxDist && !tileVision)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
                        if (!foundTarget && targetDist < (maxDist + extraDist) && canHit)
                        {
                            maxDist = targetDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            //If the player is too far, return to the player. Range is increased while attacking something.
            float distBeforeForcedReturn = maxPlayerDist;
            if (foundTarget)
            {
                distBeforeForcedReturn = extraMaxPlayerDist;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > distBeforeForcedReturn)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }

            //Go to the target if you found one
            if (foundTarget && projectile.ai[0] == 0f)
            {
                //Some minions don't ignore tiles while charging like brittle stars
                projectile.tileCollide = !ignoreTilesWhenCharging;
                Vector2 targetSpot = targetVec - projectile.Center;
                float targetDist = targetSpot.Length();
                targetSpot.Normalize();
                //Tries to get the minion in the sweet spot of 200 pixels away but the minion also charges so idk what good it does
                if (targetDist > 200f)
                {
                    float speed = goToSpeed; //8
                    targetSpot *= speed;
                    projectile.velocity = (projectile.velocity * 40f + targetSpot) / 41f;
                }
                else
                {
                    float speed = -goBackSpeed; //-4
                    targetSpot *= speed;
                    projectile.velocity = (projectile.velocity * 40f + targetSpot) / 41f; //41
                }
            }

            //Movement for idle or returning to the player
            else
            {
                //Ignore tiles so they don't get stuck everywhere like Optic Staff
                projectile.tileCollide = false;

                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = projectile.ai[0] == 1f;
                }

                //Player distance calculations
                Vector2 playerVec = player.Center - projectile.Center + returnOffset;
                float playerDist = playerVec.Length();

                //If the minion is actively returning, move faster
                float playerHomeSpeed = 6f;
                if (returningToPlayer)
                {
                    playerHomeSpeed = 15f;
                }
                //Move somewhat faster if the player is kinda far~ish
                if (playerDist > 200f && playerHomeSpeed < 8f)
                {
                    playerHomeSpeed = 8f;
                }
                //Return to normal if close enough to the player
                if (playerDist < safeDist && returningToPlayer && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                //Teleport to the player if abnormally far
                if (playerDist > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                //If more than 70 pixels away, move toward the player
                if (playerDist > 70f)
                {
                    playerVec.Normalize();
                    playerVec *= playerHomeSpeed;
                    projectile.velocity = (projectile.velocity * 40f + playerVec) / 41f;
                }
                //Minions never stay still
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }

            //Increment attack counter randomly
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            //If high enough, prepare to attack
            if (projectile.ai[1] > chargeCounterMax)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }

            //Charge at an enemy if not on cooldown
            if (projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f && foundTarget && maxDist < 500f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.ai[0] = 2f;
                        Vector2 targetPos = targetVec - projectile.Center;
                        targetPos.Normalize();
                        projectile.velocity = targetPos * chargeSpeed; //8
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public static void FloatingPetAI(this Projectile projectile, bool faceRight, float tiltFloat, bool lightPet = false)
        {
            Player player = Main.player[projectile.owner];

            //anti sticking movement as a failsafe
            float SAImovement = 0.05f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible
                if (!otherProj.active || otherProj.owner != projectile.owner || !Main.projPet[otherProj.type] || k == projectile.whoAmI)
                    continue;

                // If the other projectile is indeed another pet owned by the same player and they're too close, nudge them away.
                bool isPet = Main.projPet[otherProj.type];
                float taxicabDist = Math.Abs(projectile.position.X - otherProj.position.X) + Math.Abs(projectile.position.Y - otherProj.position.Y);
                if (isPet && taxicabDist < projectile.width)
                {
                    if (projectile.position.X < otherProj.position.X)
                        projectile.velocity.X -= SAImovement;
                    else
                        projectile.velocity.X += SAImovement;

                    if (projectile.position.Y < otherProj.position.Y)
                        projectile.velocity.Y -= SAImovement;
                    else
                        projectile.velocity.Y += SAImovement;
                }
            }

            float passiveMvtFloat = 0.5f;
            projectile.tileCollide = false;
            float range = 100f;
            Vector2 projPos = projectile.Center;
            float xDist = player.Center.X - projPos.X;
            float yDist = player.Center.Y - projPos.Y;
            yDist += Main.rand.NextFloat(-10, 20);
            xDist += Main.rand.NextFloat(-10, 20);
            //Light pets lead the player, normal pets trail the player
            xDist += 60f * (lightPet ? (float)player.direction : -(float)player.direction);
            yDist -= 60f;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 18f;

            //If player is close enough, resume normal
            if (playerDist < range && player.velocity.Y == 0f &&
                projectile.Bottom.Y <= player.Bottom.Y &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }

            //Teleport to player if too far
            if (playerDist > 2000f)
            {
                projectile.position.X = player.Center.X - projectile.width / 2;
                projectile.position.Y = player.Center.Y - projectile.height / 2;
                projectile.netUpdate = true;
            }

            if (playerDist < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.99f;
                }
                passiveMvtFloat = 0.01f;
            }
            else
            {
                if (playerDist < 100f)
                {
                    passiveMvtFloat = 0.1f;
                }
                if (playerDist > 300f)
                {
                    passiveMvtFloat = 1f;
                }
                playerDist = returnSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
            }
            if (projectile.velocity.X < playerVector.X)
            {
                projectile.velocity.X += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X += passiveMvtFloat;
                }
            }
            if (projectile.velocity.X > playerVector.X)
            {
                projectile.velocity.X -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X -= passiveMvtFloat;
                }
            }
            if (projectile.velocity.Y < playerVector.Y)
            {
                projectile.velocity.Y += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y += passiveMvtFloat * 2f;
                }
            }
            if (projectile.velocity.Y > playerVector.Y)
            {
                projectile.velocity.Y -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y -= passiveMvtFloat * 2f;
                }
            }
            if (projectile.velocity.X >= 0.25f)
            {
                projectile.direction = faceRight ? 1 : -1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.direction = faceRight ? -1 : 1;
            }
            //Tilting and change directions
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.X * tiltFloat;
        }

        public static void HealingProjectile(this Projectile projectile, int healing, int playerToHeal, float homingVelocity, float N, bool autoHomes = true, int timeCheck = 120)
        {
            int target = playerToHeal;
            Player player = Main.player[target];
            float homingSpeed = homingVelocity;
            if (player.lifeMagnet)
                homingSpeed *= 1.5f;

            Vector2 playerVector = player.Center - projectile.Center;
            float playerDist = playerVector.Length();
            if (playerDist < 50f && projectile.position.X < player.position.X + player.width && projectile.position.X + projectile.width > player.position.X && projectile.position.Y < player.position.Y + player.height && projectile.position.Y + projectile.height > player.position.Y)
            {
                if (projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech)
                {
                    int healAmt = healing;
                    player.HealEffect(healAmt, false);
                    player.statLife += healAmt;
                    if (player.statLife > player.statLifeMax2)
                    {
                        player.statLife = player.statLifeMax2;
                    }
                    NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, target, healAmt, 0f, 0f, 0, 0, 0);
                }
                projectile.Kill();
            }
            if (autoHomes)
            {
                playerDist = homingSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                projectile.velocity.X = (projectile.velocity.X * N + playerVector.X) / (N + 1f);
                projectile.velocity.Y = (projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
            }
            else if (player.lifeMagnet && projectile.timeLeft < timeCheck)
            {
                playerDist = homingVelocity / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                projectile.velocity.X = (projectile.velocity.X * N + playerVector.X) / (N + 1f);
                projectile.velocity.Y = (projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
            }
        }

        /// <summary>
        /// Call this function in the ai of your projectile so it can stick to enemies, also requires ModifyHitNPCSticky to be called in ModifyHitNPC
        /// </summary>
        /// <param name="projectile">The projectile you're adding sticky behaviour to</param>
        /// <param name="timeLeft">Number of seconds you want a projectile to cling to an NPC</param>
        public static void StickyProjAI(this Projectile projectile, int timeLeft, bool findNewNPC = false)
        {
            if (projectile.ai[0] == 1f)
            {
                int seconds = timeLeft;
                bool killProj = false;
                bool spawnDust = false;

                //the projectile follows the NPC, even if it goes into blocks
                projectile.tileCollide = false;

                //timer for triggering hit effects
                projectile.localAI[0]++;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    spawnDust = true;
                }

                //So AI knows what NPC it is sticking to
                int npcIndex = (int)projectile.ai[1];
                NPC npc = Main.npc[npcIndex];

                //Kill projectile after so many seconds or if the NPC it is stuck to no longer exists
                if (projectile.localAI[0] >= (float)(60 * seconds))
                {
                    killProj = true;
                }
                else if (!npcIndex.WithinBounds(Main.maxNPCs))
                {
                    killProj = true;
                }

                else if (npc.active && !npc.dontTakeDamage)
                {
                    //follow the NPC
                    projectile.Center = npc.Center - projectile.velocity * 2f;
                    projectile.gfxOffY = npc.gfxOffY;

                    //if attached to npc, trigger npc hit effects every half a second
                    if (spawnDust)
                    {
                        npc.HitEffect(0, 1.0);
                    }
                }
                else
                {
                    killProj = true;
                }

                //Kill the projectile or reset stats if needed
                if (killProj)
                {
                    if (findNewNPC)
                        projectile.ai[0] = 0f;
                    else
                        projectile.Kill();
                }
            }
        }

        /// <summary>
        /// Call this function in ModifyHitNPC to make your projectiles stick to enemies, needs StickyProjAI to be called in the AI of the projectile
        /// </summary>
        /// <param name="projectile">The projectile you're giving sticky behaviour to</param>
        /// <param name="maxStick">How many projectiles of this type can stick to one enemy</param>
        public static void ModifyHitNPCSticky(this Projectile projectile, int maxStick)
        {
            Player player = Main.player[projectile.owner];
            Rectangle myRect = projectile.Hitbox;

            if (projectile.owner == Main.myPlayer)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    //covers most edge cases like voodoo dolls
                    if (npc.active && !npc.dontTakeDamage &&
                        ((projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && projectile.owner < Main.maxPlayers && player.killClothier))) ||
                        (projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !projectile.ownerHitCheck)
                        {
                            bool stickingToNPC;
                            //Solar Crawltipede tail has special collision
                            if (npc.type == NPCID.SolarCrawltipedeTail)
                            {
                                Rectangle rect = npc.Hitbox;
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                stickingToNPC = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                stickingToNPC = projectile.Colliding(myRect, npc.Hitbox);
                            }
                            if (stickingToNPC)
                            {
                                //reflect projectile if the npc can reflect it (like Selenians)
                                if (npc.reflectsProjectiles && projectile.CanBeReflected())
                                {
                                    npc.ReflectProjectile(projectile);
                                    return;
                                }

                                //let the projectile know it is sticking and the npc it is sticking too
                                projectile.ai[0] = 1f;
                                projectile.ai[1] = (float)npcIndex;

                                //follow the NPC
                                projectile.velocity = (npc.Center - projectile.Center) * 0.75f;

                                projectile.netUpdate = true;

                                //Count how many projectiles are attached, delete as necessary
                                Point[] array2 = new Point[maxStick];
                                int projCount = 0;
                                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                                {
                                    Projectile proj = Main.projectile[projIndex];
                                    if (projIndex != projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == projectile.type && proj.ai[0] == 1f && proj.ai[1] == (float)npcIndex)
                                    {
                                        array2[projCount++] = new Point(projIndex, proj.timeLeft);
                                        if (projCount >= array2.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (projCount >= array2.Length)
                                {
                                    int num30 = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[num30].Y)
                                        {
                                            num30 = m;
                                        }
                                    }
                                    Main.projectile[array2[num30].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void StickToTiles(this Projectile projectile, bool ignorePlatforms, bool stickToEverything)
        {
            try
            {
                int xLeft = (int)(projectile.position.X / 16f) - 1;
                int xRight = (int)((projectile.position.X + (float)projectile.width) / 16f) + 2;
                int yBottom = (int)(projectile.position.Y / 16f) - 1;
                int yTop = (int)((projectile.position.Y + (float)projectile.height) / 16f) + 2;
                if (xLeft < 0)
                {
                    xLeft = 0;
                }
                if (xRight > Main.maxTilesX)
                {
                    xRight = Main.maxTilesX;
                }
                if (yBottom < 0)
                {
                    yBottom = 0;
                }
                if (yTop > Main.maxTilesY)
                {
                    yTop = Main.maxTilesY;
                }
                for (int x = xLeft; x < xRight; x++)
                {
                    for (int y = yBottom; y < yTop; y++)
                    {
                        Tile tile = Main.tile[x, y];
                        bool platformCheck = true;
                        if (ignorePlatforms)
                            platformCheck = !TileID.Sets.Platforms[tile.TileType] && tile.TileType != TileID.PlanterBox;
                        bool tableCheck = false;
                        if (stickToEverything)
                            tableCheck = Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0;
                        if (tile != null && tile.HasUnactuatedTile && platformCheck && (Main.tileSolid[tile.TileType] || tableCheck))
                        {
                            Vector2 tileSize;
                            tileSize.X = (float)(x * 16);
                            tileSize.Y = (float)(y * 16);
                            if (projectile.position.X + (float)projectile.width - 4f > tileSize.X && projectile.position.X + 4f < tileSize.X + 16f && projectile.position.Y + (float)projectile.height - 4f > tileSize.Y && projectile.position.Y + 4f < tileSize.Y + 16f)
                            {
                                projectile.velocity.X = 0f;
                                projectile.velocity.Y = -0.2f;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
