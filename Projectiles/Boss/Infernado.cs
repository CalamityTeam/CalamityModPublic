using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.Yharon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class Infernado : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/Flarenado";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernado");
            Main.projFrames[projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 320;
            projectile.height = 88;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.timeLeft = 720;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            float scaleBase = 36f;
            float scaleMult = 1.5f;
            float baseWidth = 320f;
            float baseHeight = 88f;

            if (projectile.velocity.X != 0f)
            {
                projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 2)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                projectile.scale = (scaleBase - projectile.ai[1]) * scaleMult / scaleBase;
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(baseWidth * projectile.scale), (int)(baseHeight * projectile.scale));
                projectile.netUpdate = true;
            }
            if (projectile.ai[1] != -1f)
            {
                projectile.scale = (scaleBase - projectile.ai[1]) * scaleMult / scaleBase;
                projectile.width = (int)(baseWidth * projectile.scale);
                projectile.height = (int)(baseHeight * projectile.scale);
            }

            if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 60)
                {
                    projectile.alpha = 60;
                }
            }
            else
            {
                projectile.alpha += 30;
                if (projectile.alpha > 150)
                {
                    projectile.alpha = 150;
                }
            }
            if (projectile.ai[0] > 0f)
            {
                projectile.ai[0] -= 1f;
            }

            if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
            {
                projectile.netUpdate = true;
                Vector2 center = projectile.Center;
                center.Y -= baseHeight * projectile.scale / 2f;
                float num618 = (scaleBase - projectile.ai[1] + 1f) * scaleMult / scaleBase;
                center.Y -= baseHeight * num618 / 2f;
                center.Y += 2f;
                Projectile.NewProjectile(center, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 11f, projectile.ai[1] - 1f);
            }
            if (projectile.ai[0] <= 0f)
            {
                float num622 = 0.104719758f;
                float num623 = (float)projectile.width / 5f;
                num623 *= 2f;
                float num624 = (float)(Math.Cos((double)(num622 * -(double)projectile.ai[0])) - 0.5) * num623;
                projectile.position.X -= num624 * -projectile.direction;
                projectile.ai[0] -= 1f;
                num624 = (float)(Math.Cos((double)(num622 * -(double)projectile.ai[0])) - 0.5) * num623;
                projectile.position.X += num624 * -projectile.direction;
            }

            if (projectile.timeLeft == 600)
                projectile.damage = projectile.GetProjectileDamage(ModContent.NPCType<Yharon>());
        }

        public override bool CanHitPlayer(Player target) => projectile.timeLeft <= 600;

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 53, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.timeLeft <= 600)
                target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 420);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
