using CalamityMod.Events;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class BossRushFailureEffectThing : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
        }

		public override void AI()
		{
            MoonlordDeathDrama.RequestLight(Utils.InverseLerp(0f, 8f, Time, true), Main.LocalPlayer.Center);
            if (Time >= 45f)
            {
                Main.PlaySound(SoundID.DD2_EtherianPortalOpen, Main.LocalPlayer.Center);
                BossRushEvent.End();
                projectile.Kill();
            }
            Time++;
        }
	}
}
