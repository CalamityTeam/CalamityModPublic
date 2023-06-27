using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public static class ImpMinionAI
    {
        public static bool DoImpMinionAI(Projectile projectile)
        {
            Player owner = Main.player[projectile.owner];
            ref float timerToShoot = ref projectile.ai[0];
            projectile.aiStyle = -1;

            float enemyDistanceDetection = 1200f;
            float fireRate = 120f; // In frames.
            float projVelocity = 20f;

            // Detects a target.
            NPC target = projectile.Center.MinionHoming(enemyDistanceDetection, owner);

            if (target is not null)
            {
                if (timerToShoot >= fireRate && Main.myPlayer == projectile.owner)
                {
                    int fireball = Projectile.NewProjectile(projectile.GetSource_FromThis(),
                        projectile.Center,
                        CalamityUtils.CalculatePredictiveAimToTarget(projectile.Center, target.Center, target.velocity, projVelocity),
                        ProjectileID.ImpFireball,
                        projectile.damage,
                        projectile.knockBack,
                        owner.whoAmI);

                    if (Main.projectile.IndexInRange(fireball))
                    {
                        Main.projectile[fireball].originalDamage = projectile.originalDamage;
                        Main.projectile[fireball].usesLocalNPCImmunity = true;
                        Main.projectile[fireball].localNPCHitCooldown = 30;
                        Main.projectile[fireball].usesIDStaticNPCImmunity = false;
                    }
                
                    timerToShoot = 0f;
                    projectile.netUpdate = true;
                }
            
                if (timerToShoot < fireRate)
                    timerToShoot++;
            }

            CheckMinionExistence(owner, projectile);

            DoAnimation(projectile);

            DecideDirection(target, projectile);

            FollowPlayer(owner, projectile, enemyDistanceDetection);

            projectile.MinionAntiClump(.5f);

            // Just so the minion doesn't stay weirdly still.
            if (projectile.velocity == Vector2.Zero)
                projectile.velocity += new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));

            // Flavor fire dust effect.
            int dustEffect = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, Main.rand.NextBool(2) ? 1f : -1f, Main.rand.NextBool(2) ? 1f : -1f);
            Main.dust[dustEffect].noGravity = true;

            return false;
        }

        #region Methods
        
        public static void CheckMinionExistence(Player owner, Projectile projectile)
        {
            owner.AddBuff(BuffID.ImpMinion, 1);
            if (projectile.type != ProjectileID.FlyingImp)
                return;
            
            if (owner.dead)
                owner.impMinion = false;
            if (owner.impMinion)
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
            // Both results are negated because some individual decided to make the minion look at the left in the sprite instead of the right.
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