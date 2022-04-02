using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class InfernadoFriendly : ModProjectile
    {
        bool intersectingSomething = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernado");
            Main.projFrames[projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 88;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.timeLeft = 500;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                intersectingSomething = true;

            float scaleBase = 44f;
            float scaleMult = 2.5f;
            float baseWidth = 320f;
            float baseHeight = 88f;

            if (Main.rand.NextBool(25))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (projectile.velocity.X != 0f)
            {
                projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 2)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                projectile.scale = (scaleBase - projectile.ai[1]) * scaleMult / scaleBase;
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(baseWidth * projectile.scale), (int)(baseHeight * projectile.scale));
                projectile.netUpdate = true;
            }
            if (projectile.ai[1] != -1f)
            {
                projectile.scale = (scaleBase - projectile.ai[1]) * scaleMult / scaleBase;
                projectile.width = (int)(baseWidth * projectile.scale);
                projectile.height = (int)(baseHeight * projectile.scale);
            }
            if (!intersectingSomething)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 100)
                {
                    projectile.alpha = 100;
                }
            }
            else
            {
                projectile.alpha += 30;
                if (projectile.alpha > 200)
                {
                    projectile.alpha = 200;
                }
            }
            if (projectile.ai[0] > 0f)
            {
                projectile.ai[0] -= 1f;
            }
            if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
            {
                projectile.netUpdate = true;
                Vector2 center = projectile.Center;
                center.Y -= baseHeight * projectile.scale / 2f;
                float num618 = (scaleBase - projectile.ai[1] + 1f) * scaleMult / scaleBase;
                center.Y -= baseHeight * num618 / 2f;
                center.Y += 2f;
                Projectile segment = Projectile.NewProjectileDirect(center, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10f, projectile.ai[1] - 1f);
                //Defaults to magic
                if (segment.whoAmI.WithinBounds(Main.maxProjectiles))
                {
                    segment.Calamity().forceMelee = projectile.Calamity().forceMelee;
                    segment.Calamity().forceRanged = projectile.Calamity().forceRanged;
                    segment.Calamity().forceMinion = projectile.Calamity().forceMinion;
                    segment.Calamity().forceRogue = projectile.Calamity().forceRogue;
                    segment.Calamity().forceTypeless = projectile.Calamity().forceTypeless;
                    segment.Calamity().forceHostile = projectile.Calamity().forceHostile;
                }
            }
            if (projectile.ai[0] <= 0f)
            {
                float num622 = 0.104719758f;
                float num623 = (float)projectile.width / 5f;
                num623 *= 2f;
                float num624 = (float)(Math.Cos((double)(num622 * -(double)projectile.ai[0])) - 0.5) * num623;
                projectile.position.X -= num624 * -projectile.direction;
                projectile.ai[0] -= 1f;
                num624 = (float)(Math.Cos((double)(num622 * -(double)projectile.ai[0])) - 0.5) * num623;
                projectile.position.X += num624 * -projectile.direction;
                return;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (!intersectingSomething)
            {
                return new Color(95, 95, 19, 255 - projectile.alpha);
            }
            return new Color(64, 64, 13, 255 - projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
