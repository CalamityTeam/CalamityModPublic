using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Melee
{
    public class BrimlanceHellfireExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 130;
            projectile.height = 130;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.melee = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.75f / 255f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
                projectile.localAI[0] += 1f;
            }
            bool flag15 = false;
            bool flag16 = false;
            if (projectile.velocity.X < 0f && projectile.position.X < projectile.ai[0])
            {
                flag15 = true;
            }
            if (projectile.velocity.X > 0f && projectile.position.X > projectile.ai[0])
            {
                flag15 = true;
            }
            if (projectile.velocity.Y < 0f && projectile.position.Y < projectile.ai[1])
            {
                flag16 = true;
            }
            if (projectile.velocity.Y > 0f && projectile.position.Y > projectile.ai[1])
            {
                flag16 = true;
            }
            if (flag15 && flag16)
            {
                projectile.Kill();
            }
            float num461 = 25f;
            if (projectile.ai[0] > 180f)
            {
                num461 -= (projectile.ai[0] - 180f) / 2f;
            }
            if (num461 <= 0f)
            {
                num461 = 0f;
                projectile.Kill();
            }
            num461 *= 0.7f;
            projectile.ai[0] += 4f;
            int num462 = 0;
            while ((float)num462 < num461)
            {
                float num463 = (float)Main.rand.Next(-10, 11);
                float num464 = (float)Main.rand.Next(-10, 11);
                float num465 = (float)Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num467];
                dust.noGravity = true;
                dust.position.X = projectile.Center.X;
                dust.position.Y = projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = num463;
                dust.velocity.Y = num464;
                num462++;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }
    }
}
