using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class AstralPikeProj : BaseSpearProjectile
    {
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            if (hit.Crit)
            {
                var source = Projectile.GetSource_FromThis();
                for (int i = 0; i < 3; i++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile star = CalamityUtils.ProjectileBarrage(source, Projectile.Center, target.Center, Main.rand.NextBool(), 800f, 800f, 800f, 800f, 10f, ModContent.ProjectileType<AstralStar>(), (int)(Projectile.damage * 0.4), 1f, Projectile.owner, true);
                        if (star.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            star.DamageType = DamageClass.Melee;
                            star.ai[0] = 3f;
                        }
                    }
                }
            }
        }
    }
}
