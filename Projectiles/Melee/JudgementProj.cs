using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class JudgementProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/JudgementProj";
        public ref float time => ref Projectile.ai[0];
        public float hitboxSize = 10;
        public Color mainColor;
        public float fade = 1;
        public float fadeOut = 1;
        public override void SetDefaults()
        {
            Projectile.width = 336;
            Projectile.height = 274;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 450;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 14;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Vector2 topCorner = Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(100f)) * Projectile.scale) * 137 * Projectile.ai[1];
            Vector2 bottomCorner = Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-100f)) * Projectile.scale) * 137 * Projectile.ai[1];

            if (time == 0)
            {
                Projectile.scale = 0.0875f;
                Projectile.velocity *= 0.9f;
                mainColor = Main.rand.NextBool() ? Color.MediumPurple : Color.MediumOrchid;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (time < 200)
            {
                Projectile.scale += 0.0026f;
                hitboxSize += 0.4525f;
                Projectile.velocity *= 0.995f;
                // Spawn 2
                if (time % 85 == 0 && time > 10)
                {
                    Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(hitboxSize * 0.8f, hitboxSize * 0.8f) - Projectile.velocity * 2, (-Projectile.velocity * 3).RotatedByRandom(0.9f), ModContent.ProjectileType<StarofJudgement>(), (int)(Projectile.damage * 0.35f), 3f, Projectile.owner, 0f);
                    star.scale = Projectile.ai[1];
                }
            }
            else
            {
                Projectile.velocity *= 0.975f;
            }
            if (time > 300 && fade > 0)
                fade -= 0.0065f;

            if (time < 250 && time > 20)
            {
                if (time < 200)
                {
                    if (Projectile.timeLeft % 4 == 0)
                    {
                        SparkParticle spark = new SparkParticle(topCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(185f)), false, 15, 1f * Projectile.ai[1], mainColor * 0.25f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    if (Projectile.timeLeft % 4 == 0)
                    {
                        SparkParticle spark = new SparkParticle(bottomCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-185f)), false, 15, 1f * Projectile.ai[1], mainColor * 0.25f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }

                if (Main.rand.NextBool(3))
                {
                    Dust trailDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(hitboxSize, hitboxSize) * Projectile.ai[1] - Projectile.velocity * 2 * Projectile.ai[1], ModContent.DustType<SquashDust>());
                    trailDust.scale = (Main.rand.NextFloat(0.7f, 0.85f) - (time < 150 ? 0 : time * 0.001f)) * Projectile.ai[1];
                    trailDust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.85f, 1.5f);
                    trailDust.color = Main.rand.NextBool() ? Color.MediumPurple : Color.MediumOrchid;
                    trailDust.noGravity = true;
                    trailDust.fadeIn = Projectile.ai[1] - 1;
                }

                if (Main.rand.NextBool(12))
                {
                    GlowOrbParticle orb = new GlowOrbParticle(topCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(170f)) * Main.rand.NextFloat(10f, 30f), false, 17, Main.rand.NextFloat(0.4f, 0.75f) * Projectile.ai[1], mainColor, true, true);
                    GeneralParticleHandler.SpawnParticle(orb);
                    GlowOrbParticle orb2 = new GlowOrbParticle(bottomCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-170f)) * Main.rand.NextFloat(10f, 30f), false, 17, Main.rand.NextFloat(0.4f, 0.75f) * Projectile.ai[1], mainColor, true, true);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }
            }
            fadeOut = Utils.GetLerpValue(0, 180, Projectile.timeLeft, true);
            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float waveFade = Utils.GetLerpValue(0, 300, Projectile.timeLeft);
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 9 * i, null, mainColor with { A = 0 } * fadeOut * 0.6f, Projectile.rotation, tex.Size() / 2f, new Vector2(1 - (0.2f * waveFade) + 0.01f * i, 1 + (0.45f * waveFade) - 0.06f * i) * Projectile.scale * Projectile.ai[1], SpriteEffects.None, 0);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.88f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize * Projectile.ai[1], targetHitbox);
    }
}
