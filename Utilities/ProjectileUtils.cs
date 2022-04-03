using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        public static int CountProjectiles(int Type) => Main.projectile.Count(proj => proj.type == Type && proj.active);

        public static int CountHookProj() => Main.projectile.Count(proj => Main.projHook[proj.type] && proj.ai[0] == 2f && proj.active && proj.owner == Main.myPlayer);

        public static bool FinalExtraUpdate(this Projectile proj) => proj.numUpdates == -1;

        public static bool IsSummon(this Projectile proj) => proj.minion || proj.sentry || CalamityLists.projectileMinionList.Contains(proj.type) || ProjectileID.Sets.MinionShot[proj.type] || ProjectileID.Sets.SentryShot[proj.type];

        public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
        {
            return projectile.modProjectile as T;
        }

        public static Projectile FindProjectileByIdentity(int identity, int ownerIndex)
        {
            // If in singleplayer, simply return the projectile at the designated index, as singleplayer will never have mismatching indices.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return Main.projectile[identity];

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].identity != identity || Main.projectile[i].owner != ownerIndex || !Main.projectile[i].active)
                    continue;

                return Main.projectile[i];
            }
            return null;
        }

        // NOTE - Do not under any circumstance use these predictive methods for enemies or bosses. It is intended for minions and player-created projectiles.
        // Due to its extremely precise nature, it will have little openings that allow players to react without dashing through the enemy.
        // The results will be neither fun nor fair.

        /// <summary>
        /// Calculates a velocity that approximately predicts where some target will be in the future based on Euler's Method.
        /// </summary>
        /// <param name="startingPosition">The starting position from where the movement is calculated.</param>
        /// <param name="targetPosition">The position of the target to hit.</param>
        /// <param name="targetVelocity">The velocity of the target to hit.</param>
        /// <param name="shootSpeed">The speed of the predictive velocity.</param>
        /// <param name="iterations">The number of iterations to perform. The more iterations, the more precise the results are.</param>
        public static Vector2 CalculatePredictiveAimToTarget(Vector2 startingPosition, Vector2 targetPosition, Vector2 targetVelocity, float shootSpeed, int iterations = 4)
        {
            float previousTimeToReachDestination = 0f;
            Vector2 currentTargetPosition = targetPosition;
            for (int i = 0; i < iterations; i++)
            {
                float timeToReachDestination = Vector2.Distance(startingPosition, currentTargetPosition) / shootSpeed;
                currentTargetPosition += targetVelocity * (timeToReachDestination - previousTimeToReachDestination);
                previousTimeToReachDestination = timeToReachDestination;
            }
            return (currentTargetPosition - startingPosition).SafeNormalize(Vector2.UnitY) * shootSpeed;
        }

        /// <summary>
        /// Calculates a velocity that approximately predicts where some target will be in the future based on Euler's Method.
        /// </summary>
        /// <param name="startingPosition">The starting position from where the movement is calculated.</param>
        /// <param name="target">The target to hit.</param>
        /// <param name="shootSpeed">The speed of the predictive velocity.</param>
        /// <param name="iterations">The number of iterations to perform. The more iterations, the more precise the results are.</param>
        public static Vector2 CalculatePredictiveAimToTarget(Vector2 startingPosition, Entity target, float shootSpeed, int iterations = 4)
        {
            return CalculatePredictiveAimToTarget(startingPosition, target.Center, target.velocity, shootSpeed, iterations);
        }

        /// <summary>
        /// Makes a projectile home in such a way that it attempts to fractionally move towards a target's expected future position.
        /// This is based on the results of the <see cref="CalculatePredictiveAimToTarget"/> method.
        /// </summary>
        /// <param name="projectile">The projectile that should home.</param>
        /// <param name="target">The target.</param>
        /// <param name="inertia">The inertia of the movement change.</param>
        public static Vector2 SuperhomeTowardsTarget(this Projectile projectile, NPC target, float homingSpeed, float inertia)
        {
            Vector2 idealVelocity = CalculatePredictiveAimToTarget(projectile.Center, target, homingSpeed);
            return (projectile.velocity * (inertia - 1f) + idealVelocity) / inertia;
        }

        public static void KillAllHostileProjectiles()
        {
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active && projectile.hostile && !projectile.friendly && projectile.damage > 0)
                {
                    projectile.Kill();
                }
            }
        }

        public static void KillShootProjectiles(bool shouldBreak, int projType, Player player)
        {
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj.active && proj.owner == player.whoAmI && proj.type == projType)
                {
                    proj.Kill();
                    if (shouldBreak)
                        break;
                }
            }
        }

        public static void KillShootProjectileMany(Player player, params int[] projTypes)
        {
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj.active && proj.owner == player.whoAmI && projTypes.Contains(proj.type))
                {
                    proj.Kill();
                }
            }
        }

        public static int FindFirstProjectile(int Type)
        {
            int index = -1;
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj.active && proj.type == Type)
                {
                    index = x;
                    break;
                }
            }
            return index;
        }

        public static void OnlyOneSentry(Player player, int Type)
        {
            int existingTurrets = player.ownedProjectileCounts[Type];
            if (existingTurrets > 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == Type &&
                        Main.projectile[i].owner == player.whoAmI &&
                        Main.projectile[i].active)
                    {
                        Main.projectile[i].Kill();
                        existingTurrets--;
                        if (existingTurrets <= 0)
                            break;
                    }
                }
            }
        }

        public static Projectile ProjectileRain(Vector2 targetPos, float xLimit, float xVariance, float yLimitLower, float yLimitUpper, float projSpeed, int projType, int damage, float knockback, int owner)
        {
            float x = targetPos.X + Main.rand.NextFloat(-xLimit, xLimit);
            if (projType == ProjectileType<AstralStarMagic>())
                x = targetPos.X + xLimit;
            float y = targetPos.Y - Main.rand.NextFloat(yLimitLower, yLimitUpper);
            Vector2 source = new Vector2(x, y);
            Vector2 velocity = targetPos - source;
            velocity.X += Main.rand.NextFloat(-xVariance, xVariance);
            float speed = projSpeed;
            float targetDist = velocity.Length();
            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;
            return Projectile.NewProjectileDirect(source, velocity, projType, damage, knockback, owner);
        }

        public static Projectile ProjectileBarrage(Vector2 originVec, Vector2 targetPos, bool fromRight, float xOffsetMin, float xOffsetMax, float yOffsetMin, float yOffsetMax, float projSpeed, int projType, int damage, float knockback, int owner, bool clamped = false, float inaccuracyOffset = 5f)
        {
            float xPos = originVec.X + Main.rand.NextFloat(xOffsetMin, xOffsetMax) * fromRight.ToDirectionInt();
            float yPos = originVec.Y + Main.rand.NextFloat(yOffsetMin, yOffsetMax) * Main.rand.NextBool().ToDirectionInt();
            Vector2 spawnPosition = new Vector2(xPos, yPos);
            Vector2 velocity = targetPos - spawnPosition;
            velocity.X += Main.rand.NextFloat(-inaccuracyOffset, inaccuracyOffset);
            velocity.Y += Main.rand.NextFloat(-inaccuracyOffset, inaccuracyOffset);
            velocity.Normalize();
            velocity *= projSpeed * (clamped ? 150f : 1f);
            //This clamp means the spawned projectiles only go at diagnals and are not accurate
            if (clamped)
            {
                velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
                velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
            }
            return Projectile.NewProjectileDirect(spawnPosition, velocity, projType, damage, knockback, owner);
        }

        public static int DamageSoftCap(double dmgInput, int cap)
        {
            // If the incoming damage is less than the cap, don't do anything.
            if (dmgInput < cap)
                return (int)dmgInput;

            // Ratio of how far over the cap you are.
            // This is a value from 1.0 upwards to theoretically infinity.
            double overpoweredRatio = dmgInput / cap;

            // Formula which reduces how "overpowered" you are to a reasonable level.
            double cappedRatio = Math.Pow(overpoweredRatio, 0.5) / 1.25 + 0.2;

            // Take the reduced ratio and multiply the cap by it to get the final capped damage.
            return (int)(cap * cappedRatio);
        }

        public static Vector2 RandomVelocity(float directionMult, float speedLowerLimit, float speedCap, float speedMult = 0.1f)
        {
            Vector2 velocity = new Vector2(Main.rand.NextFloat(-directionMult, directionMult), Main.rand.NextFloat(-directionMult, directionMult));
            //Rerolling to avoid dividing by zero
            while (velocity.X == 0f && velocity.Y == 0f)
            {
                velocity = new Vector2(Main.rand.NextFloat(-directionMult, directionMult), Main.rand.NextFloat(-directionMult, directionMult));
            }
            velocity.Normalize();
            velocity *= Main.rand.NextFloat(speedLowerLimit, speedCap) * speedMult;
            return velocity;
        }

        public static void MinionAntiClump(this Projectile projectile, float pushForce = 0.05f)
        {
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible
                if (!otherProj.active || otherProj.owner != projectile.owner || !otherProj.minion || k == projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == projectile.type;
                float taxicabDist = Math.Abs(projectile.position.X - otherProj.position.X) + Math.Abs(projectile.position.Y - otherProj.position.Y);
                if (sameProjType && taxicabDist < projectile.width)
                {
                    if (projectile.position.X < otherProj.position.X)
                        projectile.velocity.X -= pushForce;
                    else
                        projectile.velocity.X += pushForce;

                    if (projectile.position.Y < otherProj.position.Y)
                        projectile.velocity.Y -= pushForce;
                    else
                        projectile.velocity.Y += pushForce;
                }
            }
        }

        public static bool DrawBeam(this Projectile projectile, float length, float spacer, Color lightColor, Texture2D texture = null, bool curve = false)
        {
            if (texture is null)
                texture = Main.projectileTexture[projectile.type];

            float widthOffset = (float)(texture.Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;
            float heightOffset = (float)(projectile.height / 2);
            Vector2 origin = new Vector2(widthOffset, heightOffset);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Rectangle roughScreenBounds = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
            if (projectile.getRect().Intersects(roughScreenBounds))
            {
                Vector2 drawPos = projectile.position - Main.screenPosition + origin;
                drawPos.Y += projectile.gfxOffY;
                float maxTrailPoints = length;

                if (projectile.ai[1] == 1f)
                    maxTrailPoints = (int)projectile.localAI[0];

                Vector2 cumulativeOffset = Vector2.Zero;
                for (int i = 1; i <= (int)projectile.localAI[0]; i++)
                {
                    Vector2 velToUseThisIter = projectile.velocity;
                    if (curve)
                    {
                        float oldVelRatio = i / projectile.localAI[0];
                        int oldVelIndex = (int)(oldVelRatio * projectile.oldRot.Length);
                        if (oldVelIndex > 0)
                        {
                            float angleChange = projectile.oldRot[oldVelIndex - 1] - projectile.rotation;
                            velToUseThisIter = projectile.velocity.RotatedBy(angleChange);
                        }
                    }
                    cumulativeOffset += Vector2.Normalize(velToUseThisIter) * spacer;
                    Color color = projectile.GetAlpha(lightColor);
                    color *= (maxTrailPoints - (float)i) / maxTrailPoints;
                    color.A = 0;
                    float fixedRotation = projectile.rotation + MathHelper.PiOver2;
                    Main.spriteBatch.Draw(texture, drawPos - cumulativeOffset, null, color, fixedRotation, origin, projectile.scale, spriteEffects, 0f);
                }
            }
            return false;
        }

        public static void ExplodeandDestroyTiles(Projectile projectile, int explosionRadius, bool checkExplosions, List<int> tilesToCheck, List<int> wallsToCheck)
        {
            int minTileX = (int)projectile.position.X / 16 - explosionRadius;
            int maxTileX = (int)projectile.position.X / 16 + explosionRadius;
            int minTileY = (int)projectile.position.Y / 16 - explosionRadius;
            int maxTileY = (int)projectile.position.Y / 16 + explosionRadius;
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (maxTileX > Main.maxTilesX)
            {
                maxTileX = Main.maxTilesX;
            }
            if (minTileY < 0)
            {
                minTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }

            bool canKillWalls = false;
            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int y = minTileY; y <= maxTileY; y++)
                {
                    Vector2 explodeArea = new Vector2(Math.Abs(x - projectile.position.X / 16f), Math.Abs(y - projectile.position.Y / 16f));
                    float distance = explodeArea.Length();
                    if (distance < explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].WallType == WallID.None)
                    {
                        canKillWalls = true;
                        break;
                    }
                }
            }

            List<int> tileExcludeList = new List<int>()
            {
                TileID.DemonAltar,
                TileID.ElderCrystalStand
            };
            for (int i = 0; i < tilesToCheck.Count; ++i)
                tileExcludeList.Add(tilesToCheck[i]);
            List<int> wallExcludeList = new List<int>();
            for (int i = 0; i < wallsToCheck.Count; ++i)
                wallExcludeList.Add(wallsToCheck[i]);

            List<int> explosionCheckList = new List<int>()
            {
                TileID.DemonAltar,
                TileID.Cobalt,
                TileID.Mythril,
                TileID.Adamantite,
                TileID.Palladium,
                TileID.Orichalcum,
                TileID.Titanium,
                TileID.Chlorophyte,
                TileID.LihzahrdBrick,
                TileID.LihzahrdAltar,
                TileID.DesertFossil
            };
            AddWithCondition<int>(explosionCheckList, TileID.Hellstone, !Main.hardMode);

            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    bool t = 1 == 1; bool f = 1 == 2;

                    Vector2 explodeArea = new Vector2(Math.Abs(i - projectile.position.X / 16f), Math.Abs(j - projectile.position.Y / 16f));
                    float distance = explodeArea.Length();
                    if (distance < explosionRadius)
                    {
                        bool canKillTile = true;
                        if (tile != null && tile.active())
                        {
                            if (checkExplosions)
                            {
                                if (Main.tileDungeon[tile.TileType] || explosionCheckList.Contains(tile.TileType))
                                {
                                    canKillTile = false;
                                }
                                if (!TileLoader.CanExplode(i, j))
                                {
                                    canKillTile = false;
                                }
                            }
                            if (Main.tileContainer[tile.TileType])
                                canKillTile = false;
                            if (!TileLoader.CanKillTile(i, j, tile.TileType, ref t) || !TileLoader.CanKillTile(i, j, tile.TileType, ref f))
                                canKillTile = false;
                            if (tileExcludeList.Contains(tile.TileType))
                                canKillTile = false;

                            if (canKillTile)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                                if (!tile.active() && Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
                                }
                            }
                        }
                        if (canKillTile)
                        {
                            for (int x = i - 1; x <= i + 1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    bool canExplode = true;
                                    if (checkExplosions)
                                        canExplode = WallLoader.CanExplode(x, y, Main.tile[x, y].WallType);
                                    if (wallExcludeList.Any() && wallExcludeList.Contains(Main.tile[x, y].WallType))
                                        canKillWalls = false;
                                    if (Main.tile[x, y] != null && Main.tile[x, y].WallType > WallID.None && canKillWalls && canExplode)
                                    {
                                        WorldGen.KillWall(x, y, false);
                                        if (Main.tile[x, y].WallType == WallID.None && Main.netMode != NetmodeID.SinglePlayer)
                                        {
                                            NetMessage.SendData(MessageID.TileChange, -1, -1, null, 2, x, y, 0f, 0, 0, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates an explosion which is visually identical to vanilla's Rocket III and Rocket IV on-hit explosions.
        /// </summary>
        /// <param name="projectile">The projectile which is exploding.</param>
        public static void LargeFieryExplosion(this Projectile projectile)
        {
            // Sparks and such
            Vector2 corner = projectile.position;
            for (int i = 0; i < 40; i++)
            {
                int idx = Dust.NewDust(corner, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 70; i++)
            {
                int idx = Dust.NewDust(corner, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(corner, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
            }

            // Smoke, which counts as a Gore
            ExplosionGores(projectile.Center, 3);
        }
    }
}
