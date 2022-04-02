using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class GastricBelcherVomit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gastric Vomit");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = projectile.height = 18;
            projectile.minion = true;
            projectile.timeLeft = 300;
            projectile.penetrate = 3;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            //Rotation and gravity
            projectile.rotation += 0.6f * projectile.direction;
            projectile.velocity.Y += 0.27f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 233, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.10f, 150, default, 0.9f);
                splash += 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture;
            switch (projectile.ai[0])
            {
                case 1f: texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/GastricBelcherVomit2");
                         break;
                case 2f: texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/GastricBelcherVomit3");
                         break;
                default: texture = Main.projectileTexture[projectile.type];
                         break;
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
