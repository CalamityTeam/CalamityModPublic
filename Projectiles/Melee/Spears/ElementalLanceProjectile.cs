using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class ElementalLanceProjectile : BaseSpearProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ElementalLance>();
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.DamageType = DamageClass.Melee;  //Dictates whether this is a melee-class weapon.
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1.1f;
        public override float ForwardSpeed => 0.6f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<SpatialSpear>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        };
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
            {
                int idx = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 66, Projectile.direction * 2, 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[idx].noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 60);
        }
    }
}
