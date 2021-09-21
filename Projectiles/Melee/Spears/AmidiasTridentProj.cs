using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class AmidiasTridentProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trident");
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.aiStyle = 19;
            projectile.melee = true;
            projectile.timeLeft = 90;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.hide = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1f;
        public override float ForwardSpeed => 0.75f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X * 0.8f, projectile.velocity.Y * 0.8f, ModContent.ProjectileType<AmidiasWhirlpool>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
        };
    }
}
