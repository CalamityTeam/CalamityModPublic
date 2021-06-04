using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ApothJaws : ModProjectile
    {
        private const float degrees = (float)(Math.PI / 180) * 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jaws of Annihilation");
        }

        public override void SetDefaults()
        {
            projectile.width = 144;
            projectile.height = 72;
            projectile.alpha = 70;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.light = 1.5f;
        }

        public override void AI()
        {
            if (projectile.timeLeft % 8 == 0)
            {
                double angle = (double)Main.rand.Next(360) * Math.PI / 180;
                float offsetX = projectile.position.X + (float)Main.rand.Next((int)projectile.width);
                float offsetY = projectile.position.Y + (float)Main.rand.Next((int)projectile.height);
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(offsetX, offsetY, 14 * (float)Math.Cos(angle), 14 * (float)Math.Sin(angle), ModContent.ProjectileType<ApothChloro>(), projectile.damage, projectile.knockBack / 2, projectile.owner);
                }
            }
            if (projectile.timeLeft < 30)
                projectile.alpha = projectile.alpha + 6;
            else if (projectile.timeLeft < 210)
            {
                projectile.velocity.X *= 0.9f;
                projectile.velocity.Y *= 0.9f;
            }
            else if (projectile.timeLeft < 240)
            {
                if (projectile.ai[1] == 0)
                    projectile.rotation += 1.3f * degrees;
                else
                    projectile.rotation -= 1.3f * degrees;
            }
            else if (projectile.timeLeft == 240)
            {
                if (projectile.ai[1] == 0)
                    projectile.rotation = projectile.ai[0] - 30 * degrees;
                else
                {
                    projectile.rotation = projectile.ai[0] + 30 * degrees + (float)Math.PI;
                    projectile.spriteDirection = -1;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 600, true);
            target.AddBuff(ModContent.BuffType<DemonFlames>(), 600, true);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 600, true);
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30, true);
        }
    }
}
