using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class RustyDrone : ModProjectile
    {
        public const float PlayerHomingInertia = 32f;
        public const float PlayerHomingSpeed = 11f;

        public const float NPCHomingInertia = 20f;
        public const float NPCHomingSpeed = 14f;

        public const float AttackRate = 140f;

        public const float DistanceToCheck = 585f;

        public const int ExplosionShrapnelBaseDamage = 16; // Uses player.MinionDamage

        public const int ResummonCooldownTime = 60 * 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Drone");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.alpha = 255;
            projectile.Calamity().overridesMinionDamagePrevention = true; // Will only do damage once, and the location of said damage must be deliberate.
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                int oldDamage = projectile.damage;
                projectile.damage = 1; // For the initial tiny prick effect
                projectile.Damage();
                projectile.damage = oldDamage;
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.Center -= Vector2.UnitY * 1400f;
                projectile.localAI[0] += 1f;
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha = (int)MathHelper.Lerp(projectile.alpha, 0f, 0.02f);
                projectile.velocity = Vector2.UnitY * 10f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<RustyDrone>();
            player.AddBuff(ModContent.BuffType<RustyDroneBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    modPlayer.rustyDrone = false;
                }
                if (modPlayer.rustyDrone)
                {
                    projectile.timeLeft = 2;
                }
            }
            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget == null)
            {
                if (projectile.Distance(player.Center) > 120f)
                {
                    projectile.velocity = (projectile.velocity * PlayerHomingInertia + projectile.SafeDirectionTo(player.Top) * PlayerHomingSpeed) / (PlayerHomingInertia + 1);
                }
                if (projectile.frame >= 8)
                {
                    projectile.frame = 0;
                }
            }
            else
            {
                // A bit above the target, and always on the opposite side of the target's sprite direction.
                Vector2 destination = potentialTarget.Top + Vector2.UnitX * -potentialTarget.spriteDirection * Main.rand.NextFloat(50f, 75f) - Vector2.UnitY * 42f;
                projectile.velocity = (projectile.velocity * NPCHomingInertia + projectile.SafeDirectionTo(destination) * NPCHomingSpeed) / (NPCHomingInertia + 1);
                if (projectile.frame < 8)
                {
                    projectile.frame = 8;
                }
                if (projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 8;
                }
                if (projectile.Distance(destination) < 55f)
                {
                    if (projectile.ai[0]++ % AttackRate == AttackRate - 1)
                    {
                        if (projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(potentialTarget.Center, Vector2.Zero, ModContent.ProjectileType<RustyDroneTargetIndicator>(), 0, 0f, projectile.owner, projectile.whoAmI, potentialTarget.whoAmI);
                        }
                    }
                }
            }
            projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.X * 0.05f;
            projectile.ai[1]++;
            if (projectile.ai[1] >= 360f)
            {
                Utils.PoofOfSmoke(projectile.Center);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 8f, ModContent.ProjectileType<RustShrapnel>(),
                        (int)(ExplosionShrapnelBaseDamage * player.MinionDamage()), 2f, projectile.owner);
                }
                projectile.Kill();
            }
        }

        public override bool CanDamage() => projectile.localAI[0] == 0f; // Only do damage from the initial 1 damage prick, and never again. This is a support summon.
    }
}
