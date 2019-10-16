using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class GodSlayerBlaze : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blaze");
        }

        public override void SetDefaults()
        {
            projectile.width = 250;
            projectile.height = 250;
            projectile.friendly = true;
            projectile.ignoreWater = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0f, 0.75f);
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
            float scale = 0.7f;
            int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
            if (projectile.ai[1] == 0f)
            {
                scale = 1.5f;
                dustType = 173;
            }
            while ((float)num462 < num461)
            {
                float num463 = (float)Main.rand.Next(-30, 31);
                float num464 = (float)Main.rand.Next(-30, 31);
                float num465 = (float)Main.rand.Next(9, 27);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, scale);
                if (dustType != 173)
                    Main.dust[num467].color = new Color(255, 255, 255, 0);
                Main.dust[num467].noGravity = true;
                Main.dust[num467].position.X = projectile.Center.X;
                Main.dust[num467].position.Y = projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[num467];
                expr_149DF_cp_0.position.X += (float)Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[num467];
                expr_14A09_cp_0.position.Y += (float)Main.rand.Next(-10, 11);
                Main.dust[num467].velocity.X = num463;
                Main.dust[num467].velocity.Y = num464;
                num462++;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[1] == 1f)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            else
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
