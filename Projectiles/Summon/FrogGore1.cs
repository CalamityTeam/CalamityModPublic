using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FrogGore1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gore from a not frog of the explosive variety");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < 10f)
            {
                Projectile.alpha = 255 - (int)(255 * Projectile.ai[0] / 10f);
            }
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += Projectile.velocity.X * 0.03f;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(4, 8 + 1); i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                    (int)CalamityDusts.SulfurousSeaAcid,
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 4f));
            }
        }
    }
}
