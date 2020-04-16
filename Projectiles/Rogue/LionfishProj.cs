using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LionfishProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lionfish");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            int num982 = 25;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= num982;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 45f)
                {
                    float num986 = 0.98f;
                    float num987 = 0.35f;
                    projectile.ai[1] = 45f;
                    projectile.velocity.X = projectile.velocity.X * num986;
                    projectile.velocity.Y = projectile.velocity.Y + num987;
                    if (projectile.velocity.X < 0f)
                    {
                        projectile.spriteDirection = -1;
                        projectile.rotation = (float)Math.Atan2((double)-(double)projectile.velocity.Y, (double)-(double)projectile.velocity.X);
                    }
                    else
                    {
                        projectile.spriteDirection = 1;
                        projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
                    }
                }
            }
            //Sticky Behaviour
            CalamityUtils.StickyProjAI(projectile, 15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            CalamityUtils.ModifyHitNPCSticky(projectile, 6, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 72;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num193 = 0; num193 < 3; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int num194 = 0; num194 < 30; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.KingSlime || target.type == NPCID.WallofFlesh || target.type == NPCID.WallofFleshEye ||
                target.type == NPCID.SkeletronHead || target.type == NPCID.SkeletronHand)
            {
                target.buffImmune[BuffID.Venom] = false;
            }
            target.AddBuff(BuffID.Venom, 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
        }
    }
}
