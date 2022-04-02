using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ClamorNoctusWyvern : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wyvern");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.magic = true;
            projectile.aiStyle = 93;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 126, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.owner == Main.myPlayer)
            {
                projectile.timeLeft = 0;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit7, projectile.Center);
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 126, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            int head = Gore.NewGore(projectile.Center, projectile.velocity * 0.8f,  mod.GetGoreSlot("Gores/WyvernWeapons/ClamorNoctusHead"));
            Main.gore[head].timeLeft /= 10;
            int body = Gore.NewGore(projectile.Center, projectile.velocity * 0.8f, mod.GetGoreSlot("Gores/WyvernWeapons/ClamorNoctusBody"));
            Main.gore[body].timeLeft /= 10;
            int tail = Gore.NewGore(projectile.Center, projectile.velocity * 0.8f,  mod.GetGoreSlot("Gores/WyvernWeapons/ClamorNoctusTail"));
            Main.gore[tail].timeLeft /= 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
