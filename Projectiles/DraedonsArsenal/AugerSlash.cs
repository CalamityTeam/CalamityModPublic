using System;
using CalamityMod.Dusts;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class AugerSlash : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public ref float time => ref Projectile.ai[0];
        public float fade = 1;
        public int lifetime = 25;
        public Player Owner => Main.player[Projectile.owner];
        public float scaleFx => Projectile.ai[2] == 5 ? 2 : 1;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = lifetime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            Projectile.scale = Projectile.localAI[0];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            fade = (float)Math.Pow(Utils.GetLerpValue(lifetime * 0.1f, lifetime * 0.5f, Projectile.timeLeft, true), 4);
            if (Projectile.velocity.Length() > 0.1f)
                Projectile.velocity *= 0.45f;
            int dusts = (int)(30 * scaleFx);
            if (time == 1)
            {
                for (int i = 0; i < dusts; i++)
                {
                    float variance = Utils.GetLerpValue(0, dusts, i, true) * Main.rand.NextFloat(0.9f, 1f);
                    float rot = MathHelper.Lerp(-0.7f * scaleFx, 0.7f * scaleFx, variance) * Projectile.direction;
                    float swingDirScale = Projectile.ai[1] == 1 ? (1 - variance) : variance;
                    float scale = 0.4f + swingDirScale * 2f;
                    float rotScaling = 1 - Math.Abs(rot);
                    Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(rot) * (70 / scaleFx + 80 * (float)Math.Pow(rotScaling, 1.5f) * Main.rand.NextFloat(0.85f, 1f)) * scaleFx;
                    Vector2 finalDustVel = (vel * (float)Math.Pow(rotScaling, 1.5f) + (vel.RotatedBy(-MathHelper.PiOver2 * rot) * 1.5f)) * 0.03f;
                    Dust s = Dust.NewDustPerfect(Projectile.Center + (vel - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 45 * scaleFx) * Projectile.scale, ModContent.DustType<SquashDust>(), finalDustVel / scaleFx);
                    s.scale = scale * scaleFx * Projectile.scale;
                    s.noGravity = true;
                    s.color = Color.Lerp(Effects.ArsenalEffects.ArsenalGaussColor, Color.White, Math.Max(0, swingDirScale - 0.35f));
                    s.fadeIn = 0.1f + swingDirScale * Projectile.scale;
                }
            }

            Lighting.AddLight(Projectile.Center, Color.Lerp(Effects.ArsenalEffects.ArsenalGaussColor, Color.White, 0.3f).ToVector3() * 0.7f * Projectile.scale);
            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && Projectile.numHits > 0)
            {
                Projectile.numHits -= 1;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[2] == 5)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type == ModContent.ProjectileType<AugerPull>() && p.owner == Projectile.owner)
                    {
                        if (p.timeLeft > 2)
                            p.timeLeft = 2;
                    }
                }
            }

            Vector2 launchVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float launchPower = (Projectile.ai[2] == 5 ? 35 : 14);
            target.MoveNPC(launchVel, launchPower, true, Owner);

            if (Projectile.ai[2] == 5)
                modifiers.ApplyScalingForcedCrit(Projectile);

            float minMult = 0.3f;
            int hitsToMinMult = 7;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            if (Projectile.numHits == 0)
            {
                SoundStyle hit = new("CalamityMod/Sounds/Item/AugerHit");
                SoundEngine.PlaySound(hit with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f), MaxInstances = 2 }, target.Center);
            }
            if (Projectile.numHits < 4)
            {
                for (int i = 0; i < (14 - Projectile.numHits * 2); i++)
                {
                    float rot = Main.rand.NextFloat(-0.3f, 0.3f);
                    Dust s = Dust.NewDustPerfect(target.Center, ModContent.DustType<SquashDust>(), Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(rot) * (float)Math.Pow(scaleFx, 1.5f) * Main.rand.NextFloat(8, 12) * (1 - Math.Abs(rot)));
                    s.scale = Main.rand.NextFloat(1.6f, 1.9f) * (1 - Math.Abs(rot));
                    s.noGravity = true;
                    s.color = Color.Lerp(Effects.ArsenalEffects.ArsenalGaussColor, Color.White, Main.rand.NextFloat(0, 0.4f));
                    s.fadeIn = -0.5f;
                    if (i % 3 != 0)
                    {
                        Dust s2 = Dust.NewDustPerfect(target.Center, Effects.ArsenalEffects.ArsenalGaussDust, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.6f) * (float)Math.Pow(scaleFx, 1.5f) * Main.rand.NextFloat(5, 10));
                        s2.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        s2.noGravity = true;
                        s2.color = Effects.ArsenalEffects.ArsenalGaussColor;
                    }
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            Player owner = Main.player[Projectile.owner];

            if (!Collision.CanHitLine(
                    owner.Center, 1, 1,
                    target.Center, 1, 1))
                return false;
            return base.CanHitNPC(target);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft <= 10)
                return false;
            Player Owner = Main.player[Projectile.owner];
            Vector2 start = (time < 2 ? Owner.Center : Projectile.Center);
            float scale = (scaleFx > 1 ? 1.5f : 1);
            float length = 135 * scale;
            float size = 135 * scale;
            float _ = float.NaN;
            bool hit = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, start + Projectile.velocity.SafeNormalize(Vector2.UnitX) * length * Projectile.scale, size * Projectile.scale, ref _);
            return (hit);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D proj = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/AugerSlash").Value;

            Vector2 vel = Projectile.rotation.ToRotationVector2();

            Color drawColor = Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 };
            float drawRotation = Projectile.velocity.ToRotation();

            float lerp = 1 - (float)Math.Pow((Utils.GetLerpValue(lifetime, 0, time, true)), 2f);
            float sizeLerp = (float)Math.Pow((Utils.GetLerpValue(lifetime, lifetime * 0.2f, time, true)), 5f);
            Vector2 squash = new Vector2(1 + lerp * 0.5f, 1.25f - lerp * 1.1f) * 0.08f * sizeLerp;
            Vector2 rotationPoint = new Vector2(proj.Width * 0.85f, proj.Height / 2);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 105 * Projectile.scale;

            SpriteEffects spfx = (Projectile.ai[1] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            if (Projectile.direction == -1)
                spfx = (Projectile.ai[1] == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            Main.EntitySpriteDraw(proj, drawPosition, null, Color.White with { A = 0 } * sizeLerp, drawRotation, rotationPoint, squash * Projectile.scale * scaleFx, spfx);
            return false;
        }
    }
}
