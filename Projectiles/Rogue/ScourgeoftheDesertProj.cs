using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScourgeoftheDesertProj : ModProjectile
    {
        private int StealthDamageCap = 0;
        private int BaseDamage = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourge");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.aiStyle = 113;
            aiType = ProjectileID.BoneJavelin;
            projectile.penetrate = 3;
            projectile.Calamity().rogue = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (StealthDamageCap == 0)
                {
                    BaseDamage = projectile.damage;
                }

                projectile.damage = (int)((BaseDamage * ((StealthDamageCap > 10 ? 10 : StealthDamageCap) * 20) / 100) + BaseDamage); //20% damage boost per hit, max of 200%
                StealthDamageCap++;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (StealthDamageCap == 0)
                {
                    BaseDamage = projectile.damage;
                }

                projectile.damage = (int)((BaseDamage * ((StealthDamageCap > 10 ? 10 : StealthDamageCap) * 20) / 100) + BaseDamage); //20% damage boost per hit, max of 200%
                StealthDamageCap++;
            }
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                projectile.penetrate = 20 - StealthDamageCap;
                projectile.velocity.Y *= 1.025f;
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 32);
            }
            projectile.velocity.X *= 1.025f;
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + MathHelper.PiOver4;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
