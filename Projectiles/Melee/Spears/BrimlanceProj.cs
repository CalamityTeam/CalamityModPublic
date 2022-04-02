using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class BrimlanceProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlance");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 40;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.hide = true;
            projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(4))
            {
                int idx = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, projectile.direction * 2, 0f, 150, default, 1f);
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<BrimlanceHellfireExplosion>(), (int)(projectile.damage * 0.65), knockback, Main.myPlayer);
                for (int i = 0; i < 3; i++)
                {
                    Vector2 fireVelocity = new Vector2(0f, Main.rand.NextFloat(7f, 10f)).RotatedByRandom(MathHelper.TwoPi);
                    Projectile.NewProjectile(target.Center, fireVelocity, ModContent.ProjectileType<BrimlanceStandingFire>(), (int)(projectile.damage * 0.4), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
