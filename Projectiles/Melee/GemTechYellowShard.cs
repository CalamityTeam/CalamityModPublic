using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GemTechYellowShard : ModProjectile
    {
        public const int IntangibleFrames = 12;

        public ref float Time => ref Projectile.ai[0];

        public override string Texture => "CalamityMod/Projectiles/Typeless/GemTechYellowGem";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Trireme's Yellow Gem");

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.FinalExtraUpdate())
                Time++;

            if (Projectile.localAI[0] == 0f)
            {
                // Play a shatter sound.
                SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

                // Create a triangular puff of yellow dust.
                Vector2 initialVelocity = (Main.rand.NextFloatDirection() * 0.11f).ToRotationVector2() * Main.rand.NextFloat(2f, 3.3f);
                for (int i = 0; i < 4; i++)
                {
                    Dust crystalShard = Dust.NewDustPerfect(Projectile.Center, 267);
                    crystalShard.velocity = initialVelocity * i / 4f;
                    crystalShard.scale = 1.225f;
                    crystalShard.color = Color.Yellow;
                    crystalShard.noGravity = true;

                    Dust.CloneDust(crystalShard).velocity = initialVelocity.RotatedBy(MathHelper.Pi * 0.666f) * i / 4f;
                    Dust.CloneDust(crystalShard).velocity = initialVelocity.RotatedBy(MathHelper.Pi * -0.666f) * i / 4f;
                }
                Projectile.localAI[0] = 1f;
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi * 0.0005f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Opacity = (float)System.Math.Sqrt(Projectile.timeLeft / 45f);
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool? CanDamage() => Time >= IntangibleFrames;
    }
}
