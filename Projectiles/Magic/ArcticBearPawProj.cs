using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ArcticBearPawProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.coldDamage = true;
            projectile.penetrate = 5;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.coldDamage = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arctic Bear Paw");
        }

        public override void AI()
        {
            //make pretty dust
            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 88);
            Main.dust[index2].noGravity = true;

            if (projectile.velocity.X > -0.05f && projectile.velocity.X < 0.05f &
                projectile.velocity.Y > -0.05f && projectile.velocity.Y < 0.05f)
            {
                projectile.Kill();
            }
            else
            {
                projectile.velocity *= 0.968f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Confused, Main.rand.Next(60, 240));
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }
    }
}
