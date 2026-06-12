using System;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class NidhoggExplosion : ModProjectile, ILocalizedModType
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 420;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Vector2 dustVel = (Vector2.UnitX).RotatedByRandom(100) * Main.rand.NextFloat(3f, 8.5f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel.SafeNormalize(Vector2.UnitX) * 120, ModContent.DustType<SquashDust>(), -dustVel, 0, default, Main.rand.NextFloat(0.5f, 1f));
            dust.noGravity = true;
            dust.fadeIn = 0.05f;
            dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target != null && target.CanBeMoved())
                {
                    if (Vector2.Distance(target.Center, Projectile.Center) > 15 && Vector2.Distance(target.Center, Projectile.Center) < 500)
                    {
                        Vector2 moveDir = target.Center.DirectionTo(Projectile.Center).SafeNormalize(Vector2.UnitX);
                        target.velocity = Vector2.Lerp(target.velocity, moveDir * 8, 0.12f);
                        target.Center += moveDir * 1.5f;
                    }
                }
            }
            Projectile.rotation += 0.07f;
            if (shrink > 0)
                shrink -= strong ? 0.03f : 0.03f;
            else
                shrink = 1;

            if (!strong)
                time++;
            strongTimer++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!strong)
                modifiers.SourceDamage *= 0.1f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.5f * Projectile.scale, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            float shrinkFade = Math.Min(Utils.GetLerpValue(1, 0.7f, shrink, true), Utils.GetLerpValue(0f, 0.3f, shrink, true));
            float sine = Math.Abs(MathHelper.Lerp((float)Math.Sin(time * 0.5f / MathHelper.Pi), 0.4f, 0.8f));
            float sine2 = Math.Abs(MathHelper.Lerp((float)Math.Sin(time * 0.7f / MathHelper.Pi), 0.4f, 0.8f));
            float fade = (float)Math.Pow(Utils.GetLerpValue(0, 10, Projectile.timeLeft, true), 3);
            float areaScale = (strong ? 1f : 1) * fade;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSquareParticleBig").Value;
            Color drawColor = Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 } * (strong ? 0.5f : 0.3f) * shrinkFade;

            Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition, null, drawColor, MathHelper.PiOver4 - MathHelper.PiOver2 * sine * 3, tex2.Size() / 2f, (areaScale + sine * 0.5f) * shrink, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition, null, drawColor, MathHelper.PiOver2 * sine2 * 3, tex2.Size() / 2f, (areaScale - sine2 * 0.5f) * (float)Math.Pow(shrink, 2), SpriteEffects.None, 0);

            return false;
        }
    }
}
