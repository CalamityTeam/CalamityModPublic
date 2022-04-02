using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class PolypLauncherSentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polyp Launcher");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 25;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            projectile.velocity.Y += 0.5f;
            if (projectile.velocity.Y > 10f)
            {
                projectile.velocity.Y = 10f;
            }

            projectile.StickToTiles(false, false);

            if (projectile.ai[0] > 0f)
            {
                projectile.ai[0] -= 1f;
                return;
            }
            projectile.ai[1] += Main.rand.Next(1,3);

            NPC potentialTarget = projectile.Center.MinionHoming(800f, player, false);

            if (projectile.owner == Main.myPlayer && potentialTarget != null)
            {
                if (projectile.ai[1] > 40f)
                {
                    Vector2 spawnPosition = new Vector2(projectile.oldPosition.X + (projectile.width / 2), projectile.oldPosition.Y + (projectile.height / 2));

                    float shootSpeed = 16f;
                    float gravity = -PolypLauncherProjectile.Gravity;
                    float distance = Vector2.Distance(spawnPosition, potentialTarget.Center);
                    float angle = 0.25f * (float)Math.Asin(MathHelper.Clamp(gravity * distance * 1.5f / (float)Math.Pow(shootSpeed, 2), -1f, 1f));

                    Vector2 velocity = new Vector2(0f, -shootSpeed).RotatedBy(angle).RotatedByRandom(0.1f);
                    velocity.X *= (potentialTarget.Center.X - projectile.Center.X < 0).ToDirectionInt();

                    Projectile.NewProjectile(spawnPosition, velocity, ModContent.ProjectileType<PolypLauncherProjectile>(), projectile.damage, projectile.knockBack, projectile.owner);
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool CanDamage() => false;
    }
}
