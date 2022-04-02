using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class EarthenPikeSpear : BaseSpearProjectile
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
            projectile.scale = 1.5f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 32, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 6f)
            {
                projectile.localAI[0] = 0f;
                if (Main.myPlayer == projectile.owner)
                {
                    float velocityY = projectile.velocity.Y * 1.25f;
                    if (velocityY < 0.1f)
                        velocityY = 0.1f;
                    int proj = Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y,
                        projectile.velocity.X * 1.25f, velocityY, ModContent.ProjectileType<FossilShard>(), (int)(projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceMelee = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
        }
    }
}
