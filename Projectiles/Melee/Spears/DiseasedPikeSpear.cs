using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class DiseasedPikeSpear : BaseSpearProjectile
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
            if (Main.rand.NextBool(4))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 107, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 7;
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 4; i++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.5f, ModContent.ProjectileType<PlagueSeeker>(), (int)(Projectile.damage * 0.75), Projectile.knockBack, Projectile.owner);
                    Main.projectile[proj].extraUpdates += i;
                }
            }
        }
    }
}
