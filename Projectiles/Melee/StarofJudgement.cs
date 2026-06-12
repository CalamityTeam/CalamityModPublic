using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using CalamityMod.Particles;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Melee
{
    public class StarofJudgement : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/StarofJudgement";
        public ref float time => ref Projectile.ai[0];
        public Color mainColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hitboxSize = Projectile.width * Projectile.scale;
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize, targetHitbox);
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (time == 0)
            {
                mainColor = Main.rand.NextBool() ? Color.MediumPurple : Color.MediumOrchid;
                if (Projectile.numHits == 0)
                    Projectile.scale *= (Projectile.ai[2] == 1 ? 0.75f : 0.5f);
                if (Projectile.ai[2] == 1)
                {
                    Projectile.localNPCHitCooldown = -1;
                    Projectile.penetrate = 1;
                    Projectile.extraUpdates = 1;
                }
            }
            Projectile.rotation += (Projectile.velocity.X + MathF.Abs(Projectile.velocity.Y) * Projectile.direction) * 0.01f;

            if (Projectile.ai[2] == 1)
            {
                if (time > 18)
                {
                    CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 25, MathHelper.Clamp(30 - time, 15, 30));
                }
                else
                    Projectile.velocity = Projectile.velocity.RotatedBy(0.085f * Projectile.ai[1]);
                if (time % 2 == 0)
                {
                    Particle spark = new CustomSpark(Projectile.Center, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 13, 0.35f, mainColor * 0.75f, new Vector2(0.8f, 1.35f) * Projectile.scale, true, false, shrinkSpeed: 0.3f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            if (Projectile.ai[2] != 1 && time > 10)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 700f * (Projectile.numHits > 1 ? 2 : 1), 20, 20f * (Projectile.numHits > 1 ? 5 : 1));
                if (Main.rand.NextBool(3))
                {
                    Dust trailDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5) - Projectile.velocity, ModContent.DustType<SquashDust>());
                    trailDust.scale = Main.rand.NextFloat(0.9f, 1.05f) * Projectile.scale;
                    trailDust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.5f);
                    trailDust.color = Main.rand.NextBool() ? Color.MediumPurple : Color.MediumOrchid;
                    trailDust.noGravity = true;
                    trailDust.fadeIn = Projectile.scale - 1;
                }
            }

            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/StarofJudgement").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.Lerp(mainColor, Color.White, 0.2f) with { A = 0 };
            float drawRotation = Projectile.rotation;
            Vector2 rotationPoint = texture.Size() * 0.5f;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], drawColor * 0.5f, 1, texture, true, true);
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, drawPosition, null, Color.White with { A = 0 }, drawRotation, rotationPoint, Projectile.scale * 0.8f, SpriteEffects.None);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.CanBeChasedBy())
                Projectile.penetrate++;

            if (Projectile.ai[2] == 0)
            {
                Projectile.timeLeft = 180;
                time = 0;
                Projectile.velocity *= 1.1f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[2] == 1)
            {
                int points = 5;
                float radians = MathHelper.TwoPi / points;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f).RotatedByRandom(100));
                for (int k = 0; k < points; k++)
                {
                    Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);
                    Particle spark = new PointParticle((Projectile.Center + velocity * 7.5f), velocity * 5.5f, false, 9, 1.2f, mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            else
            {
                for (int k = 0; k < 3; k++)
                {
                    GlowOrbParticle orb = new GlowOrbParticle(Projectile.Center, new Vector2(4, 4).RotatedByRandom(100f) * Main.rand.NextFloat(0.3f, 0.8f), false, 15, Main.rand.NextFloat(0.6f, 0.95f), mainColor);
                    GeneralParticleHandler.SpawnParticle(orb);
                }
            }
        }
    }
}
