using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class VulcanProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public ref float time => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 720;
            Projectile.extraUpdates = 8;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20 * Projectile.MaxUpdates;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            if (time > 120 && Projectile.penetrate > 1)
                Projectile.penetrate = 1;
            
            if (time > 60 && Projectile.extraUpdates > 2 && time % 4 == 0)
                Projectile.extraUpdates--;

            if (Projectile.velocity.Length() < 5)
                Projectile.velocity *= 1.01f;
            if (targetDist < 1400)
            {
                if (true)
                {
                    Particle trail = new CustomSpark(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 6, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 6, 0.15f, Effects.ArsenalEffects.ArsenalGaussColor * 0.9f, new Vector2(1f, 1f), true, true, glowCenterScale: 0.6f, glowOpacity: 0.8f, shrinkSpeed: 0.9f);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
                if (time % 17 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), Effects.ArsenalEffects.ArsenalGaussDust);
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.1f, 0.2f);
                    dust.scale = Main.rand.NextFloat(0.7f, 0.85f);
                    dust.noGravity = true;
                    dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
                    dust.noLightEmittence = time % 9 != 0;
                    dust.fadeIn = 0.3f;
                }
            }

            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player Owner = Main.player[Projectile.owner];
            impactDust();

            Vector2 launchVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float launchPower = 4;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }

        public override void OnKill(int timeLeft)
        {
            impactDust();
        }
        public void impactDust()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquareDust>());
                dust.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.1f) * Main.rand.NextFloat(9f, 19f);
                dust.scale = Main.rand.NextFloat(0.6f, 0.85f);
                dust.noGravity = true;
                dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
                dust.noLightEmittence = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D proj = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanProjectile").Value;
            proj = ModContent.Request<Texture2D>("CalamityMod/Particles/SquareRotated").Value;

            Texture2D square = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSquareParticleThick").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 vel = Projectile.rotation.ToRotationVector2();

            Color drawColor = Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 };
            float drawRotation = Projectile.velocity.ToRotation();
            Vector2 rotationPoint = proj.Size() * 0.5f;

            int rate = 50;

            float shrink = Utils.GetLerpValue(rate - 1, 0, time % rate);
            float fade = Math.Min((float)Math.Pow(Utils.GetLerpValue(0, rate * 0.8f, time % rate, true), 3), (float)Math.Pow(Utils.GetLerpValue(rate - 1, rate * 0.8f, time % 30, true), 3));
            float speed = Utils.GetLerpValue(2, 10, Projectile.extraUpdates);
            Vector2 squash = new Vector2(1 + 2 * speed, 1 - 0.3f * speed);

            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(square, drawPosition, null, drawColor * fade * (1 - speed), drawRotation + MathHelper.PiOver4, square.Size() * 0.5f, Projectile.scale * (0.05f + 0.2f * shrink), SpriteEffects.None);

            
            Main.EntitySpriteDraw(proj, drawPosition, null, drawColor, drawRotation, rotationPoint, squash * Projectile.scale * 0.1f, SpriteEffects.None);
            Main.EntitySpriteDraw(proj, drawPosition, null, Color.White with { A = 0 }, drawRotation, rotationPoint, squash * Projectile.scale * 0.06f, SpriteEffects.None);
            return false;
        }
    }
}
