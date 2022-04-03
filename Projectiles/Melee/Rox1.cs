using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class Rox1 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.13f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            //Projectile rotation
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
            //Dust trail
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 191, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 160, default, 0.7f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Changes the texture of the projectile
            if (Projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Rox2");
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 20)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), Projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            if (Projectile.ai[0] == 2f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Rox3");
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 20)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), Projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 191, -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f, 120, default, 1.2f);
            }
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }
    }
}
