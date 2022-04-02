using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class YoungDuke : ModProjectile
    {
        public const float DistanceBeforeCharge = 420f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Young Old Duke");
            Main.projFrames[projectile.type] = 16;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.miniOldDuke)
            {
                projectile.active = false;
                return;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<YoungDuke>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.youngDuke = false;
                }
                if (modPlayer.youngDuke)
                {
                    projectile.timeLeft = 2;
                }
            }

            // Adjust damage as needed
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }

            bool playerHalfLife = player.statLife <= player.statLifeMax2 * 0.5f;

            // Frames
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (playerHalfLife && projectile.frame < Main.projFrames[projectile.type] / 2)
            {
                projectile.frame += Main.projFrames[projectile.type] / 2;
            }
            else if (!playerHalfLife && projectile.frame >= Main.projFrames[projectile.type] / 2)
            {
                projectile.frame -= Main.projFrames[projectile.type] / 2;
            }

            NPC potentialTarget = projectile.Center.ClosestNPCAt(1600f);
            if (potentialTarget != null)
            {
                if (projectile.Distance(potentialTarget.Center) < DistanceBeforeCharge)
                {
                    projectile.ai[0] += 1f;
                    int timePerCharge = playerHalfLife ? 25 : 35;
                    float chargeSpeed = playerHalfLife ? 25f : 20f;
                    if (projectile.ai[0] >= timePerCharge)
                    {
                        projectile.velocity = projectile.SafeDirectionTo(potentialTarget.Center) * chargeSpeed;
                        projectile.ai[0] = 0f;
                    }
                    else
                        projectile.velocity *= 0.9825f;
                }
                else
                {
                    float intertia = 0.94f;
                    float homeSpeed = playerHalfLife ? 38f : 32f;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(potentialTarget.Center) * homeSpeed, 1f - intertia);
                }
            }
            else if (!projectile.WithinRange(player.Center, 140f))
                projectile.velocity = (projectile.velocity * 30f + projectile.SafeDirectionTo(player.Center) * 16f) / 31f;

            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
        }
    }
}
