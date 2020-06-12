using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class SausageMakerSpear : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            projectile.width = 44;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 42;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            //projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1.1f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 5, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 1.2f, projectile.velocity.Y * 1.2f, ModContent.ProjectileType<Blood2>(), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
