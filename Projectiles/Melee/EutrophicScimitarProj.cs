using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class EutrophicScimitarProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scimitar Zap");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.melee = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(127, 255, 253);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Eutrophication>(), 60);
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.35f / 255f, (255 - projectile.alpha) * 0.35f / 255f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 2)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }
            if (projectile.velocity.X < 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            }
            else
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            }
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 226, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.5f);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust dust;
                Vector2 position = new Vector2(projectile.position.X - 4, projectile.position.Y - 4);
                dust = Main.dust[Terraria.Dust.NewDust(position, 58, 58, 226, 0f, 0f, 0, new Color(255, 255, 255), 0.4605263f)];
                dust.noGravity = true;
                dust.fadeIn = 0.9473684f;
            }
        }
    }
}
