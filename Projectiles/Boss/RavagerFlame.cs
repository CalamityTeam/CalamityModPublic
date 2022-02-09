using CalamityMod.Events;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class RavagerFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Flame");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.aiStyle = 1;
			projectile.extraUpdates = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 2 : 1;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

        public override void AI()
        {
            for (int num468 = 0; num468 < 2; num468++)
            {
                int num469 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 100, default, 3f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
