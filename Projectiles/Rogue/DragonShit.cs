using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DragonShit : ModProjectile
    {
        public NPC target;
        public Vector2 rotationVector = Vector2.UnitY * -13f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 66;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 420;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            target = projectile.Center.ClosestNPCAt(1200f);
            if (projectile.localAI[0] == 0f)
            {
                projectile.ai[0] = Utils.SelectRandom(Main.rand, -1f, 1f);
                projectile.localAI[0] = 1f;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (projectile.timeLeft >= 380)
            {
                projectile.velocity *= 1.07f;
            }
            else if (target != null)
            {
                projectile.velocity = (projectile.velocity * 23f + projectile.DirectionTo(target.Center) * 14.975f) / 24f;
            }
            else
            {
                projectile.timeLeft = Math.Min(projectile.timeLeft, 15);
                projectile.alpha += 17;
                projectile.velocity = rotationVector;
                rotationVector = rotationVector.RotatedBy(MathHelper.ToRadians(14.975f * projectile.ai[0]));
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D projectileTexture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(projectileTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, projectileTexture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)projectileTexture.Width / 2f, (float)frameHeight / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 80);
            for (int d = 0; d < 5; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }
			CalamityUtils.ExplosionGores(projectile.Center, 3);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
        }
    }
}
