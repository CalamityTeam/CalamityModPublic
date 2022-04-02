using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SeasSearingSecondary : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydroshower");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.scale <= 3.6f)
            {
                projectile.scale *= 1.01f;
                projectile.width = (int)(16f * projectile.scale);
                projectile.height = (int)(32f * projectile.scale);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 3; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 217, 0f, 0f, 100, new Color(60, Main.DiscoG, 190), projectile.scale);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                    int num470 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 202, 0f, 0f, 100, new Color(60, Main.DiscoG, 190), projectile.scale);
                    Main.dust[num470].noGravity = true;
                    Main.dust[num470].velocity *= 0f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int endoftime = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<TyphoonBubble>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[endoftime].localAI[1] = 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }
    }
}
