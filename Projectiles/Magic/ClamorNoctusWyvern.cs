using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class ClamorNoctusWyvern : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = ProjAIStyleID.Nail;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 126, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
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

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit7, Projectile.Center);
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 126, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            if (Main.netMode != NetmodeID.Server)
            {
                int head = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.8f, Mod.Find<ModGore>("ClamorNoctusHead").Type);
                Main.gore[head].timeLeft /= 10;
                int body = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.8f, Mod.Find<ModGore>("ClamorNoctusBody").Type);
                Main.gore[body].timeLeft /= 10;
                int tail = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.8f, Mod.Find<ModGore>("ClamorNoctusTail").Type);
                Main.gore[tail].timeLeft /= 10;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
