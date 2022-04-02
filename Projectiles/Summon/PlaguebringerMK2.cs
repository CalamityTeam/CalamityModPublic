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
                float x = (125f + 40f * (int)(projectile.ai[1] % 10f)) * sign;
                int y = -160 - 50 * (int)(projectile.ai[1] / 10);
                Vector2 destination = potentialTarget.Center + new Vector2(x, y);
                if (projectile.Distance(destination) < 6f)
                {
                    projectile.velocity *= 0.9f;
                }
                else
                    projectile.velocity = projectile.SafeDirectionTo(destination) * projectile.Distance(destination) / 36f;

                int timeNeeded = (int)MathHelper.Lerp(60f, 18f, MathHelper.Clamp(projectile.localAI[1] / 320f, 0f, 1f));
                if (projectile.ai[0] >= timeNeeded && Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.Center,
                        projectile.SafeDirectionTo(potentialTarget.Center) * 14f,
                        ModContent.ProjectileType<MK2RocketNormal>(),
                        (int)(projectile.damage * 0.9),
                        3f,
                        projectile.owner);
                    Projectile.NewProjectile(projectile.Center,
                        projectile.SafeDirectionTo(potentialTarget.Center) * 11.5f,
                        ModContent.ProjectileType<MK2RocketHoming>(),
                        (int)(projectile.damage * 0.9),
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
                float x = (45f + 35f * (int)(projectile.ai[1] % 10f)) * -player.direction;
                int y = -60 - 50 * (int)(projectile.ai[1] / 10);
                Vector2 distanceToDestination = player.Center - projectile.Center + new Vector2(x, y);
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
                    {
                        projectile.Center = player.Center;
                        projectile.netUpdate = true;
                    }
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
        }

        public override bool CanDamage() => false;
    }
}
