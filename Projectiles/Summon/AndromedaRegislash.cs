using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class AndromedaRegislash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regislash");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 582;
            Projectile.height = 304;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.light = 3f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                if (CalamityUtils.CountProjectiles(Projectile.type) > 1)
                {
                    Projectile.Kill();
                    return;
                }
                SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Projectile.Center);
                Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
                Projectile.localAI[0] = 1f;
            }
            Projectile.position = player.Center - Projectile.Size / 2f;
            if (Math.Abs(Math.Cos(Projectile.rotation)) > 0.675f)
            {
                Projectile.position.X += Math.Sign(Math.Cos(Projectile.rotation)) * 295f;
            }
            Projectile.position.Y += (float)Math.Sin(Projectile.rotation) * 325f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 7 == 6)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
            }
            Projectile.direction = ((player.Center.X - Projectile.Center.X) < 0).ToDirectionInt();
            Projectile.spriteDirection = Projectile.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 startPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(texture, startPos, rectangle, Projectile.GetAlpha(lightColor), rotation, origin, scale, spriteEffects, 0f);
            return false;
        }
    }
}
