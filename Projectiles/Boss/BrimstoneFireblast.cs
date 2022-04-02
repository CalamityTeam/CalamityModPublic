using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneFireblast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Gigablast");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 50;
            projectile.height = 50;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
                projectile.frame = 0;

            if (projectile.ai[1] == 1f)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 60f, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 60) / 60f), 0f, 1f);

            Lighting.AddLight(projectile.Center, 0.9f * projectile.Opacity, 0f, 0f);

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
            }

            int target = Player.FindClosest(projectile.Center, 1, 1);
            float projSpeed = projectile.velocity.Length();
            Vector2 playerVec = Main.player[target].Center - projectile.Center;
            playerVec.Normalize();
            playerVec *= projSpeed;
            projectile.velocity = (projectile.velocity * 24f + playerVec) / 25f;
            projectile.velocity.Normalize();
            projectile.velocity *= projSpeed;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int drawStart = frameHeight * projectile.frame;
            lightColor.R = (byte)(255 * projectile.Opacity);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180, true);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SCalSounds/BrimstoneGigablastImpact"), projectile.Center);

            if (projectile.ai[1] == 0f)
            {
                float spread = 12f * MathHelper.PiOver2 * 0.01f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 30f;
                double offsetAngle;
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BrimstoneBarrage>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BrimstoneBarrage>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
                    }
                }
            }

            for (int j = 0; j < 2; j++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 50, default, 1f);
            }
            for (int k = 0; k < 20; k++)
            {
                int redFire = Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1.5f);
                Main.dust[redFire].noGravity = true;
                Main.dust[redFire].velocity *= 3f;
                redFire = Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 50, default, 1f);
                Main.dust[redFire].velocity *= 2f;
                Main.dust[redFire].noGravity = true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
