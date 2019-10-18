using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class CinquedeaProj : ModProjectile
    {
        internal float gravspin = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinquedea");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            drawOriginOffsetY = 11;
            drawOffsetX = -22;

            bool stealthstrike = projectile.ai[1] == 1 && projectile.penetrate == 1;
            if (projectile.spriteDirection == 1)
            {
                gravspin = projectile.velocity.Y * 0.03f;
            }
            if (projectile.spriteDirection == -1)
            {
                gravspin = projectile.velocity.Y * -0.03f;
            }
            projectile.ai[0]++;
            //Fucking slopes
            if (projectile.ai[0] > 2f)
            {
                projectile.tileCollide = true;
            }
            //Face-forward rotation code
            if ((projectile.ai[0] <= 80 && !stealthstrike) || stealthstrike || projectile.velocity.Y <= 0)
            {
                projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
                //Rotating 45 degrees if shooting right
                if (projectile.spriteDirection == 1)
                {
                    projectile.rotation += MathHelper.ToRadians(45f);
                }
                //Rotating 45 degrees if shooting right
                if (projectile.spriteDirection == -1)
                {
                    projectile.rotation -= MathHelper.ToRadians(45f);
                }
            }

            //Stealth strike
            if (stealthstrike)
            {
                float pcx = projectile.Center.X;
                float pcy = projectile.Center.Y;
                float var1 = 800f;
                bool flag = false;
                for (int npcvar = 0; npcvar < 200; npcvar++)
                {
                    if (Main.npc[npcvar].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[npcvar].Center, 1, 1))
                    {
                        float var2 = Main.npc[npcvar].position.X + (Main.npc[npcvar].width / 2);
                        float var3 = Main.npc[npcvar].position.Y + (Main.npc[npcvar].height / 2);
                        float var4 = Math.Abs(projectile.position.X + (projectile.width / 2) - var2) + Math.Abs(projectile.position.Y + (projectile.height / 2) - var3);
                        if (var4 < var1)
                        {
                            var1 = var4;
                            pcx = var2;
                            pcy = var3;
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    float homingstrenght = 7f;
                    Vector2 vector1 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    float var6 = pcx - vector1.X;
                    float var7 = pcy - vector1.Y;
                    float var8 = (float)Math.Sqrt(var6 * var6 + var7 * var7);
                    var8 = homingstrenght / var8;
                    var6 *= var8;
                    var7 *= var8;
                    projectile.velocity.X = (projectile.velocity.X * 20f + var6) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 20f + var7) / 21f;
                }
            }
            //Gravity code
            else
            {
                if (projectile.ai[0] > 80)
                {
                    projectile.velocity.Y = projectile.velocity.Y + 0.15f;
                    if (projectile.velocity.Y > 0)
                    {
                        projectile.rotation += gravspin;
                    }
                    if (projectile.velocity.Y > 10f)
                    {
                        projectile.velocity.Y = 10f;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[1] == 1f && projectile.penetrate == 1)
                projectile.timeLeft = 180;
        }
    }
}
