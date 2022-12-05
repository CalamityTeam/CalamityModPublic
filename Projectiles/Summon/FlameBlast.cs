using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Summon
{
    public class FlameBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float count = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.SentryShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (count == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 20;
                Projectile.height = 20;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                count += 1f;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 5; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 0.75f);
                    Main.dust[num469].velocity *= 0f;
                }
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(300f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(potentialTarget.Center) * 25f) / 21f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 244, Projectile.oldVelocity.X * 0f, Projectile.oldVelocity.Y * 0f);
            }
        }
    }
}
