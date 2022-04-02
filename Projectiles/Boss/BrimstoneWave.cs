using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneWave : ModProjectile
    {
        private int x;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Flame Skull");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 1200;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(x);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            x = reader.ReadInt32();
        }

        public override void AI()
        {
            x++;
            projectile.velocity.Y = (float)(5D * Math.Sin(x / 5D));

            projectile.frameCounter++;
            if (projectile.frameCounter > 12)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            if (projectile.timeLeft < 30)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 30f, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 1170) / 30f), 0f, 1f);

            Lighting.AddLight(projectile.Center, 0.5f * projectile.Opacity, 0f, 0f);

            if (projectile.velocity.X < 0f)
                projectile.spriteDirection = 1;
            else
                projectile.spriteDirection = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            lightColor.R = (byte)(255 * projectile.Opacity);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
