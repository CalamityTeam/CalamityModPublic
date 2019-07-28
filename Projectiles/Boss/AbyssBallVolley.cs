using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AbyssBallVolley : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Abyss Ball Volley");
		}

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.hostile = true;
            projectile.alpha = 60;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
        	if (projectile.ai[1] == 0f)
        	{
        		projectile.ai[1] = 1f;
        		Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 33);
        	}
            if (Main.rand.Next(2) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, 0f, 0f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, 0f, 0f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(BuffID.Weak, 120);
        }
    }
}
