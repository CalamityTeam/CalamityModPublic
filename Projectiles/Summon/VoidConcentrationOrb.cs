using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    class VoidConcentrationOrb : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Orb");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 50;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;

            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.scale = 0.01f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.scale < 1f)
                return false;
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int d = 0; d < 6; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[shadow].scale = 0.5f;
                    Main.dust[shadow].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 10; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 3f);
                Main.dust[shadow].noGravity = true;
                Main.dust[shadow].velocity *= 5f;
                shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 2f;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[projectile.owner];
            NPC target = CalamityUtils.MinionHoming(projectile.Center, 1800f, owner);

            if (projectile.scale >= 1f)
            {
                if (target != null)
                {
                    float num550 = 40f;
                    Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num551 = target.Center.X - vector43.X;
                    float num552 = target.Center.Y - vector43.Y;
                    float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                    if (num553 < 100f)
                    {
                        num550 = 28f; //14
                    }
                    num553 = num550 / num553;
                    num551 *= num553;
                    num552 *= num553;
                    projectile.velocity.X = (projectile.velocity.X * 25f + num551) / 26f;
                    projectile.velocity.Y = (projectile.velocity.Y * 25f + num552) / 26f;
                    if (Main.rand.NextBool(5))
                        projectile.velocity *= 1.1f;
                }

            }
            else
            {
                projectile.scale += 0.025f;
                projectile.velocity *= 1.03f;
            }

            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type] - 1)
            {
                projectile.frame = 0;
            }
            projectile.frameCounter++;

            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 4;
            }
        }
    }
}
