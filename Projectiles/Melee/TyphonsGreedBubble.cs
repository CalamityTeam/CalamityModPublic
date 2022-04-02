using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TyphonsGreedBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Typhon's Greed");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            float inertia = 50f * projectile.ai[1]; //100
            float speed = 10f * projectile.ai[1]; //5
            float num954 = 40f;
            if (Math.Abs(projectile.velocity.X) > 0.2f)
            {
                projectile.spriteDirection = -projectile.direction;
            }
            projectile.rotation = projectile.velocity.ToRotation();
            Lighting.AddLight(projectile.Center, 0f, 0.1f, 0.7f);

            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead)
            {
                if (projectile.Distance(player.Center) > num954)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(player.Center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * (inertia - 1f) + moveDirection * speed) / inertia;
                }
            }
            else
            {
                if (projectile.timeLeft > 30)
                {
                    projectile.timeLeft = 30;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)),
                projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item21, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X -= (float)(projectile.width / 2);
            projectile.position.Y -= (float)(projectile.height / 2);
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int num194 = 0; num194 < 6; num194++)
            {
                int num195 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 186, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 186, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 2;
            projectile.Damage();
        }
    }
}
