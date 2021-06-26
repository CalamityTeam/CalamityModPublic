using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ExoLightBomb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Vector2 InitialCenter;
        public Vector2 Destination;
        public const float MaxRadius = 90f;
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public int DustType
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public float YDirection
        {
            get => projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Light");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.alpha = 127;
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
            Lighting.AddLight(projectile.Center, Color.DarkSlateGray.ToVector3());
            projectile.alpha = (int)MathHelper.Lerp(255, 127, Utils.InverseLerp(0f, 25f, Time, true));
            projectile.scale = MathHelper.Lerp(0.001f, 1f, Utils.InverseLerp(0f, 25f, Time, true));
            if (projectile.localAI[0] == 0f)
            {
                InitialCenter = projectile.Center;
                if (Main.myPlayer == projectile.owner)
                {
                    Destination = Main.MouseWorld;
                    projectile.netUpdate = true;
                }
                DustType = Main.rand.NextBool(2) ? 107 : 234;
                if (Main.rand.NextBool(4))
                {
                    DustType = 269;
                }
                projectile.localAI[0] = 1f;
            }

            if (Destination == Vector2.Zero)
                return;

            if (Time <= 90f)
            {
                projectile.position = Vector2.Lerp(InitialCenter, Destination, Time / 90f);
                projectile.position.Y += (float)Math.Sin(Time / 90f * MathHelper.TwoPi) * 75f;
            }
            else if (Time < 180f)
            {
                // For those who haven't seen this yet (namely in the 1.4 source), this allows you to achieve a
                // fade-in and fade-out effect with relative ease. At 90, multiply by 0, and somewhere in-between until 115, where you multiply by 1.
                // And then do the same for the fade-out effect of 180f-165. These inverse linear interpolations cannot overlap by definition because
                // their ranges do not overlap. Overall a really cool trick.
                float radius = MaxRadius * Utils.InverseLerp(90f, 115f, Time, true) * Utils.InverseLerp(180f, 165f, Time, true);
                radius *= 1f + (float)Math.Cos(Main.GlobalTime / 24f) * 0.25f;
                if (radius < 5f)
                    radius = 5f;
                projectile.Center = Destination + ((Time - 90) / 90f * MathHelper.ToRadians(720f) + (YDirection == -1).ToInt() * MathHelper.Pi).ToRotationVector2() * radius;
            }
            else if (Time == 180f)
                projectile.Kill();

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;
        }

        public void GenerateRotationalDust()
        {
            for (int i = 0; i < 9; i++)
            {
                float angle = Time / 45f + i / 9f * MathHelper.TwoPi;
                Vector2 offsetSpan = angle.ToRotationVector2() * projectile.Size * 0.65f * projectile.scale;
                Vector2 edgePosition = projectile.Center + offsetSpan.RotatedBy(projectile.rotation);
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
            }) * projectile.Opacity;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D lightTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/PhotovisceratorLight");
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(projectile.timeLeft / 16f + Main.GlobalTime / 20f + i / (float)projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = CalamityUtils.MulticolorLerp(MathHelper.Clamp(colorInterpolation, 0f, 0.99f), new Color[]
                {
                    Color.PaleGreen,
                    Color.Violet,
                    Color.SlateGray
                }) * projectile.Opacity;
                color.A = 0;
                Vector2 drawPosition = projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTime % 60f * MathHelper.TwoPi);

                // Become smaller the futher along the old positions we are.
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)projectile.oldPos.Length);

                Vector2 outerScale = new Vector2(1.25f) * intensity;
                Vector2 innerScale = new Vector2(1.25f) * intensity * 0.7f;
                outerColor *= intensity * projectile.scale;
                innerColor *= intensity * projectile.scale;
                spriteBatch.Draw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectileDirect(projectile.Center,
                                                   Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 16f,
                                                   ModContent.ProjectileType<ExoSpark>(),
                                                   projectile.damage,
                                                   projectile.knockBack * 0.3f,
                                                   projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            target.AddBuff(BuffID.CursedInferno, 180);
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Ichor, 180);
        }
    }
}
