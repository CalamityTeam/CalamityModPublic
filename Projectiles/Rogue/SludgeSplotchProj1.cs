using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SludgeSplotchProj1 : ModProjectile
    {
        public static int sludgeDustType = 191;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sludge Splotch");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, sludgeDustType, 0f, 0f, 225, new Color(255, 255, 255), 3);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].velocity = Main.dust[dust].velocity * 0.25f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Slimed, 120);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(BuffID.Slimed, 120);

        public override void Kill(int timeLeft)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 sparkVelocity = CalamityUtils.RandomVelocity(100f, 40f, 60f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkVelocity, ModContent.ProjectileType<SludgeSplotchProj2>(), Projectile.damage, 0, Projectile.owner);
                }
            }

            SoundEngine.PlaySound(SoundID.NPCDeath9 with { Volume = SoundID.NPCDeath9.Volume * 2 }, Projectile.position);

            int numDust = 20;
            float spread = 3f;
            for (int i = 0; i < numDust; i++)
            {
                Vector2 velocity = Projectile.velocity + new Vector2(Main.rand.NextFloat(-spread, spread), Main.rand.NextFloat(-spread, spread));

                int dust = Dust.NewDust(Projectile.Center, 1, 1, sludgeDustType, velocity.X, velocity.Y, 175, default, 3f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
