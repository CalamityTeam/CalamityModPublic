using CalamityMod.Events;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class RavagerFlame : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.extraUpdates = BossRushEvent.BossRushActive ? 2 : 1;
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                int icyFlame = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, 0f, 0f, 100, default, 3f);
                Main.dust[icyFlame].noGravity = true;
                Main.dust[icyFlame].velocity *= 0f;
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
