using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CinquedeaProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Cinquedea";

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
                //Rotating 45 degrees if shooting left
                if (projectile.spriteDirection == -1)
                {
                    projectile.rotation -= MathHelper.ToRadians(45f);
                }
            }

            //Stealth strike
            if (stealthstrike)
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 250f, 7f, 20f);
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
            Main.PlaySound(SoundID.Dig, projectile.position);
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[1] == 1f && projectile.penetrate == 1)
                projectile.timeLeft = 180;
        }
    }
}
