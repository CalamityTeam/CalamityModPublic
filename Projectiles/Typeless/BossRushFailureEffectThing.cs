using CalamityMod.Events;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class BossRushFailureEffectThing : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            MoonlordDeathDrama.RequestLight(Utils.InverseLerp(0f, 8f, Time, true), Main.LocalPlayer.Center);
            if (Time >= 45f)
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Main.LocalPlayer.Center);
                BossRushEvent.End();
                Projectile.Kill();
            }
            Time++;
        }
    }
}
