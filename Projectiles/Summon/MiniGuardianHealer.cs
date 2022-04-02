using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianHealer : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Healer Guardian");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.tileCollide = false;
            projectile.width = 68;
            projectile.height = 82;
            projectile.friendly = true;
            projectile.minionSlots = 0f;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.timeLeft *= 5;
        }

        public override bool? CanCutTiles()
        {
            CalamityPlayer modPlayer = Main.player[projectile.owner].Calamity();
            bool psa = modPlayer.pArtifact && !modPlayer.profanedCrystal;
            if (!psa && !modPlayer.profanedCrystalBuffs)
                return false;
            return null;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.gHealer = false;
            }
            if (modPlayer.gHealer)
            {
                projectile.timeLeft = 2;
            }
            if (!modPlayer.pArtifact || player.dead)
            {
                projectile.active = false;
                return;
            }
            projectile.MinionAntiClump();
            Player projOwner = Main.player[projectile.owner];
            float num16 = 0.5f;
            projectile.tileCollide = false;
            int num17 = 100;
            Vector2 vector3 = projectile.Center;
            float num18 = projOwner.Center.X - vector3.X;
            float num19 = projOwner.Center.Y - vector3.Y;
            num19 += (float)Main.rand.Next(-10, 21);
            num18 += (float)Main.rand.Next(-10, 21);
            num18 += (float)(60 * -(float)projOwner.direction);
            num19 -= 60f;
            float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
            float num21 = 18f;

            if (num20 < (float)num17 && projOwner.velocity.Y == 0f &&
                projectile.position.Y + (float)projectile.height <= projOwner.position.Y + (float)projOwner.height &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }
            if (num20 > 2000f)
            {
                projectile.position = projOwner.position;
                projectile.netUpdate = true;
            }
            if (num20 < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.90f;
                }
                num16 = 0.01f;
            }
            else
            {
                if (num20 < 100f)
                {
                    num16 = 0.1f;
                }
                if (num20 > 300f)
                {
                    num16 = 1f;
                }
                num20 = num21 / num20;
                num18 *= num20;
                num19 *= num20;
            }

            if (projectile.velocity.X < num18)
            {
                projectile.velocity.X += num16;
                if (num16 > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X += num16;
                }
            }
            if (projectile.velocity.X > num18)
            {
                projectile.velocity.X -= num16;
                if (num16 > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X -= num16;
                }
            }
            if (projectile.velocity.Y < num19)
            {
                projectile.velocity.Y += num16;
                if (num16 > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y += num16 * 2f;
                }
            }
            if (projectile.velocity.Y > num19)
            {
                projectile.velocity.Y -= num16;
                if (num16 > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y -= num16 * 2f;
                }
            }
            if (projectile.velocity.X > 0.25f)
            {
                projectile.direction = -1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.direction = 1;
            }

            if (Math.Abs(projectile.velocity.X) > 0.2f)
            {
                projectile.spriteDirection = -projectile.direction;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }
    }
}
