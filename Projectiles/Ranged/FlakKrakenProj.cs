using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kraken");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 54;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.scale = 0.002f;
            projectile.timeLeft = 36000;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            if (projectile.type != ModContent.ProjectileType<FlakKrakenProj>() ||
                !Main.projectile[(int)projectile.ai[1]].active ||
                Main.projectile[(int)projectile.ai[1]].type != ModContent.ProjectileType<FlakKrakenGun>())
            {
                projectile.Kill();
                return;
            }


            // This code uses player-specific fields (such as the mouse), and does not need to be run for anyone
            // other than its owner.
            if (Main.myPlayer != projectile.owner)
                return;

            projectile.rotation += 0.2f;
            if (projectile.localAI[0] < 1f)
            {
                projectile.localAI[0] += 0.002f;
                projectile.scale += 0.002f;
                projectile.width = projectile.height = (int)(50f * projectile.scale);
            }
            else
            {
                projectile.width = projectile.height = 50;
            }
            Player player = Main.player[projectile.owner];
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
            }
            vector2 += new Vector2(num78, num79);
            float speed = 30f;
            float speedScale = 3f;
            Vector2 vectorPos = projectile.Center;
            if (Vector2.Distance(vector2, vectorPos) < 90f)
            {
                speed = 10f;
                speedScale = 1f;
            }
            if (Vector2.Distance(vector2, vectorPos) < 30f)
            {
                speed = 3f;
                speedScale = 0.3f;
            }
            if (Vector2.Distance(vector2, vectorPos) < 10f)
            {
                speed = 1f;
                speedScale = 0.1f;
            }
            float num678 = vector2.X - vectorPos.X;
            float num679 = vector2.Y - vectorPos.Y;
            float num680 = (float)Math.Sqrt((double)(num678 * num678 + num679 * num679));
            num680 = speed / num680;
            num678 *= num680;
            num679 *= num680;
            if (projectile.velocity.X < num678)
            {
                projectile.velocity.X = projectile.velocity.X + speedScale;
                if (projectile.velocity.X < 0f && num678 > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X + speedScale;
                }
            }
            else if (projectile.velocity.X > num678)
            {
                projectile.velocity.X = projectile.velocity.X - speedScale;
                if (projectile.velocity.X > 0f && num678 < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X - speedScale;
                }
            }
            if (projectile.velocity.Y < num679)
            {
                projectile.velocity.Y = projectile.velocity.Y + speedScale;
                if (projectile.velocity.Y < 0f && num679 > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + speedScale;
                }
            }
            else if (projectile.velocity.Y > num679)
            {
                projectile.velocity.Y = projectile.velocity.Y - speedScale;
                if (projectile.velocity.Y > 0f && num679 < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - speedScale;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * projectile.localAI[0]);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 200, 50, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
