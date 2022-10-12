using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class RustyBeaconPulse : ModProjectile
    {
        public float LifetimeCompletion => 1f - Projectile.timeLeft / (float)RustyBeaconPrototype.PulseLifetime;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Irradiated Pulse");
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.timeLeft = RustyBeaconPrototype.PulseLifetime;
            Projectile.scale = 0.001f;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.localAI[0] = Main.rand.NextBool().ToDirectionInt();
                Projectile.netUpdate = true;
            }

            Projectile.Opacity = 1f - (float)Math.Pow(LifetimeCompletion, 1.56);
            Projectile.scale = MathHelper.Lerp(0.5f, 7f, LifetimeCompletion);
            Projectile.rotation += Projectile.localAI[0] * 0.012f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color c1 = new Color(153, 226, 104, 0);
            Color c2 = new Color(158, 128, 175, 92);
            return Color.Lerp(c1, c2, 1f - Projectile.Opacity) * Projectile.Opacity * 0.67f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color drawColor = Projectile.GetAlpha(lightColor) * 0.33f;
            for (int i = 0; i < 8; i++)
            {
                float rotation = Projectile.rotation;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * Projectile.scale;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                if (i % 2 == 1)
                    rotation *= -1f;

                Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, rotation, texture.Size() * 0.5f, Projectile.scale, 0, 0);
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target) => !target.CountsAsACritter && !target.friendly && target.chaseable;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), RustyBeaconPrototype.IrradiatedDebuffTime);
            target.AddBuff(BuffID.Poisoned, RustyBeaconPrototype.PoisonedDebuffTime);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.scale * 48f, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
