using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Enemy
{
    public class SulphuricAcidBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 0.01f;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0f;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 6)
                Projectile.frame = 0;

            if (Projectile.localAI[1] < 1f)
            {
                Projectile.localAI[1] += 0.01f;
                Projectile.scale += 0.01f;
                Projectile.width = (int)(30f * Projectile.scale);
                Projectile.height = (int)(30f * Projectile.scale);
            }
            else
            {
                Projectile.damage = 20;
                Projectile.width = 30;
                Projectile.height = 30;
                Projectile.tileCollide = true;
            }

            if (Projectile.localAI[0] > 2f)
            {
                Projectile.Opacity += 0.08f;
                if (Projectile.Opacity > 0.6f)
                    Projectile.Opacity = 0.6f;
            }
            else
                Projectile.localAI[0] += 1f;

            if (Projectile.ai[1] > 30f)
            {
                if (Projectile.velocity.Y > -2f)
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.05f;
            }
            else
                Projectile.ai[1] += 1f;

            if (Projectile.wet)
            {
                if (Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y = Projectile.velocity.Y * 0.98f;
                if (Projectile.velocity.Y > -1f)
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.2f;
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.localAI[1] >= 1f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.localAI[1] < 1f)
                return;

            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
            Projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 60;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);

            for (int num246 = 0; num246 < 25; num246++)
            {
                int num247 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 0, default, 1f);
                Main.dust[num247].position = (Main.dust[num247].position + Projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = 255 - (int)(Projectile.Opacity * 255f);
            }
        }
    }
}
