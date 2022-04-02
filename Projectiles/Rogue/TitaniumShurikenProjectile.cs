using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class TitaniumShurikenProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TitaniumShuriken";

        private static float RotationIncrement = 0.22f;
        private static float ReboundTime = 26f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shuriken");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                drawOffsetX = -11;
                drawOriginOffsetY = -10;
                drawOriginOffsetX = 0;

                // ai[0] stores whether the knife is returning. If 0, it isn't. If 1, it is.
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] >= ReboundTime)
                    {
                        projectile.ai[0] = 1f;
                        projectile.ai[1] = 0f;
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    projectile.tileCollide = false;
                    float returnSpeed = 16f;
                    float acceleration = 3.2f;
                    Player owner = Main.player[projectile.owner];

                    // Delete the shuriken if it's excessively far away.
                    Vector2 playerCenter = owner.Center;
                    float xDist = playerCenter.X - projectile.Center.X;
                    float yDist = playerCenter.Y - projectile.Center.Y;
                    float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                    if (dist > 3000f)
                        projectile.Kill();

                    dist = returnSpeed / dist;
                    xDist *= dist;
                    yDist *= dist;

                    // Home back in on the player.
                    if (projectile.velocity.X < xDist)
                    {
                        projectile.velocity.X = projectile.velocity.X + acceleration;
                        if (projectile.velocity.X < 0f && xDist > 0f)
                            projectile.velocity.X += acceleration;
                    }
                    else if (projectile.velocity.X > xDist)
                    {
                        projectile.velocity.X = projectile.velocity.X - acceleration;
                        if (projectile.velocity.X > 0f && xDist < 0f)
                            projectile.velocity.X -= acceleration;
                    }
                    if (projectile.velocity.Y < yDist)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + acceleration;
                        if (projectile.velocity.Y < 0f && yDist > 0f)
                            projectile.velocity.Y += acceleration;
                    }
                    else if (projectile.velocity.Y > yDist)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - acceleration;
                        if (projectile.velocity.Y > 0f && yDist < 0f)
                            projectile.velocity.Y -= acceleration;
                    }

                    // Delete the projectile if it touches its owner.
                    if (Main.myPlayer == projectile.owner)
                        if (projectile.Hitbox.Intersects(owner.Hitbox))
                        {
                            projectile.Kill(); //boomerangs return to you so you get a refund
                            Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<TitaniumShuriken>());
                        }
                }

                // Rotate the shuriken as it flies.
                projectile.rotation += RotationIncrement;
                return;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                return false;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            if (projectile.Calamity().stealthStrike)
            {
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            }
            else
            {
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2) && !projectile.Calamity().stealthStrike)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<TitaniumShuriken>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer && projectile.Calamity().stealthStrike)
            {
                int projNumber = Main.rand.Next(1,3);
                for (int index2 = 0; index2 < projNumber; index2++)
                {
                    float xVector = (float)Main.rand.Next(-35, 36) * 0.02f;
                    float yVector = (float)Main.rand.Next(-35, 36) * 0.02f;
                    xVector *= 10f;
                    yVector *= 10f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, xVector, yVector, ModContent.ProjectileType<TitaniumClone>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer && projectile.Calamity().stealthStrike)
            {
                int projNumber = Main.rand.Next(1,3);
                for (int index2 = 0; index2 < projNumber; index2++)
                {
                    float xVector = (float)Main.rand.Next(-35, 36) * 0.02f;
                    float yVector = (float)Main.rand.Next(-35, 36) * 0.02f;
                    xVector *= 10f;
                    yVector *= 10f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, xVector, yVector, ModContent.ProjectileType<TitaniumClone>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
