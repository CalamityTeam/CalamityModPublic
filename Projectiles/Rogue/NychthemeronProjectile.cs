using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class NychthemeronProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Nychthemeron";

        public static int lifetime = 300;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                // Thrown out and floating in the air
                Projectile.velocity *= 0.99f;
            }
            else
            {
                Player owner = Main.player[Projectile.owner];

                Projectile.tileCollide = false;

                // Recall to the player
                Vector2 toPlayer = owner.Center - Projectile.Center;
                toPlayer.Normalize();
                toPlayer *= Projectile.ai[0];
                Projectile.velocity = toPlayer;

                Projectile.ai[0] += 0.5f;
                Projectile.extraUpdates = 1;
                if (Projectile.ai[0] > 20f)
                {
                    Projectile.ai[0] = 20f;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner && Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation += 0.4f * Projectile.direction * ((float)Projectile.timeLeft / (float)lifetime);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            float minScale = 0.9f;
            float maxScale = 1.1f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                Dust.NewDust(Projectile.position, 4, 4, 236, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Dust.NewDust(Projectile.position, 4, 4, 240, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
            }

            Projectile.Kill();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0)
            {
                float minScale = 0.9f;
                float maxScale = 1.1f;
                int numDust = 2;
                for (int i = 0; i < numDust; i++)
                {
                    Dust.NewDust(Projectile.position, 4, 4, 236, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                    Dust.NewDust(Projectile.position, 4, 4, 240, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                }
            }
        }
    }
}
