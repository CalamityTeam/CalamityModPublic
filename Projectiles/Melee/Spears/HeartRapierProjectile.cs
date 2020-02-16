using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class HeartRapierProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rapier");
        }

        public override void SetDefaults()
        {
            projectile.width = 44;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 44;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.Calamity().trueMelee = true;
        }
        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1.1f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 12, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 6f)
            {
                projectile.localAI[0] = 0f;
                if (Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y,
                        projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<Feather>(), (int)(projectile.damage * 0.2), 0f, projectile.owner, 0f, 0f);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
                return;
            Player player = Main.player[projectile.owner];
            player.statLife += 5;
            player.HealEffect(5);
        }
    }
}
