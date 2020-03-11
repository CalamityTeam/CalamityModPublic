using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlaguebringerMK2 : ModProjectile
    {
        public const float DistanceToCheck = 1000.0001f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer MK2");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 38;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
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
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
			}
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<PlaguebringerMK2>();
            player.AddBuff(ModContent.BuffType<FuelCellBundleBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.plaguebringerMK2 = false;
                }
                if (modPlayer.plaguebringerMK2)
                {
                    projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);

            if (potentialTarget != null)
            {
                int sign = (potentialTarget.Center.X - projectile.Center.X < 0).ToDirectionInt();
                Vector2 destination = potentialTarget.Center + new Vector2((125f + 40f * projectile.ai[1]) * sign, -160f);
                if (projectile.Distance(destination) < 65f)
                {
                    projectile.velocity *= 0.9f;
                }
                else
                {
                    Vector2 velocity = projectile.DirectionTo(destination) * projectile.Distance(destination) / 36f;
                    projectile.velocity = velocity;
                }
                int timeNeeded = (int)MathHelper.Lerp(60f, 18f, MathHelper.Clamp(projectile.localAI[1] / 320f, 0f, 1f));
                if (projectile.ai[0] >= timeNeeded)
                {
                    Projectile.NewProjectile(projectile.Center,
                        projectile.DirectionTo(potentialTarget.Center) * 14f,
                        ModContent.ProjectileType<MK2RocketNormal>(),
                        (int)(projectile.damage * 0.4),
                        3f,
                        projectile.owner);
                    Projectile.NewProjectile(projectile.Center,
                        projectile.DirectionTo(potentialTarget.Center) * 11.5f,
                        ModContent.ProjectileType<MK2RocketHoming>(),
                        (int)(projectile.damage * 0.4),
                        3f,
                        projectile.owner);
                    projectile.ai[0] = 0;
                }
                else projectile.ai[0]++;
                projectile.localAI[1]++;
                projectile.direction = projectile.spriteDirection = -sign;
            }
            else
            {
                projectile.localAI[1] = 0;
                Vector2 distanceToDestination = player.Center - projectile.Center;
                distanceToDestination.X -= 10f * player.direction;
                distanceToDestination.X -= projectile.ai[1] * 40 * player.direction;
                distanceToDestination.Y -= 10f;
                float distance = distanceToDestination.Length();
                if (distance > 10f)
                {
                    float speed = 20f;
                    if (distance < 50f)
                    {
                        speed /= 2f;
                    }
                    Vector2 velocity = distanceToDestination.SafeNormalize(projectile.direction * Vector2.UnitX) * speed;
                    projectile.velocity = (projectile.velocity * 20f + velocity) / 21f;
                    if (distance > 2250f)
                        projectile.Center = player.Center;
                }
                else
                {
                    projectile.direction = player.direction;
                    projectile.velocity *= 0.9f;
                }
                projectile.direction = projectile.spriteDirection = player.direction;
            }

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

            Lighting.AddLight(projectile.Center - Vector2.UnitY * 21f, 0.25f, 0.865f, 0.825f);
        }
    }
}
