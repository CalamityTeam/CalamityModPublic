using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class LanternSoul : ModProjectile
    {
        public float count = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lantern");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 0;
            }

            Player player = Main.player[projectile.owner];
            if (count == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                count += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            projectile.velocity = new Vector2(0f, (float)Math.Sin((double)(6.28318548f * projectile.ai[0] / 300f)) * 0.5f);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 300f)
            {
                projectile.ai[0] = 0f;
                projectile.netUpdate = true;
            }

            int num = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<LanternFlame>())
                {
                    num++;
                }
            }
            if (num < 20)
            {
                projectile.ai[1] += (float)Main.rand.Next(2,6) + 1f;
                if (projectile.ai[1] >= 75f)
                {
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                    if (projectile.owner == Main.myPlayer)
                    {
                        float startOffsetX = Main.rand.NextFloat(15f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                        float startOffsetY = Main.rand.NextFloat(15f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                        Vector2 startPos = new Vector2(projectile.position.X + startOffsetX, projectile.position.Y + startOffsetY);
                        Vector2 speed = new Vector2(0f, 0f);
                        Projectile.NewProjectile(startPos, speed, ModContent.ProjectileType<LanternFlame>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
