using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        public static bool AnyProjectiles(int projectileID)
        {
            // Efficiently loop through all projectiles, using a specially designed continue continue that attempts to minimize the amount of OR
            // checks per iteration.
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.type != projectileID || !p.active)
                    continue;

                return true;
            }

            return false;
        }

        public static IEnumerable<Projectile> AllProjectilesByID(int projectileID)
        {
            // This uses the same efficient loop idea as AnyProjectiles.
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.type != projectileID || !p.active)
                    continue;

                yield return p;
            }
        }

        public static int CountProjectiles(int projectileID) => Main.projectile.Count(proj => proj.type == projectileID && proj.active);

        public static int CountHookProj() => Main.projectile.Count(proj => Main.projHook[proj.type] && proj.ai[0] == 2f && proj.active && proj.owner == Main.myPlayer);

        public static bool FinalExtraUpdate(this Projectile proj) => proj.numUpdates == -1;

        public static bool IsTrueMelee(this Projectile proj)
        {
            if (proj is null || !proj.active)
                return false;
            return proj.CountsAsClass<TrueMeleeDamageClass>() || proj.CountsAsClass<TrueMeleeNoSpeedDamageClass>();
        }

        public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
        {
            return projectile.ModProjectile as T;
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

        #region Projectile AI Utilities
        public static void ExpandHitboxBy(this Projectile projectile, int width, int height)
        {
            projectile.position = projectile.Center;
            projectile.width = width;
            projectile.height = height;
            projectile.position -= projectile.Size * 0.5f;
        }
        public static void ExpandHitboxBy(this Projectile projectile, int newSize) => projectile.ExpandHitboxBy(newSize, newSize);
        public static void ExpandHitboxBy(this Projectile projectile, Vector2 newSize) => projectile.ExpandHitboxBy((int)newSize.X, (int)newSize.Y);
        public static void ExpandHitboxBy(this Projectile projectile, float expandRatio) => projectile.ExpandHitboxBy((int)(projectile.width * expandRatio), (int)(projectile.height * expandRatio));

        public static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float N)
        {
            if (!projectile.friendly)
                return;

            // Set amount of extra updates.
            if (projectile.Calamity().defExtraUpdates == -1)
                projectile.Calamity().defExtraUpdates = projectile.extraUpdates;

            Vector2 destination = projectile.Center;
            float maxDistance = distanceRequired;
            bool locatedTarget = false;

            // Find a target.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);
                if (!Main.npc[i].CanBeChasedBy(projectile, false) || !projectile.WithinRange(Main.npc[i].Center, maxDistance + extraDistance))
                    continue;

                if (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    destination = Main.npc[i].Center;
                    locatedTarget = true;
                    break;
                }
            }

            if (locatedTarget)
            {
                // Increase amount of extra updates to greatly increase homing velocity.
                projectile.extraUpdates = projectile.Calamity().defExtraUpdates + 1;

                // Home in on the target.
                Vector2 homeDirection = (destination - projectile.Center).SafeNormalize(Vector2.UnitY);
                projectile.velocity = (projectile.velocity * N + homeDirection * homingVelocity) / (N + 1f);
            }
            else
            {
                // Set amount of extra updates to default amount.
                projectile.extraUpdates = projectile.Calamity().defExtraUpdates;
            }
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
        /// <param name="predictionStrength">The ratio for how much the projectile aims ahead of the target. 1f is normal predictiveness. 0.01f is the lowest possible value, equating to no practical predictiveness.</param>
        public static Vector2 SuperhomeTowardsTarget(this Projectile projectile, NPC target, float homingSpeed, float inertia, float predictionStrength = 1f)
        {
            if (predictionStrength < 0.01f) { predictionStrength = 0.01f; }
            Vector2 idealVelocity = CalculatePredictiveAimToTarget(projectile.Center, target, homingSpeed / predictionStrength) * predictionStrength;
            return (projectile.velocity * (inertia - 1f) + idealVelocity) / inertia;
        }
        #endregion

        #region Projectile Spawning Utilities
        public static Projectile ProjectileRain(IEntitySource source, Vector2 targetPos, float xLimit, float xVariance, float yLimitLower, float yLimitUpper, float projSpeed, int projType, int damage, float knockback, int owner)
        {
            float x = targetPos.X + Main.rand.NextFloat(-xLimit, xLimit);
            if (projType == ProjectileType<AstralStarMagic>())
                x = targetPos.X + xLimit;
            float y = targetPos.Y - Main.rand.NextFloat(yLimitLower, yLimitUpper);
            Vector2 spawnPosition = new Vector2(x, y);
            Vector2 velocity = targetPos - spawnPosition;
            velocity.X += Main.rand.NextFloat(-xVariance, xVariance);
            float speed = projSpeed;
            float targetDist = velocity.Length();
            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;
            return Projectile.NewProjectileDirect(source, spawnPosition, velocity, projType, damage, knockback, owner);
        }

        public static Projectile ProjectileBarrage(IEntitySource source, Vector2 originVec, Vector2 targetPos, bool fromRight, float xOffsetMin, float xOffsetMax, float yOffsetMin, float yOffsetMax, float projSpeed, int projType, int damage, float knockback, int owner, bool clamped = false, float inaccuracyOffset = 5f)
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
            return Projectile.NewProjectileDirect(source, spawnPosition, velocity, projType, damage, knockback, owner);
        }

        public static Projectile SpawnOrb(Projectile projectile, int damage, int projType, float distanceRequired, float speedMult, bool gsPhantom = false)
        {
            float ai1 = Main.rand.NextFloat() + 0.5f;
            int[] array = new int[Main.maxNPCs];
            int targetArrayA = 0;
            int targetArrayB = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float enemyDist = Vector2.Distance(projectile.Center, npc.Center);
                    if (enemyDist < distanceRequired)
                    {
                        if (Collision.CanHit(projectile.position, 1, 1, npc.position, npc.width, npc.height) && enemyDist > 50f)
                        {
                            array[targetArrayB] = i;
                            targetArrayB++;
                        }
                        else if (targetArrayB == 0)
                        {
                            array[targetArrayA] = i;
                            targetArrayA++;
                        }
                    }
                }
            }
            if (targetArrayA == 0 && targetArrayB == 0)
            {
                return Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<NobodyKnows>(), 0, 0f, projectile.owner);
            }
            int target = targetArrayB <= 0 ? array[Main.rand.Next(targetArrayA)] : array[Main.rand.Next(targetArrayB)];
            Vector2 velocity = RandomVelocity(100f, speedMult, speedMult, 1f);
            Projectile orb = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.Center, velocity, projType, damage, 0f, projectile.owner, gsPhantom ? 0f : target, gsPhantom ? ai1 : 0f);
            return orb;
        }

        // TODO -- This overused method should NOT have hardcoded projectile type checks in it.
        public static void MagnetSphereHitscan(Projectile projectile, float distanceRequired, float homingVelocity, float projectileTimer, int maxTargets, int spawnedProjectile, double damageMult = 1D, bool attackMultiple = false)
        {
            // Only shoot once every N frames.
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > projectileTimer)
            {
                projectile.localAI[1] = 0f;

                // Only search for targets if projectiles could be fired.
                float maxDistance = distanceRequired;
                bool homeIn = false;
                int[] targetArray = new int[maxTargets];
                int targetArrayIndex = 0;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                        bool canHit = true;
                        if (extraDistance < maxDistance)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                        if (projectile.WithinRange(Main.npc[i].Center, maxDistance + extraDistance) && canHit)
                        {
                            if (targetArrayIndex < maxTargets)
                            {
                                targetArray[targetArrayIndex] = i;
                                targetArrayIndex++;
                                homeIn = true;
                            }
                            else
                                break;
                        }
                    }
                }

                // If there is anything to actually shoot at, pick targets at random and fire.
                if (homeIn)
                {
                    int randomTarget = Main.rand.Next(targetArrayIndex);
                    randomTarget = targetArray[randomTarget];

                    projectile.localAI[1] = 0f;
                    Vector2 spawnPos = projectile.Center + projectile.velocity * 4f;
                    Vector2 velocity = Vector2.Normalize(Main.npc[randomTarget].Center - spawnPos) * homingVelocity;

                    if (attackMultiple)
                    {
                        for (int i = 0; i < targetArrayIndex; i++)
                        {
                            velocity = Vector2.Normalize(Main.npc[targetArray[i]].Center - spawnPos) * homingVelocity;

                            if (projectile.owner == Main.myPlayer)
                            {
                                int projectile2 = Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, velocity, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);

                                if (projectile.type == ProjectileType<EradicatorProjectile>())
                                    if (projectile2.WithinBounds(Main.maxProjectiles))
                                        Main.projectile[projectile2].DamageType = RogueDamageClass.Instance;
                            }
                        }

                        return;
                    }

                    if (projectile.type == ProjectileType<GodsGambitYoyo>())
                    {
                        velocity.Y += Main.rand.Next(-30, 31) * 0.05f;
                        velocity.X += Main.rand.Next(-30, 31) * 0.05f;
                    }

                    if (projectile.owner == Main.myPlayer)
                    {
                        int projectile2 = Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, velocity, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);

                        if (projectile.type == ProjectileType<GodsGambitYoyo>() || projectile.type == ProjectileType<ShimmersparkYoyo>())
                            if (projectile2.WithinBounds(Main.maxProjectiles))
                                Main.projectile[projectile2].DamageType = DamageClass.MeleeNoSpeed;
                    }
                }
            }
        }
        #endregion

        #region Projectile Despawning/Killing Utilities
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

        public struct RocketBehaviorInfo
        {
            internal int rocketItemType;
            
            // Explosion radii for various rocket ammos. Defaults to the sizes used in vanilla launchers.
            public int smallRadius = 3; // Rocket I and II
            public int mediumRadius = 6; // Rocket III and IV
            public int largeRadius = 9; // Mini Nuke and Cluster Rockets

            public bool respectStandardBlastImmunity = true;
            public List<int> tilesToCheck = null;
            public List<int> wallsToCheck = null;

            public int clusterProjectileID = ProjectileID.ClusterFragmentsI;
            public int destructiveClusterProjectileID = ProjectileID.ClusterFragmentsII;
            public float clusterSplitDamageMultiplier = 0.5f;

            public RocketBehaviorInfo(int rocketID)
            {
                rocketItemType = rocketID;
            }
        }

        /// <summary>
        /// For a given projectile that is used as a rocket and uses rocket ammo, this utility provides a shorthand way to check for checking what behaviour should each type of ammo do. It can also return you the radius of the explosion that'll happen so you can use it for your effect's size.
        /// </summary>
        /// <param name="proj">The projectile in question.</param>
        /// <param name="info">Struct containing information on the desired rocket behavior.</param>
        public static int RocketBehavior(this Projectile proj, RocketBehaviorInfo info)
        {
            int explosionRadius = 0;

            // Used for Cluster Rockets to determine damage.

            // Used for Dry Rockets, Water Rockets etc. to place water. Not always needed.
            Point center = proj.Center.ToTileCoordinates();
            DelegateMethods.v2_1 = center.ToVector2();
            DelegateMethods.f_1 = 3f;

            void SpawnClusterFragments(bool destructiveVariant = false)
            {
                if (proj.owner != Main.myPlayer)
                    return;

                int projID = destructiveVariant ? info.destructiveClusterProjectileID : info.clusterProjectileID;
                int clusterDamage = (int)(proj.damage * info.clusterSplitDamageMultiplier);

                float thetaStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                for (float i = 0; i < 6; ++i)
                {
                    float thetaIter = thetaStart + (i * MathHelper.TwoPi / 6f);
                    float dist = Main.rand.NextFloat(4f, 6f);

                    Vector2 clusterVel = thetaIter.ToRotationVector2() * dist - Vector2.UnitY;

                    Projectile clusterFragment = Projectile.NewProjectileDirect(proj.GetSource_FromThis(), proj.Center, clusterVel, projID, clusterDamage, 0f, proj.owner);
                    clusterFragment.timeLeft -= Main.rand.Next(30);
                }
            }

            switch (info.rocketItemType)
            {
                case ItemID.RocketI:
                    explosionRadius = info.smallRadius;
                    break;

                case ItemID.RocketII:
                    explosionRadius = info.smallRadius;
                    proj.ExplodeTiles(explosionRadius, info.respectStandardBlastImmunity, info.tilesToCheck, info.wallsToCheck);
                    break;

                case ItemID.RocketIII:
                    explosionRadius = info.mediumRadius;
                    break;

                case ItemID.RocketIV:
                    explosionRadius = info.mediumRadius;
                    proj.ExplodeTiles(explosionRadius, info.respectStandardBlastImmunity, info.tilesToCheck, info.wallsToCheck);
                    break;

                case ItemID.MiniNukeI:
                    explosionRadius = info.largeRadius;
                    break;

                case ItemID.MiniNukeII:
                    explosionRadius = info.largeRadius;
                    proj.ExplodeTiles(explosionRadius, info.respectStandardBlastImmunity, info.tilesToCheck, info.wallsToCheck);
                    break;

                case ItemID.ClusterRocketI:
                    explosionRadius = info.largeRadius;
                    SpawnClusterFragments(false);
                    break;

                case ItemID.ClusterRocketII:
                    explosionRadius = info.largeRadius;
                    SpawnClusterFragments(true);
                    break;

                case ItemID.DryRocket:
                    DelegateMethods.f_1 = 3.5f;
                    Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadDry);
                    break;

                case ItemID.WetRocket:
                    Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadWater);
                    break;

                case ItemID.LavaRocket:
                    Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadLava);
                    break;

                case ItemID.HoneyRocket:
                    Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadHoney);
                    break;
            }

            return explosionRadius;
        }
        #endregion

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
                texture = TextureAssets.Projectile[projectile.type].Value;

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
                Color alpha = projectile.GetAlpha(lightColor);
                float fixedRotation = projectile.rotation + MathHelper.PiOver2;
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
                    Color color = alpha;
                    color *= (maxTrailPoints - (float)i) / maxTrailPoints;
                    color.A = 0;
                    Main.spriteBatch.Draw(texture, drawPos - cumulativeOffset, null, color, fixedRotation, origin, projectile.scale, spriteEffects, 0f);
                }
            }
            return false;
        }

        public static void DrawBackglow(this Projectile projectile, Color backglowColor, float backglowArea, Texture2D? texture = null, Rectangle? frame = null)
        {
            texture ??= TextureAssets.Projectile[projectile.type].Value;

            // Use a fallback for the frame.
            frame ??= texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Value.Size() * 0.5f;
            Color backAfterimageColor = backglowColor * projectile.Opacity;
            for (int i = 0; i < 10; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 10f).ToRotationVector2() * backglowArea;
                Main.spriteBatch.Draw(texture, drawPosition + drawOffset, frame, backAfterimageColor, projectile.rotation, origin, projectile.scale, 0, 0f);
            }
        }

        public static void DrawProjectileWithBackglow(this Projectile projectile, Color backglowColor, Color lightColor, float backglowArea, Texture2D? texture = null, Rectangle? frame = null)
        {
            texture ??= TextureAssets.Projectile[projectile.type].Value;

            // Use a fallback for the frame.
            frame ??= texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Value.Size() * 0.5f;

            projectile.DrawBackglow(backglowColor, backglowArea, texture, frame);
            Main.spriteBatch.Draw(texture, drawPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, 0, 0f);
        }

        public static void DrawStarTrail(this Projectile projectile, Color outer, Color inner, float auraHeight = 10f)
        {
            Texture2D aura = ModContent.Request<Texture2D>("CalamityMod/Projectiles/StarTrail").Value;
            Vector2 offsets = new Vector2(0f, projectile.gfxOffY) - Main.screenPosition;
            Rectangle auraRec = aura.Frame();
            float auraRotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 auraOrigin = new Vector2(auraRec.Width / 2f, auraHeight);

            // Outer trail
            Vector2 drawStartOuter = offsets + projectile.Center + projectile.velocity;
            Vector2 spinPoint = -Vector2.UnitY * auraHeight;
            float time = Main.player[projectile.owner].miscCounter % 216000f / 60f;
            Color outerColor = outer * 0.2f;
            outerColor.A = 0;
            float rotation = MathHelper.TwoPi * time;
            for (int o = 0; o < 6; o += 2)
            {
                Vector2 spinStart = drawStartOuter + spinPoint.RotatedBy(rotation - MathHelper.Pi * o / 3f);
                float scaleMultOuter = 1.5f - o * 0.1f;
                Main.EntitySpriteDraw(aura, spinStart, auraRec, outerColor, auraRotation, auraOrigin, scaleMultOuter, SpriteEffects.None, 0);
            }

            // Inner trail
            Vector2 drawStartInner = offsets + projectile.Center - projectile.velocity * 0.5f;
            Color innerColor = inner * 0.5f;
            innerColor.A = 0;
            for (float i = 0f; i < 1f; i += 0.5f)
            {
                float scaleMult = time % 0.5f / 0.5f;
                scaleMult = (scaleMult + i) % 1f;
                float colorMult = scaleMult * 2f;
                if (colorMult > 1f)
                    colorMult = 2f - colorMult;

                Main.EntitySpriteDraw(aura, drawStartInner, auraRec, innerColor * colorMult, auraRotation, auraOrigin, 0.3f + scaleMult * 0.5f, SpriteEffects.None, 0);
            }
        }

        private static readonly List<int> vanillaBlastImmuneTiles = new List<int>()
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

        public static void ExplodeTiles(this Projectile p, int explosionRadius, bool respectStandardBlastImmunity = true, IEnumerable<int> customBlastImmuneTiles = null, IEnumerable<int> customBlastImmuneWalls = null)
            => ExplodeTiles(p.Center, explosionRadius, respectStandardBlastImmunity, customBlastImmuneTiles, customBlastImmuneWalls);
        public static void ExplodeTiles(Vector2 explosionPos, int explosionRadius, bool respectStandardBlastImmunity = true, IEnumerable<int> customBlastImmuneTiles = null, IEnumerable<int> customBlastImmuneWalls = null)
        {
            // Define limits for explosion iteration.
            int minTileX = (int)explosionPos.X / 16 - explosionRadius;
            int maxTileX = (int)explosionPos.X / 16 + explosionRadius;
            int minTileY = (int)explosionPos.Y / 16 - explosionRadius;
            int maxTileY = (int)explosionPos.Y / 16 + explosionRadius;
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

            // This checks for whether the explosion should be allowed to destroy walls. It's rather arbitrary, but it's how vanilla works.
            bool allowWallDestruction = false;
            float projTileX = explosionPos.X / 16f;
            float projTileY = explosionPos.Y / 16f;
            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int y = minTileY; y <= maxTileY; y++)
                {
                    Vector2 explodeArea = new Vector2(Math.Abs(x - projTileX), Math.Abs(y - projTileY));
                    float distance = explodeArea.Length();
                    if (distance < explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].WallType == WallID.None)
                    {
                        allowWallDestruction = true;
                        break;
                    }
                }
            }

            // Tiles which can never be exploded under any circumstances. Bad things happen if they blow up.
            HashSet<int> blastImmuneTiles = new()
            {
                TileID.DemonAltar,
                TileID.ElderCrystalStand
            };

            // If respecting vanilla blast immunities, toss in that whole list.
            if (respectStandardBlastImmunity)
            {
                foreach (int tileID in vanillaBlastImmuneTiles)
                    blastImmuneTiles.Add(tileID);

                // Conditionally toss in Hellstone if it's not Hardmode yet.
                if (!Main.hardMode)
                    blastImmuneTiles.Add(TileID.Hellstone);
            }

            // If specified, add custom blast immune tiles.
            if (customBlastImmuneTiles is not null)
                foreach (int tileID in customBlastImmuneTiles)
                    blastImmuneTiles.Add(tileID);

            // If specified, define custom blast immune walls.
            HashSet<int> blastImmuneWalls = null;
            if (customBlastImmuneWalls is not null)
            {
                blastImmuneWalls = new();
                foreach (int wallID in customBlastImmuneWalls)
                    blastImmuneWalls.Add(wallID);
            }

            // Actually perform the explosion.
            bool refTrue = true, refFalse = false;
            for (int tx = minTileX; tx <= maxTileX; tx++)
            {
                for (int ty = minTileY; ty <= maxTileY; ty++)
                {
                    Tile tile = Main.tile[tx, ty];
                    ushort type = tile.TileType;

                    Vector2 explodeArea = new Vector2(Math.Abs(tx - projTileX), Math.Abs(ty - projTileY));
                    float distance = explodeArea.Length();
                    if (distance >= explosionRadius)
                        continue;

                    bool canBlastThisTile = true;
                    if (tile != null && tile.HasTile)
                    {
                        if (blastImmuneTiles.Contains(type) || // Respects standard blast immunities if enabled, so they're covered
                            Main.tileContainer[tile.TileType] || // Chests should never be exploded
                            // Dungeon tiles and TileLoader CanExplode are considered part of respecting standard blast immunities
                            respectStandardBlastImmunity && (Main.tileDungeon[type] || !TileLoader.CanExplode(tx, ty)) ||
                            // TileLoader CanKillTile can block the destruction of a tile regardless of whether it is via an explosion
                            !TileLoader.CanKillTile(tx, ty, tile.TileType, ref refTrue) || !TileLoader.CanKillTile(tx, ty, tile.TileType, ref refFalse))
                        {
                            canBlastThisTile = false;
                        }

                        // Destroy the tile itself (not the wall).
                        if (canBlastThisTile)
                        {
                            WorldGen.KillTile(tx, ty, false, false, false);

                            // If the tile was actually destroyed (KillTile can fail) then send netcode indicating as such.
                            if (!tile.HasTile && Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, tx, ty, 0f, 0, 0, 0);
                        }
                    }

                    // Skip destroying walls if the tile was not blasted
                    // Also skip destroying walls if this explosion is not allowed to destroy walls by vanilla rules
                    if (!canBlastThisTile || !allowWallDestruction)
                        continue;

                    // For every destroyed tile, destroy a 3x3 area of walls around it to prevent ugly wall corners
                    // This is what causes explosion packet spam, btw.
                    for (int wx = tx - 1; wx <= tx + 1; wx++)
                    {
                        for (int wy = ty - 1; wy <= ty + 1; wy++)
                        {
                            // Check whether this wall is explodable.
                            bool canBlastThisWall = !respectStandardBlastImmunity || WallLoader.CanExplode(wx, wy, Main.tile[wx, wy].WallType);

                            // If custom wall blast immunities were defined, respect them.
                            // If this is what stops a wall from being blown up, prevent all further wall destruction to prevent ugly floating walls.
                            if (blastImmuneWalls is not null && blastImmuneWalls.Contains(Main.tile[wx, wy].WallType))
                            {
                                allowWallDestruction = false;
                                goto PostWallBlastLoop; // Walls cannot be destroyed for the remainder of this explosion. Stop now.
                            }

                            // Destroy the wall itself.
                            if (Main.tile[wx, wy] != null && Main.tile[wx, wy].WallType > WallID.None && canBlastThisWall)
                            {
                                WorldGen.KillWall(wx, wy, false);

                                // If the wall was actually destroyed (KillWall can fail) then send netcode indicating as such.
                                if (Main.tile[wx, wy].WallType == WallID.None && Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, wx, wy, 0f, 0, 0, 0);
                            }
                        }
                    }

                    // Label to jump to if wall destruction is aborted.
                    PostWallBlastLoop:;
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
                if (Main.rand.NextBool())
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
            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
        }
    }
}
