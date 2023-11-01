using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Boss
{
    public class EssenceDust : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.45f, 0f, 0.55f);
            for (int i = 0; i < 5; i++)
            {
                int essenceDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 0.5f);
                Main.dust[essenceDust].noGravity = true;
                Main.dust[essenceDust].velocity *= 0f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
            Projectile.position = Projectile.Center;
            int size = Projectile.ai[0] > 60 ? 192 : 96;
            Projectile.width = Projectile.height = size;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 10; i++)
            {
                int godSlay = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1f);
                Main.dust[godSlay].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[godSlay].scale = 0.5f;
                    Main.dust[godSlay].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 15; j++)
            {
                int godSlay2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[godSlay2].noGravity = true;
                Main.dust[godSlay2].velocity *= 5f;
                godSlay2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1f);
                Main.dust[godSlay2].velocity *= 2f;
            }
            Projectile.Damage();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }
    }
}
