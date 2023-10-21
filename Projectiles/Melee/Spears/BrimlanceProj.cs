using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class BrimlanceProj : BaseSpearProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Brimlance>();
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(4))
            {
                int idx = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, Projectile.direction * 2, 0f, 150, default, 1f);
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<BrimlanceHellfireExplosion>()] < 3)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<BrimlanceHellfireExplosion>(), (int)(Projectile.damage * 0.35), hit.Knockback, Main.myPlayer);

                for (int i = 0; i < 2; i++)
                {
                    Vector2 fireVelocity = new Vector2(0f, Main.rand.NextFloat(7f, 10f)).RotatedByRandom(MathHelper.TwoPi);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, fireVelocity, ModContent.ProjectileType<BrimlanceStandingFire>(), (int)(Projectile.damage * 0.25), 0f, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
