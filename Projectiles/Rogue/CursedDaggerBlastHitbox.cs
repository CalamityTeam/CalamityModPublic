using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Utilities;
using CalamityMod.Items.Weapons.Rogue;
using static Humanizer.In;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using System.Reflection.Metadata;

namespace CalamityMod.Projectiles.Rogue
{
    public class CursedDaggerBlastHitbox : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 30 == 0)
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFurySwing with { Volume = 1.2f }, Projectile.position);

            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(200f, 200f), 75);
            dust2.scale = Main.rand.NextFloat(0.8f, 1.1f);
            dust2.noGravity = true;

            int sparkCount = Main.rand.Next(18);
            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < sparkCount; i++)
            {
                float angle = i / (float)sparkCount * MathHelper.TwoPi + offset;
                Vector2 sparkPos = Projectile.Center + angle.ToRotationVector2() * Main.rand.Next(65, 200);
                int sparkLifetime = Main.rand.Next(10, 18);
                float sparkScale = Main.rand.NextFloat(0.8f, 1f) * 0.955f;
                Color sparkColor = Color.Lerp(Color.LawnGreen, Color.Green, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.LawnGreen, Main.rand.NextFloat());
                SparkParticle spark = new SparkParticle(sparkPos, (angle - MathHelper.PiOver2 * Projectile.direction).ToRotationVector2() * Main.rand.NextFloat(2f, 5.5f), false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 600);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width, targetHitbox);
    }
}
