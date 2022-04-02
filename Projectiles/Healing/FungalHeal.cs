using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class FungalHeal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.extraUpdates = 10;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                Player player = Main.player[projectile.owner];
                if ((player.ActiveItem().summon &&
                    !player.ActiveItem().melee &&
                    !player.ActiveItem().ranged &&
                    !player.ActiveItem().magic &&
                    !player.ActiveItem().Calamity().rogue) ||
                    player.ActiveItem().hammer > 0 ||
                    player.ActiveItem().pick > 0 ||
                    player.ActiveItem().axe > 0)
                {
                    projectile.timeLeft = 600;
                }
                projectile.localAI[0] += 1f;
            }

            projectile.HealingProjectile((int)projectile.ai[1], (int)projectile.ai[0], 5f, 15f);
            float num494 = projectile.velocity.X * 0.334f;
            float num495 = -(projectile.velocity.Y * 0.334f);
            int num496 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 56, 0f, 0f, 100, default, 0.5f);
            Dust dust = Main.dust[num496];
            dust.noGravity = true;
            dust.position.X -= num494;
            dust.position.Y -= num495;
            float num498 = projectile.velocity.X * 0.2f;
            float num499 = -(projectile.velocity.Y * 0.2f);
            int num500 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 56, 0f, 0f, 100, default, 0.7f);
            Dust dust2 = Main.dust[num500];
            dust2.noGravity = true;
            dust2.position.X -= num498;
            dust2.position.Y -= num499;
        }
    }
}
