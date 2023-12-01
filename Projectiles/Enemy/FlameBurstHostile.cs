using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Enemy
{
    public class FlameBurstHostile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float count = 0;

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (count == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 20;
                Projectile.height = 20;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                for (int i = 0; i < 10; i++)
                {
                    int flameDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[flameDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[flameDust].scale = 0.5f;
                        Main.dust[flameDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    int sparkDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 3f);
                    Main.dust[sparkDust].noGravity = true;
                    Main.dust[sparkDust].velocity *= 5f;
                    sparkDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                    Main.dust[sparkDust].velocity *= 2f;
                }
                count += 1f;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int k = 0; k < 5; k++)
                {
                    int sparky = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 0.75f);
                    Main.dust[sparky].velocity *= 0f;
                }
            }
            int playerTarget = (int)Player.FindClosest(Projectile.Center, 1, 1);
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] < 200f && Projectile.ai[1] > 40f)
            {
                float scaleFactor = Projectile.velocity.Length();
                Vector2 playerDistance = Main.player[playerTarget].Center - Projectile.Center;
                playerDistance.Normalize();
                playerDistance *= scaleFactor;
                Projectile.velocity = (Projectile.velocity * 24f + playerDistance) / 25f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= scaleFactor;
            }
            if (Projectile.ai[0] < 0f)
            {
                if (Projectile.velocity.Length() < 18f)
                {
                    Projectile.velocity *= 1.02f;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 246, Projectile.oldVelocity.X * 0f, Projectile.oldVelocity.Y * 0f);
            }
        }
    }
}
