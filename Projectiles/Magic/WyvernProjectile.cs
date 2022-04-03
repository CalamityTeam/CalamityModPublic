using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class WyvernProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wyvern");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = 93;
        }

        public override void AI()
        {
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 16, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.timeLeft = 0;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit7, Projectile.Center);
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 16, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            int head = Gore.NewGore(Projectile.Center, Projectile.velocity * 0.8f, Mod.GetGoreSlot("Gores/WyvernWeapons/WyvernHead"));
            Main.gore[head].timeLeft /= 10;
            int body = Gore.NewGore(Projectile.Center, Projectile.velocity * 0.8f, Mod.GetGoreSlot("Gores/WyvernWeapons/WyvernBody"));
            Main.gore[body].timeLeft /= 10;
            int tail = Gore.NewGore(Projectile.Center, Projectile.velocity * 0.8f, Mod.GetGoreSlot("Gores/WyvernWeapons/WyvernTail"));
            Main.gore[tail].timeLeft /= 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
