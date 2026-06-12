using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class AugerPull : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public float sine = 0;
        public float shrink = 1;
        public int strongTimer = 0;
        public bool strong => strongTimer < 10;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.scale = Projectile.ai[0];
            if (strongTimer == 0)
            {
                Particle bolt2 = new CustomPulse(Projectile.Center, Vector2.Zero, Effects.ArsenalEffects.ArsenalGaussColor, "CalamityMod/Particles/GlowSquareParticleBig", Vector2.One, 0, 1.1f * Projectile.scale, 0.7f * Projectile.scale, 30);
                GeneralParticleHandler.SpawnParticle(bolt2);
                for (int i = 0; i < 20; i++)
                {
                    Vector2 dustVel2 = (Vector2.UnitX).RotatedByRandom(100) * Main.rand.NextFloat(10f, 15.5f) * Projectile.scale;
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dustVel2.SafeNormalize(Vector2.UnitX) * 360, ModContent.DustType<SquashDust>(), -dustVel2 * 1.5f, 0, default, Main.rand.NextFloat(1.2f, 1.8f));
                    dust2.noGravity = true;
                    dust2.fadeIn = 0.3f;
                    dust2.color = Effects.ArsenalEffects.ArsenalGaussColor;
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target != null && target.CanBeMoved() && Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1) && Vector2.Distance(target.Center, Projectile.Center) < 650 * Projectile.scale)
                    {
                        Vector2 moveDir = target.Center.DirectionTo(Projectile.Center).SafeNormalize(Vector2.UnitX);
                        target.velocity = moveDir;
                        target.Center += moveDir * 3f;
                    }
                }
            }

            Vector2 dustVel = (Vector2.UnitX).RotatedByRandom(100) * Main.rand.NextFloat(3f, 8.5f) * Projectile.scale;
            Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel.SafeNormalize(Vector2.UnitX) * 120, Effects.ArsenalEffects.ArsenalGaussDust, -dustVel, 0, default, Main.rand.NextFloat(0.5f, 1f));
            dust.noGravity = true;
            dust.fadeIn = 0.05f;
            dust.color = Effects.ArsenalEffects.ArsenalGaussColor;

            if (Projectile.timeLeft > 2)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target != null && target.CanBeMoved() && Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1))
                    {
                        if (Vector2.Distance(target.Center, Projectile.Center) > 15 * Projectile.scale && Vector2.Distance(target.Center, Projectile.Center) < 300 * Projectile.scale)
                        {
                            Vector2 moveDir = target.Center.DirectionTo(Projectile.Center).SafeNormalize(Vector2.UnitX);
                            target.velocity = Vector2.Lerp(target.velocity, moveDir * 5, 0.12f);
                            target.Center += moveDir * 2f;
                        }
                    }
                }
            }
            
            Projectile.rotation += 0.05f;
            if (shrink > 0)
                shrink -= 0.05f;
            else
                shrink = 1;

            if (!strong)
                time++;
            strongTimer++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float shrinkFade = Math.Min(Utils.GetLerpValue(1, 0.7f, shrink, true), Utils.GetLerpValue(0f, 0.3f, shrink, true));
            float sine = Math.Abs(MathHelper.Lerp((float)Math.Sin(time * 0.1f / MathHelper.Pi), 0.4f, 0.8f));
            float sine2 = Math.Abs(MathHelper.Lerp((float)Math.Sin(time * 0.3f / MathHelper.Pi), 0.4f, 0.8f));
            float fade = (float)Math.Pow(Utils.GetLerpValue(0, 10, Projectile.timeLeft, true), 3);
            float areaScale = (strong ? 1f : 1) * fade;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSquareParticleBig").Value;
            Color drawColor = Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 } * (strong ? 0.5f : 0.3f) * shrinkFade;

            Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition, null, drawColor, MathHelper.PiOver4 - MathHelper.PiOver2 * sine * 3, tex2.Size() / 2f, (areaScale + sine * 0.5f) * shrink * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition, null, drawColor, MathHelper.PiOver2 * sine2 * 3, tex2.Size() / 2f, (areaScale - sine2 * 0.5f) * (float)Math.Pow(shrink, 2) * Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
