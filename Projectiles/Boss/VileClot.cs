using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class VileClot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.light = 0.6f;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            if (Projectile.velocity.Length() < 12f)
                Projectile.velocity *= 1.01f;

            int vileDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y),
                Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default, 1.5f);
            Main.dust[vileDust].noGravity = true;

            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.CursedInferno, 60);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 6; i++)
            {
                int killDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, default, 2.5f);
                Main.dust[killDust].noGravity = true;
                Dust dust = Main.dust[killDust];
                dust.velocity *= 2f;
                killDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
                dust = Main.dust[killDust];
                dust.velocity *= 2f;
            }
        }
    }
}
