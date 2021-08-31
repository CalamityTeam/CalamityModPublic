using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AbyssBallVolley2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyss Ball Volley");
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 26;
            projectile.height = 26;
            projectile.hostile = true;
            projectile.penetrate = 1;
			projectile.alpha = 60;
			projectile.tileCollide = false;
            projectile.timeLeft = 300;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

        public override void AI()
        {
			if (projectile.timeLeft < 60)
				projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 60f, 0f, 1f);

			if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 33);
            }

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 127, 0f, 0f);
                Main.dust[dust].noGravity = true;
            }
        }

		public override bool CanHitPlayer(Player target) => projectile.timeLeft >= 60;

		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.timeLeft < 60)
				return;

			target.AddBuff(BuffID.Darkness, 120);
        }
    }
}
