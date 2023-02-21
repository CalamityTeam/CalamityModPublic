using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Melee
{
    public class Exoboom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exoplasma Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 cinderSpawnPosition = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(50f);
                    Vector2 cinderVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(5f, 18f);
                    Color cinderColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityUtils.ExoPalette);
                    SquishyLightParticle cinder = new(cinderSpawnPosition, cinderVelocity, 1.1f, cinderColor, 32, 1f, 4f);
                    GeneralParticleHandler.SpawnParticle(cinder);
                }
                Projectile.localAI[0] = 1f;
            }

            // Create smoke.
            for (int i = 0; i < 2; i++)
            {
                Color smokeColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityUtils.ExoPalette);
                smokeColor = Color.Lerp(smokeColor, Color.Gray, 0.55f);
                HeavySmokeParticle smoke = new(Projectile.Center, Main.rand.NextVector2Circular(27f, 27f), smokeColor, 40, 1.8f, 1f, 0.03f, true, 0.075f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }
    }
}
