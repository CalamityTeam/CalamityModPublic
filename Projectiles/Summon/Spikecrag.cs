using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Spikecrag : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spikecrag");
            Main.projFrames[projectile.type] = 4;
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
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (Main.player[projectile.owner].minionDamage != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Main.player[projectile.owner].minionDamage);
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

            float centerX = projectile.Center.X;
            float centerY = projectile.Center.Y;
            float num474 = 1000f;
            bool homeIn = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false))
                {
                    float num476 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float num477 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                    if (num478 < num474)
                    {
                        num474 = num478;
                        homeIn = true;
                    }
                }
            }
            if (projectile.owner == Main.myPlayer && homeIn)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] -= 1f;
                    return;
                }
                projectile.ai[1] += 1f;
                if ((projectile.ai[1] % 10f) == 0f)
                {
                    int amount = Main.rand.Next(6, 10);
                    for (int i = 0; i < amount; i++)
                    {
                        float velocityX = Main.rand.NextFloat(-10f, 10f);
                        float velocityY = Main.rand.NextFloat(-15f, -8f);
                        Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), velocityX, velocityY, ModContent.ProjectileType<SpikecragSpike>(), (int)((double)projectile.damage * 0.80), 0f, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.penetrate == 0)
            {
                projectile.Kill();
            }
            return false;
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
