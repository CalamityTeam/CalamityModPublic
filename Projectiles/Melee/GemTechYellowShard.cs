using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GemTechYellowShard : ModProjectile
    {
        public const int IntangibleFrames = 12;

        public ref float Time => ref projectile.ai[0];

        public override string Texture => "CalamityMod/ExtraTextures/GemTechArmor/YellowGem";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Trireme's Yellow Gem");

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.MaxUpdates = 2;
            projectile.timeLeft = 180;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.FinalExtraUpdate())
                Time++;

            if (Time >= IntangibleFrames)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(750f);

                if (potentialTarget != null)
                    projectile.velocity = (projectile.velocity * 24f + projectile.SafeDirectionTo(potentialTarget.Center) * 9f) / 25f;
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            // Play a shatter sound.
            Main.PlaySound(SoundID.Item27, projectile.Center);

            // Create a triangular puff of yellow dust.
            Vector2 initialVelocity = Vector2.UnitX * Main.rand.NextFloat(2f, 3.3f);
            for (int i = 0; i < 4; i++)
            {
                Dust crystalShard = Dust.NewDustPerfect(projectile.Center, 267);
                crystalShard.velocity = initialVelocity * i / 4f;
                crystalShard.scale = 1.225f;
                crystalShard.color = Color.Yellow;
                crystalShard.noGravity = true;

                Dust.CloneDust(crystalShard).velocity = initialVelocity.RotatedBy(MathHelper.Pi * 0.666f) * i / 4f;
                Dust.CloneDust(crystalShard).velocity = initialVelocity.RotatedBy(MathHelper.Pi * -0.666f) * i / 4f;
            }
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool CanDamage() => Time >= IntangibleFrames;
    }
}
