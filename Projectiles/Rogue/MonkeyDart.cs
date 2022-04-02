using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class MonkeyDart : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monkey Dart");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 6;
            drawOffsetX = -10;
            drawOriginOffsetY = 0;
            drawOriginOffsetX = 0;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 40;
            projectile.extraUpdates = 2;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //Code to make it not shoot backwards
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            //Gravity
            projectile.velocity.Y = projectile.velocity.Y + 0.04f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
            //Dust trail
            if (Main.rand.Next(25) == 0)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 21, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = projectile.Center;
                Main.dust[d].noLight = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //Add the venom debuff
            target.AddBuff(BuffID.Venom, 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, projectile.position);
            //Dust splash
            int dustsplash = 0;
            while (dustsplash < 4)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 1, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = projectile.Center;
                dustsplash += 1;
            }

            //Randomly not consume item if it wasnt a stealth strike
            if (Main.rand.Next(4) == 0 && projectile.ai[0] != 1)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, 27, 27, ModContent.ItemType<MonkeyDarts>());
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //This makes the projectile only bounce once
            if (projectile.ai[0] == 1)
            {
                if (projectile.ai[1] >= 1f)
                {
                    projectile.Kill();
                }
                else
                {
                    //Code for bouncing
                    Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
                    Main.PlaySound(SoundID.Dig, projectile.position);
                    if (projectile.velocity.X != oldVelocity.X)
                    {
                        projectile.velocity.X = -oldVelocity.X;
                    }
                    if (projectile.velocity.Y != oldVelocity.Y)
                    {
                        projectile.velocity.Y = -oldVelocity.Y;
                    }
                    projectile.ai[1] += 1f;
                }
                return false;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] == 1)
            {
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
    }
}
