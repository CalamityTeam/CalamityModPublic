using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneHellblast2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Hellblast");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.penetrate = -1;
            projectile.timeLeft = 1500;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            // Cal Clone bullet hell projectiles accelerate after a certain time has passed
            if (projectile.ai[0] == 2f && Main.expertMode && projectile.timeLeft < 1260)
            {
                if (projectile.velocity.Length() < ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 10f : 8f))
                    projectile.velocity *= 1.01f;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter >= 10)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            if (projectile.timeLeft < 60)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 60f, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 1440) / 60f), 0f, 1f);

            Lighting.AddLight(projectile.Center, 0.9f * projectile.Opacity, 0f, 0f);

            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = (float)Math.Atan2(-projectile.velocity.Y, -projectile.velocity.X);
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int drawStart = frameHeight * projectile.frame;
            lightColor.R = (byte)(255 * projectile.Opacity);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            if (projectile.ai[0] == 0f)
            {
                target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120);
            }
            else
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
