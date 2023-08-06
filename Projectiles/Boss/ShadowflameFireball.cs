using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class ShadowflameFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.8f;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.scale = 1.25f;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            // Main chunky dark purple dust at the front of the fireball
            for (int i = 0; i < 2; ++i)
            {
                int dustType = 27;
                float dustScale = Main.rand.NextFloat(1.4f, 2.4f);
                int dustID = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType);
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].velocity = Projectile.velocity;
                Main.dust[dustID].scale = dustScale;
            }

            // Trailing brighter purple fire trail dust
            int dustType2 = 70;
            float velMult = Main.rand.NextFloat(0.05f, 0.6f);
            float dustScale2 = Main.rand.NextFloat(1.2f, 1.8f);
            int dustID2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType2);
            Main.dust[dustID2].noGravity = true;
            Main.dust[dustID2].velocity *= 0.1f;
            Main.dust[dustID2].velocity += Projectile.velocity * velMult;
            Main.dust[dustID2].scale = dustScale2;

            Projectile.rotation += 0.3f * (float)Projectile.direction;

            if (Projectile.ai[1] == 1f)
            {
                int num103 = (int)Player.FindClosest(Projectile.Center, 1, 1);
                Vector2 vector11 = Main.player[num103].Center - Projectile.Center;
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] >= 60f)
                {
                    if (Projectile.ai[0] < 240f)
                    {
                        float scaleFactor2 = Projectile.velocity.Length();
                        vector11.Normalize();
                        vector11 *= scaleFactor2;
                        Projectile.velocity = (Projectile.velocity * 24f + vector11) / 25f;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= scaleFactor2;
                    }
                    else if (Projectile.velocity.Length() < 18f)
                    {
                        Projectile.tileCollide = true;
                        Projectile.velocity *= 1.02f;
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Shadowflame>(), 180, true);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            int killDust = 20;
            for (int i = 0; i < killDust; ++i)
            {
                int dustType = Main.rand.NextBool() ? 70 : 27;
                float dustScale = Main.rand.NextFloat(1f, 1.6f);
                int dustID = Dust.NewDust(Projectile.Center, 1, 1, dustType);
                Main.dust[dustID].velocity *= 4f;
                Main.dust[dustID].scale = dustScale;
            }
        }
    }
}
