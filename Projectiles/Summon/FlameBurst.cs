using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Summon
{
    public class FlameBurst : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private float count = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SentryShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
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
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[dust].scale = 0.5f;
                        Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 3f);
                    Main.dust[dust2].noGravity = true;
                    Main.dust[dust2].velocity *= 5f;
                    dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                    Main.dust[dust2].velocity *= 2f;
                }
                count += 1f;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int k = 0; k < 5; k++)
                {
                    int otherDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 0.75f);
                    Main.dust[otherDust].velocity *= 0f;
                }
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(1200f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(potentialTarget.Center) * 20f) / 21f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 246, Projectile.oldVelocity.X * 0f, Projectile.oldVelocity.Y * 0f);
            }
        }
    }
}
