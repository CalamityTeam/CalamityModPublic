using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShockGrenadeProjectile : ModProjectile
    {
        public static int spriteWidth = 14;
        public static int spriteHeight = 30;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shock Grenade");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            if (projectile.velocity.Y >= 16f)
            {
                projectile.velocity.Y = 16f;
            }

            projectile.rotation += projectile.direction * 0.2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);

            projectile.localAI[0] = 0;
            projectile.localAI[1] = 0;

            if (projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X < 0)
                {
                    projectile.localAI[0] = 1;
                }
                if (oldVelocity.X > 0)
                {
                    projectile.localAI[0] = -1;
                }
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y < 0)
                {
                    projectile.localAI[1] = 1;
                }
                if (oldVelocity.Y > 0)
                {
                    projectile.localAI[1] = -1;
                }
            }
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Rogue/ShockGrenadeProjectileGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 94, 0.75f, 0f);

            int boltCount = Main.rand.Next(5, 11);
            for (int i = 0; i < boltCount; i++)
            {
                int boltScatter = 1;
                Vector2 boltVelocity = new Vector2(Main.rand.NextFloat(-boltScatter, boltScatter), Main.rand.NextFloat(-boltScatter * 2, boltScatter * 2));
                if (projectile.localAI[0] != 0)
                {
                    boltVelocity.X *= -1;
                }
                if (projectile.localAI[1] != 0)
                {
                    boltVelocity.Y *= -1;
                }
                boltVelocity.X += projectile.localAI[0];
                boltVelocity.Y += projectile.localAI[1] * 2;
                boltVelocity.Normalize();
                boltVelocity *= 10;

                int boltType = Main.rand.Next(0, 2);
                int boltDamage = projectile.damage / 2;

                int boltAI = 0;
                if (projectile.Calamity().stealthStrike)
                {
                    boltAI = 1;
                }

                Projectile.NewProjectile(projectile.Center, boltVelocity, ModContent.ProjectileType<ShockGrenadeBolt>(), boltDamage, 0, projectile.owner, boltType, boltAI);
            }

            if (projectile.Calamity().stealthStrike)
            {
                int auraDamage = projectile.damage / 4;
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockTeslaAura>(), auraDamage, 1, projectile.owner, 0, 0);

                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 93, 0.5f, 0f);
            }
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockGrenadeExplosion>(), projectile.damage, 3, projectile.owner, 0, 0);
        }
    }
}
