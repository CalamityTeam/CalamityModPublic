using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VeeringWindFrostWave : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.6f);
        }

        public override bool PreDraw(ref Color lightColor) // Photoviscerator ball drawcode, slightly edited.
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 4.8f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.Blue, Color.LightBlue, colorInterpolation);
                Color invertedColor = Color.Lerp(Color.LightBlue, Color.DarkBlue, 1f - colorInterpolation);
                color.A = 0;
                invertedColor.A = 0;
                Vector2 drawOffset = new Vector2(-28f, -28f);
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color outerColor = color;
                Color innerColor = invertedColor;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                if (Projectile.timeLeft <= 30) // Shrinks to nothing when projectile is nearing death.
                {
                    intensity *= Projectile.timeLeft / 30f;
                }
                // Become smaller the futher along the old positions we are.
                Vector2 outerScale = new Vector2(1f) * intensity;
                Vector2 innerScale = new Vector2(1f) * intensity;
                outerColor *= intensity * 0.5f;
                innerColor *= intensity * 0.5f;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }
    }
}
