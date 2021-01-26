using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ExcelsusBlue : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Excelsus");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 300;
            projectile.alpha = 100;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 16f && projectile.timeLeft > 85)
            {
                projectile.velocity *= 1.05f;
            }
            if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) > 0f && projectile.timeLeft <= 85)
            {
                projectile.velocity *= 0.98f;
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.02f;
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.tileCollide = false;
            if (projectile.timeLeft > 85)
            {
                projectile.timeLeft = 85;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return default(Color);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color;
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                color = new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            else
            {
                color = new Color(255, 255, 255, 100);
            }
            Vector2 origin = new Vector2(39f, 46f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/ExcelsusBlueGlow"), projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.timeLeft > 85)
            {
                projectile.timeLeft = 85;
            }
            target.AddBuff(BuffID.Frostburn, 600);
        }
    }
}
