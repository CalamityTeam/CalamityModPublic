using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Spikecrag : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spikecrag");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 42;
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
            if (projectile.frame > 3)
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

            float maxDistance = 1000f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                    if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(projectile.Center, projectile.width, projectile.height, Main.npc[i].Center, Main.npc[i].width, Main.npc[i].height))
                    {
                        homeIn = true;
                        break;
                    }
                }
            }

            if (projectile.owner == Main.myPlayer && homeIn)
            {
                projectile.ai[1] += 1f;
                if ((projectile.ai[1] % 10f) == 0f)
                {
                    int amount = Main.rand.Next(5, 8);
                    for (int i = 0; i < amount; i++)
                    {
                        float velocityX = Main.rand.NextFloat(-10f, 10f);
                        float velocityY = Main.rand.NextFloat(-15f, -8f);
                        Projectile.NewProjectile(projectile.oldPosition.X + (projectile.width / 2), projectile.oldPosition.Y + (projectile.height / 2), velocityX, velocityY, ModContent.ProjectileType<SpikecragSpike>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool CanDamage() => false;
    }
}
