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
    public class ExoLight : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Vector2 InitialCenter;
        public Vector2 Destination;
        public const float MaxRadius = 90f;
        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public int DustType
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public float YDirection
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Light");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
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
            writer.Write(YDirection);
            writer.WriteVector2(Destination);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            YDirection = reader.ReadSingle();
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
                if (Main.myPlayer == Projectile.owner)
                {
                    Destination = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }
                DustType = Main.rand.NextBool(2) ? 107 : 234;
                if (Main.rand.NextBool(4))
                {
                    DustType = 269;
                }
                Projectile.localAI[0] = 1f;
            }

            if (Destination == Vector2.Zero)
                return;

            if (Time <= 90f)
            {
                Projectile.position = Vector2.Lerp(InitialCenter, Destination, Time / 90f);
                Projectile.position.Y += (float)Math.Sin(Time / 90f * MathHelper.TwoPi) * 75f;
            }
            else if (Time < 180f)
            {
                // For those who haven't seen this yet (namely in the 1.4 source), this allows you to achieve a
                // fade-in and fade-out effect with relative ease. At 90, multiply by 0, and somewhere in-between until 115, where you multiply by 1.
                // And then do the same for the fade-out effect of 180f-165. These inverse linear interpolations cannot overlap by definition because
                // their ranges do not overlap. Overall a really cool trick.
                float radius = MaxRadius * Utils.GetLerpValue(90f, 115f, Time, true) * Utils.GetLerpValue(180f, 165f, Time, true);
                radius *= 1f + (float)Math.Cos(Main.GlobalTimeWrappedHourly / 24f) * 0.25f;
                if (radius < 5f)
                    radius = 5f;
                Projectile.Center = Destination + ((Time - 90) / 90f * MathHelper.ToRadians(720f) + (YDirection == -1).ToInt() * MathHelper.Pi).ToRotationVector2() * radius;
            }
            else if (Time == 180f)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;
        }

        public void GenerateRotationalDust()
        {
            for (int i = 0; i < 9; i++)
            {
                float angle = Time / 45f + i / 9f * MathHelper.TwoPi;
                Vector2 offsetSpan = angle.ToRotationVector2() * Projectile.Size * 0.65f * Projectile.scale;
                Vector2 edgePosition = Projectile.Center + offsetSpan.RotatedBy(Projectile.rotation);
                Dust dust = Dust.NewDustPerfect(edgePosition + Main.rand.NextVector2Circular(4f, 4f), DustType);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.scale = 0.65f;
            }
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
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/PhotovisceratorLight");
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 16f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = CalamityUtils.MulticolorLerp(MathHelper.Clamp(colorInterpolation, 0f, 0.99f), new Color[]
                {
                    Color.PaleGreen,
                    Color.Violet,
                    Color.SlateGray
                }) * Projectile.Opacity;
                color.A = 0;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);

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

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projID = ModContent.ProjectileType<ExoSpark>();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vel = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 16f;
                    Projectile.NewProjectileDirect(Projectile.GetProjectileSource_FromThis(), Projectile.Center, vel, projID, Projectile.damage, Projectile.knockBack * 0.3f, Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.ExoDebuffs(1.5f);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.ExoDebuffs(1.5f);
    }
}
