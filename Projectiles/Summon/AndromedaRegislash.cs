using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class AndromedaRegislash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regislash");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 582;
            projectile.height = 304;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.light = 3f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                if (CalamityUtils.CountProjectiles(projectile.type) > 1)
                {
                    projectile.Kill();
                    return;
                }
                Main.PlaySound(SoundID.DD2_DrakinShot, projectile.Center);
                projectile.rotation = projectile.AngleTo(Main.MouseWorld);
                projectile.localAI[0] = 1f;
            }
            projectile.position = player.Center - projectile.Size / 2f;
            if (Math.Abs(Math.Cos(projectile.rotation)) > 0.675f)
            {
                projectile.position.X += Math.Sign(Math.Cos(projectile.rotation)) * 295f;
            }
            projectile.position.Y += (float)Math.Sin(projectile.rotation) * 325f;
            projectile.frameCounter++;
            if (projectile.frameCounter % 7 == 6)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.Kill();
            }
            projectile.direction = ((player.Center.X - projectile.Center.X) < 0).ToDirectionInt();
            projectile.spriteDirection = projectile.direction;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 startPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            float rotation = projectile.rotation;
            float scale = projectile.scale;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(texture, startPos, rectangle, projectile.GetAlpha(lightColor), rotation, origin, scale, spriteEffects, 0f);
            return false;
        }
    }
}
