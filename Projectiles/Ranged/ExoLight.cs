using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator left click side projectile (waving light)
    public class ExoLight : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/ExtraTextures/SmallGreyscaleCircle";

        public Vector2 InitialCenter;
        public Vector2 Destination;
        public const float MaxRadius = 90f;
        public ref float YDirection => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 36;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 127;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Destination);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Destination = reader.ReadVector2();
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.DarkSlateGray.ToVector3());
            Projectile.alpha = (int)MathHelper.Lerp(255, 127, Utils.GetLerpValue(0f, 25f, Time, true));
            Projectile.scale = MathHelper.Lerp(0.001f, 1f, Utils.GetLerpValue(0f, 25f, Time, true));
            if (Projectile.localAI[0] == 0f)
            {
                InitialCenter = Projectile.Center;
                Projectile.localAI[0] = 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Destination = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }
            }

            if (Destination == Vector2.Zero)
                return;

            if (Time <= 60f)
            {
                Projectile.Center = Vector2.Lerp(InitialCenter, Destination, Time / 60f);
                Projectile.Center += (Vector2.UnitY * MathF.Sin(Time / 60f * MathHelper.TwoPi) * 75f * YDirection).RotatedBy(Projectile.velocity.ToRotation());
            }
            else if (Time < 120f)
            {
                // For those who haven't seen this yet (namely in the 1.4 source), this allows you to achieve a
                // fade-in and fade-out effect with relative ease. At 90, multiply by 0, and somewhere in-between until 115, where you multiply by 1.
                // And then do the same for the fade-out effect of 180f-165. These inverse linear interpolations cannot overlap by definition because
                // their ranges do not overlap. Overall a really cool trick.
                float radius = MaxRadius * Utils.GetLerpValue(60f, 75f, Time, true) * Utils.GetLerpValue(120f, 105f, Time, true);
                radius *= 1f + MathF.Cos(Main.GlobalTimeWrappedHourly / 24f) * 0.25f;
                if (radius < 5f)
                    radius = 5f;
                Projectile.Center = Destination + ((Time - 60) / 60f * MathHelper.ToRadians(720f) + (YDirection == -1).ToInt() * MathHelper.Pi).ToRotationVector2() * radius;
            }
            else if (Time == 120f)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return CalamityUtils.MulticolorLerp(Time / 60f % 1f, new Color[]
            {
                Color.PaleGreen,
                Color.Violet,
                Color.SlateGray
            }) * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = MathF.Cos(Projectile.timeLeft / 16f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = CalamityUtils.MulticolorLerp(MathHelper.Clamp(colorInterpolation, 0f, 0.99f), new Color[]
                {
                    Color.PaleGreen,
                    Color.Violet,
                    Color.SlateGray
                }) * Projectile.Opacity;
                color.A = 0;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-15f, -15f);
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * MathF.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);

                // Become smaller the futher along the old positions we are.
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);

                Vector2 outerScale = new Vector2(1.25f) * intensity;
                Vector2 innerScale = new Vector2(1.25f) * intensity * 0.7f;
                outerColor *= intensity * Projectile.scale;
                innerColor *= intensity * Projectile.scale;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projID = ModContent.ProjectileType<ExoSpark>();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vel = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 16f;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, projID, Projectile.damage, Projectile.knockBack * 0.3f, Projectile.owner);
                }

                // If the explosion happens naturally, there's instead only 1 giant explosion instead of 2 separate ones
                if (Time >= 120f && YDirection == 1)
                    return;

                float scaleBonus = Time >= 120f ? Main.rand.NextFloat(2.4f, 3.2f) : Main.rand.NextFloat(0.8f, 1.6f);
                Projectile.scale = 4.5f * scaleBonus;
                Particle pinkBoom = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.Violet, Vector2.One, Main.rand.NextFloat(0f, MathHelper.TwoPi), 0f, 0.5f * scaleBonus, 12);
                GeneralParticleHandler.SpawnParticle(pinkBoom);
                Particle greenBoom = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.PaleGreen, Vector2.One, Main.rand.NextFloat(0f, MathHelper.TwoPi), 0f, 0.4f * scaleBonus, 12);
                GeneralParticleHandler.SpawnParticle(greenBoom);

                // we should probably have a generic util to do this whole thing
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.Damage();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * Projectile.scale * 0.5f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 360);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 360);
    }
}
