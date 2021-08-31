using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LeadTomahawkProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/LeadTomahawk";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Tomahawk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1f)
            {
                projectile.penetrate = -1;
            }

            float rotateratio = 0.019f;
            float rotation = (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * rotateratio;
            projectile.rotation += rotation * projectile.direction;

            projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

            if (projectile.ai[0] == 0)
            {
                projectile.damage *= 2;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] == 10)
            {
                projectile.damage /= 2;
            }

            return;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.Next(4) == 0)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<LeadTomahawk>());
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return false;
        }
    }
}
