using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class SwordsplosionPurple : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Sword");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 300;
            aiType = ProjectileID.EnchantedBeam;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.3f / 255f, (255 - projectile.alpha) * 0.4f / 255f, (255 - projectile.alpha) * 1f / 255f);
            if (projectile.localAI[1] > 7f)
            {
                int num308 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, 100, 255), 1.2f);
                Main.dust[num308].velocity *= 0.1f;
                Main.dust[num308].noGravity = true;
            }
            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.Atan(90);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 100, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 295)
                return false;

            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 7; k++)
            {
                int num308 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, 0f, 0f, 150, new Color(Main.DiscoR, 100, 255), 1.2f);
                Main.dust[num308].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }
    }
}
