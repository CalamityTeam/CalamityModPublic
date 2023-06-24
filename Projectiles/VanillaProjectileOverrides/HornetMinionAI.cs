using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public static class HornetMinionAI
    {
        public static bool DoHornetMinionAI(Projectile projectile)
        {
            Player owner = Main.player[projectile.owner];
            ref float timerToBurst = ref projectile.ai[0];
            ref float timerForProjectiles = ref projectile.ai[1];
            projectile.aiStyle = -1;

            float enemyDistanceDetection = 1200f;
            float amountOfProjectilesPerBurst = 3f;
            float timeBetweenBurst = 180f;
            float timeBetweenProjectiles = 20f;
            float projVelocity = 15f;

            NPC target = projectile.Center.MinionHoming(enemyDistanceDetection, owner);

            if (target is not null)
            {
                timerToBurst++;
                if (timerToBurst >= timeBetweenBurst)
                {
                    timerForProjectiles++;

                    if (timerForProjectiles % timeBetweenProjectiles == 0f && Main.myPlayer == projectile.owner)
                    {
                        int stinger = Projectile.NewProjectile(projectile.GetSource_FromThis(),
                            projectile.Center,
                            CalamityUtils.CalculatePredictiveAimToTarget(projectile.Center, target.Center, target.velocity, projVelocity),
                            ProjectileID.HornetStinger,
                            projectile.damage,
                            projectile.knockBack,
                            owner.whoAmI);

                        if (Main.projectile.IndexInRange(stinger))
                            Main.projectile[stinger].originalDamage = projectile.originalDamage;
                    
                        projectile.netUpdate = true;
                    }

                    if (timerForProjectiles >= timeBetweenProjectiles * amountOfProjectilesPerBurst)
                    {
                        timerForProjectiles = 0f;
                        timerToBurst = 0f;
                        projectile.netUpdate = true;
                    }
                }
            }

            CheckMinionExistence(owner, projectile);

            DoAnimation(projectile);

            DecideDirection(target, projectile);

            FollowPlayer(owner, projectile, enemyDistanceDetection);

            projectile.MinionAntiClump(.5f);

            // Just so the minion doesn't stay weirdly still.
            if (projectile.velocity == Vector2.Zero)
                projectile.velocity += new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
            
            return false;
        }

        #region Methods

        public static void CheckMinionExistence(Player owner, Projectile projectile)
        {
            owner.AddBuff(BuffID.HornetMinion, 1);
            if (projectile.type != ProjectileID.Hornet)
                return;

            if (owner.dead)
                owner.hornetMinion = false;
            if (owner.hornetMinion)
                projectile.timeLeft = 2;
        }

        public static void DoAnimation(Projectile projectile)
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 6 == 0)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }

        public static void DecideDirection(NPC target, Projectile projectile)
        {
            // Both directions are negated because some individual decided to make the minion look at the left in the sprite instead of the right.
            if (target is not null)
                projectile.direction = projectile.spriteDirection = (target.Center.X - projectile.Center.X < 0).ToDirectionInt();
            else
                projectile.direction = projectile.spriteDirection = -projectile.velocity.X.DirectionalSign();
                
        }

        public static void FollowPlayer(Player owner, Projectile projectile, float enemyDistanceDetection)
        {    
            // If the minion starts to get far, force the minion to go to you.
            if (projectile.WithinRange(owner.Center, enemyDistanceDetection) && !projectile.WithinRange(owner.Center, 300f))
            {
                projectile.velocity = (owner.Center - projectile.Center) / 30f;
                projectile.netUpdate = true;
            }

            // The minion will change directions to you if it's going away from you, meaning it'll just hover around you.
            else if (!projectile.WithinRange(owner.Center, 160f))
            {
                projectile.velocity = (projectile.velocity * 37f + projectile.SafeDirectionTo(owner.Center) * 17f) / 40f;
                projectile.netUpdate = true;
            }

            // Teleport to the owner if sufficiently far away.
            if (!projectile.WithinRange(owner.Center, enemyDistanceDetection))
            {
                projectile.Center = owner.Center;
                projectile.velocity *= 0.3f;
                projectile.netUpdate = true;
            }
        }

        #endregion
    }
}