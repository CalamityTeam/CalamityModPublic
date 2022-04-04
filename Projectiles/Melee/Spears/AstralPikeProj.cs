using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class AstralPikeProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pike");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            Projectile.aiStyle = 19;
            Projectile.DamageType = DamageClass.Melee;  //Dictates whether projectile is a melee-class weapon.
            Projectile.timeLeft = 90;
            Projectile.height = 40;  //The height of the .png file in pixels divided by 2.
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            target.immune[Projectile.owner] = 6;
            if (crit)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile star = CalamityUtils.ProjectileBarrage(Projectile.Center, target.Center, Main.rand.NextBool(), 800f, 800f, 800f, 800f, 10f, ModContent.ProjectileType<AstralStar>(), (int)(Projectile.damage * 0.4), 1f, Projectile.owner, true);
                        if (star.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            star.Calamity().forceMelee = true;
                            star.ai[0] = 3f;
                        }
                    }
                }
            }
        }
    }
}
