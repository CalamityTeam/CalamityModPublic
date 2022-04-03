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
            Projectile.width = 70;
            Projectile.aiStyle = 19;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 90;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1f;
        public override float ForwardSpeed => 0.75f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y, Projectile.velocity.X * 0.8f, Projectile.velocity.Y * 0.8f, ModContent.ProjectileType<AmidiasWhirlpool>(), Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
        };
    }
}
