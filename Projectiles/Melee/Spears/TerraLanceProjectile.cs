using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class TerraLanceProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lance");
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
            //projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y,
                                projectile.velocity.X * 1.4f, projectile.velocity.Y * 1.4f, ModContent.ProjectileType<TerraSpear>(), (int)(projectile.damage * 0.7), projectile.knockBack * 0.7f, projectile.owner, 0f, 0f);
        };
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
        }
    }
}
