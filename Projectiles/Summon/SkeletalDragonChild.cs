using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SkeletalDragonChild : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Child");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 48;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 7;
            projectile.minionSlots = 0f;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
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

            if (projectile.ai[0] < 0 || projectile.ai[0] >= Main.projectile.Length)
            {
                projectile.Kill();
                return;
            }

            Projectile mother = Main.projectile[(int)projectile.ai[0]];

            if (!mother.active || mother.type != ModContent.ProjectileType<SkeletalDragonMother>())
            {
                projectile.Kill();
                return;
            }

            NPC target = projectile.Center.MinionHoming(SkeletalDragonMother.DistanceToCheck * 0.666f, player);

            if (target != null)
            {
                projectile.ai[1]++;
                if (projectile.ai[1] % 55f == 54f && Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(target.Center) * 19f, ModContent.ProjectileType<BloodSpit>(), mother.damage, mother.knockBack, mother.owner);
                }
            }

            // If the mother is dive-bombing
            if (mother.ai[1] == 1f)
            {
                if (target != null)
                {
                    if (projectile.Distance(mother.Center) > 620f)
                    {
                        projectile.velocity = (mother.Center - projectile.Center) / 45f;
                    }
                    else
                    {
                        projectile.velocity = (projectile.velocity * 19f + projectile.SafeDirectionTo(mother.Center) * 18f) / 20f;
                    }
                }
                else
                {
                    if (projectile.Distance(mother.Center) > 250f)
                    {
                        projectile.velocity = (mother.Center - projectile.Center) / 28f;
                    }
                    else
                    {
                        projectile.velocity += new Vector2(Math.Sign(mother.Center.X - projectile.Center.X), Math.Sign(mother.Center.Y - projectile.Center.Y)) * new Vector2(0.04f, 0.0225f);
                        if (projectile.velocity.Length() > 8f)
                        {
                            projectile.velocity *= 8f / projectile.velocity.Length();
                        }
                    }
                }
            }
            else
            {
                if (projectile.Distance(mother.Center) > 250f)
                {
                    projectile.velocity = (mother.Center - projectile.Center) / 28f;
                }
                else
                {
                    projectile.velocity += new Vector2(Math.Sign(mother.Center.X - projectile.Center.X), Math.Sign(mother.Center.Y - projectile.Center.Y)) * new Vector2(0.04f, 0.0225f);
                    if (projectile.velocity.Length() > 8f)
                    {
                        projectile.velocity *= 8f / projectile.velocity.Length();
                    }
                }
            }

            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();

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
    }
}
