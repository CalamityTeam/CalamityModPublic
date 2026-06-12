using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GrandGuardianBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Color mainColor = Color.White;
        public int time = 0;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 35;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.scale = Projectile.ai[0];
            if (mainColor == Color.White)
            {
                mainColor = Main.rand.NextBool() ? Color.Cyan : Color.DarkOrchid;
                Particle bolt2 = new CustomPulse(Projectile.Center, Vector2.Zero, mainColor * 0.45f, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 2.56f * Projectile.scale, 35);
                GeneralParticleHandler.SpawnParticle(bolt2);
                Particle bolt3 = new CustomPulse(Projectile.Center, Vector2.Zero, (mainColor == Color.Cyan ? Color.Orchid : Color.Cyan) * 0.65f, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 1.26f * Projectile.scale, 15);
                GeneralParticleHandler.SpawnParticle(bolt3);

                for (int i = 0; i < 25; i++)
                {
                    Vector2 dustVel = new Vector2(7, 7).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.8f) * 1;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel * 9 * Projectile.scale, Main.rand.NextBool(4) ? 264 : 66, dustVel * 4, 0, default, Main.rand.NextFloat(0.8f, 1.4f));
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool() ? Color.Lerp(mainColor, Color.White, 0.5f) : mainColor;

                    bool color = Main.rand.NextBool();
                    GenericSparkle sparker = new GenericSparkle(Projectile.Center + dustVel * 9 * Projectile.scale, dustVel * 4, color ? Color.Cyan : Color.DarkOrchid, color ? Color.DarkOrchid : Color.Cyan, Main.rand.NextFloat(0.6f, 0.8f) * Projectile.scale, 11, Main.rand.NextFloat(-0.1f, 0.1f), 2.68f);
                    GeneralParticleHandler.SpawnParticle(sparker);
                }
            }
            if (time == 6)
            {
                //Particle bolt2 = new CustomPulse(Projectile.Center, Vector2.Zero, mainColor * 0.65f, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-10f, 10f), 2.56f, 3.26f, 24);
                //GeneralParticleHandler.SpawnParticle(bolt2);
            }
            time++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.5f * Projectile.scale, targetHitbox);
    }
}
