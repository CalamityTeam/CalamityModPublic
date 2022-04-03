using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class AngelicBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angelic Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            // Hitscan laser with a long range
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 200;

            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            // Actual laser dust
            for (int i = 0; i < 7; ++i)
            {
                int dustType = 262; // Main.rand.NextBool() ? 244 : 246;
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);

                Main.dust[idx].noGravity = true;
                Main.dust[idx].position -= i * 0.1666f * Projectile.velocity;
                Main.dust[idx].velocity *= 1f;
                float scale = Main.rand.NextFloat(0.8f, 1.4f);
                Main.dust[idx].scale = scale;
            }

            // Sparkles "burning off" of the laser beam
            if (Main.rand.NextBool())
            {
                int dustType = Main.rand.NextBool() ? 244 : 246;
                int idx = Dust.NewDust(Projectile.position, 1, 1, dustType);

                Main.dust[idx].noGravity = true;
                float ySpeed = Main.rand.NextFloat(3.0f, 5.6f);
                Main.dust[idx].velocity.Y -= ySpeed;
                float scale = Main.rand.NextFloat(0.4f, 0.8f);
                Main.dust[idx].scale = scale;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}
