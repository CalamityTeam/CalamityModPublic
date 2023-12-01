using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public static class ImpMinionAI
    {
        // Shorter range when the minion has no target yet: 40 tiles
        private const float MinEnemyDistanceDetection = 640f;

        // Longer range when a target is already acquired: 75 tiles
        private const float MaxEnemyDistanceDetection = 1200f;

        private static float EnemyDistanceDetection { get => Target is null ? MinEnemyDistanceDetection : MaxEnemyDistanceDetection; }

        private const int FireRate = 75;
        private const float ProjectileVelocity = 20f;
        private static NPC Target { get; set; }

        public static bool DoImpMinionAI(Projectile proj)
        {
            Player owner = Main.player[proj.owner];
            Target = owner.Center.MinionHoming(EnemyDistanceDetection, owner, CalamityPlayer.areThereAnyDamnBosses);
            ref float shootTimer = ref proj.ai[0];

            CheckMinionExistence(proj, owner);
            DoAnimation(proj);
            DecideDirection(proj);
            FollowPlayer(proj, owner);
            proj.MinionAntiClump();

            if (Target is not null)
            {
                shootTimer += Main.rand.NextBool(20) ? 2 : 1;
                if (shootTimer >= FireRate && proj.owner == Main.myPlayer)
                {
                    Vector2 toTargetDirection = CalamityUtils.CalculatePredictiveAimToTarget(proj.Center, Target, ProjectileVelocity);

                    Projectile fireball = Projectile.NewProjectileDirect(proj.GetSource_FromThis(),
                        proj.Center,
                        toTargetDirection,
                        ProjectileID.ImpFireball,
                        proj.damage,
                        proj.knockBack,
                        proj.owner);

                    fireball.localNPCHitCooldown = 30;
                    fireball.Size *= 2f;
                    fireball.tileCollide = false;
                    fireball.usesLocalNPCImmunity = true;
                    fireball.usesIDStaticNPCImmunity = false;

                    proj.velocity -= toTargetDirection.SafeNormalize(Vector2.Zero);

                    if (!Main.dedServ)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            Dust shootDust = Dust.NewDustPerfect(proj.Center + toTargetDirection.SafeNormalize(Vector2.Zero) * proj.Size / 2f, DustID.Torch, toTargetDirection.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 5f), Scale: 2f);
                            shootDust.noGravity = true;
                            shootDust.noLight = true;
                            shootDust.noLightEmittence = true;
                        }
                    }

                    shootTimer = 0f;
                    SyncVariables(proj);
                }
            }
            else
            {
                // Quickly deaccelerate when getting too close to the owner.
                if (proj.WithinRange(owner.Center, 128f))
                {
                    proj.velocity *= 0.875f;
                    SyncVariables(proj);
                }
            }

            if (!Main.dedServ)
            {
                // Flavor fire dust effect.
                Dust dustEffect = Dust.NewDustDirect(proj.position, proj.width, proj.height, DustID.Torch);
                dustEffect.noGravity = true;
                dustEffect.noLight = true;
                dustEffect.noLightEmittence = true;
            }

            return false;
        }

        #region AI Methods

        private static void CheckMinionExistence(Projectile proj, Player owner)
        {
            owner.AddBuff(BuffID.ImpMinion, 2);
            if (proj.type != ProjectileID.FlyingImp)
                return;

            if (owner.dead)
                owner.impMinion = false;
            if (owner.impMinion)
                proj.timeLeft = 2;
        }

        private static void DoAnimation(Projectile proj)
        {
            proj.frameCounter++;
            if (proj.frameCounter >= 6)
            {
                proj.frame = (proj.frame + 1) % Main.projFrames[proj.type];
                proj.frameCounter = 0;
            }
        }

        private static void DecideDirection(Projectile proj)
        {
            // Both results are negated because some individual decided to make the minion look at the left in the sprite instead of the right.
            if (Target is not null)
                proj.direction = proj.spriteDirection = (Target.Center.X - proj.Center.X < 0).ToDirectionInt();
            else
                proj.direction = proj.spriteDirection = -proj.velocity.X.DirectionalSign();
        }

        private static void FollowPlayer(Projectile proj, Player owner)
        {
            // The minion will hover around the owner.
            if (!proj.WithinRange(owner.Center, 160f))
            {
                proj.velocity = (proj.velocity + proj.SafeDirectionTo(owner.Center)) * 0.9f;
                SyncVariables(proj);
            }

            // The minion will teleport on the owner if they get far enough.
            if (!proj.WithinRange(owner.Center, MaxEnemyDistanceDetection))
            {
                proj.Center = owner.Center;
                SyncVariables(proj);
            }
        }

        private static void SyncVariables(Projectile proj)
        {
            proj.netUpdate = true;
            if (proj.netSpam >= 10)
                proj.netSpam = 9;
        }

        #endregion
    }
}
