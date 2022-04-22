using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class UrchinSpearProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;  //The width of the .png file in pixels divided by 2.
            Projectile.aiStyle = 19;
            Projectile.DamageType = DamageClass.Melee;  //Dictates whether this is a melee-class weapon.
            Projectile.timeLeft = 90;
            Projectile.height = 56;  //The height of the .png file in pixels divided by 2.
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y,
                        Projectile.velocity.X * 1.6f, Projectile.velocity.Y * 1.6f, ModContent.ProjectileType<UrchinSpikeFugu>(), Projectile.damage, Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
        };

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
