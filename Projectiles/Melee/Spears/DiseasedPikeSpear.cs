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
            projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether projectile is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 40;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 4; i++)
                {
                    int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity * 0.5f, ModContent.ProjectileType<PlagueSeeker>(), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner);
                    Main.projectile[proj].extraUpdates += i;
                }
            }
        }
    }
}
