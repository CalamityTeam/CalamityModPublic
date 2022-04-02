using CalamityMod.NPCs.AstrumDeus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class DeusRitualDrama : ModProjectile
    {
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public bool CreatedWithStarcore => projectile.ai[1] == 1f;
        public const int TotalSinePeriods = 6;
        public const int PulseTime = 45;
        public const int TotalRitualTime = 420;
        public const float MaxUpwardRise = 540f;
        public static readonly Point PulseSize = new Point(300, 300);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deus Ritual Drama");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = TotalRitualTime;
        }

        public override void AI()
        {
            projectile.extraUpdates = CreatedWithStarcore.ToInt();

            Time++;
            if (Time == TotalRitualTime - PulseTime)
            {
                int idx = NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y - (int)MaxUpwardRise, ModContent.NPCType<AstrumDeusHeadSpectral>(), 1);
                if (idx != -1)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AstrumDeusSpawn"), projectile.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        CalamityUtils.BossAwakenMessage(idx);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, idx);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float upwardnessRatio = Utils.InverseLerp(60f, TotalRitualTime, Time, true);
            float upwardness = MathHelper.Lerp(0f, MaxUpwardRise, upwardnessRatio);
            if (Time >= TotalRitualTime - PulseTime)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                float pulseCompletionRatio = Utils.InverseLerp(TotalRitualTime - PulseTime, TotalRitualTime, Time, true);
                Vector2 scale = projectile.scale * (3f + pulseCompletionRatio * 5f) * new Vector2(1.5f, 1f);
                DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                    projectile.Center - Main.screenPosition + PulseSize.ToVector2() * scale * 0.5f - Vector2.UnitY * upwardness,
                    new Rectangle(0, 0, PulseSize.X, PulseSize.Y),
                    new Color(new Vector4(1f - (float)Math.Sqrt(pulseCompletionRatio))) * 0.66f,
                    projectile.rotation,
                    PulseSize.ToVector2(),
                    scale,
                    SpriteEffects.None, 0);

                Color pulseColor = Color.Lerp(Color.Cyan * 1.5f, Color.OrangeRed, MathHelper.Clamp(pulseCompletionRatio * 1.5f, 0f, 1f));
                GameShaders.Misc["ForceField"].UseColor(pulseColor);
                GameShaders.Misc["ForceField"].Apply(drawData);
                drawData.Draw(spriteBatch);
                return false;
            }
            float outwardnessRatio = Utils.InverseLerp(60f, 220f, Time, true);
            if (Time > 250f)
                outwardnessRatio = 1f - Utils.InverseLerp(250f, TotalRitualTime - PulseTime, Time, true);
            float outwardness = MathHelper.Lerp(0f, 140f, outwardnessRatio);

            Vector2 offset = new Vector2((float)Math.Sin(Time / (TotalRitualTime - PulseTime) * MathHelper.TwoPi * TotalSinePeriods) * outwardness, -upwardness);

            // If the stars "collide", generate some small explosion dust.
            if (!Main.dedServ && Math.Abs(offset.X) < 6f && Time > 60f)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center + Vector2.UnitY * offset.Y, 261);
                    dust.color = Utils.SelectRandom(Main.rand, Color.Cyan, Color.OrangeRed);
                    dust.scale = 1.15f;
                    dust.velocity = Main.rand.NextVector2CircularEdge(3f, 3f) * Main.rand.NextFloat(0.7f, 1.4f);
                    dust.noGravity = true;

                    float angle = MathHelper.TwoPi * i / 20f;
                    dust = Dust.NewDustPerfect(projectile.Center + Vector2.UnitY * offset.Y, 261);
                    dust.color = Utils.SelectRandom(Main.rand, Color.Cyan, Color.OrangeRed);
                    dust.scale = 1.15f;
                    dust.velocity = angle.ToRotationVector2() * 7f;
                    dust.noGravity = true;
                }
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AstralBeaconOrbPulse"), projectile.Center);
            }

            DrawStars(spriteBatch, offset);

            return false;
        }

        public void DrawStars(SpriteBatch spriteBatch, Vector2 offset)
        {
            Texture2D starTexture = ModContent.GetTexture(Texture);
            for (int i = 0; i < 6; i++)
            {
                float angle = MathHelper.TwoPi * i / 6f + Time / 15f;
                Vector2 angularOffset = angle.ToRotationVector2() * 4f;
                spriteBatch.Draw(starTexture,
                                 projectile.Center + angularOffset + offset - Main.screenPosition,
                                 null,
                                 Color.Cyan * 0.5f,
                                 0f,
                                 starTexture.Size() * 0.5f,
                                 0.6f,
                                 SpriteEffects.None,
                                 0f);
                spriteBatch.Draw(starTexture,
                                 projectile.Center + angularOffset + offset * new Vector2(-1f, 1f) - Main.screenPosition,
                                 null,
                                 Color.OrangeRed * 0.5f,
                                 0f,
                                 starTexture.Size() * 0.5f,
                                 0.6f,
                                 SpriteEffects.None,
                                 0f);
            }
            spriteBatch.Draw(starTexture,
                             projectile.Center + offset - Main.screenPosition,
                             null,
                             Color.Cyan * 1.4f,
                             0f,
                             starTexture.Size() * 0.5f,
                             0.6f,
                             SpriteEffects.None,
                             0f);
            spriteBatch.Draw(starTexture,
                             projectile.Center + offset * new Vector2(-1f, 1f) - Main.screenPosition,
                             null,
                             Color.OrangeRed * 1.1f,
                             0f,
                             starTexture.Size() * 0.5f,
                             0.6f,
                             SpriteEffects.None,
                             0f);

            // Generate dust at the star position. This gives them a trail effect.
            if (!Main.dedServ)
            {
                Dust dust2 = Dust.NewDustPerfect(projectile.Center + offset, 261);
                dust2.color = Color.Cyan;
                dust2.scale = 1.15f;
                dust2.velocity = Vector2.Zero;
                dust2.noGravity = true;

                dust2 = Dust.NewDustPerfect(projectile.Center + offset * new Vector2(-1f, 1f), 261);
                dust2.color = Color.OrangeRed;
                dust2.scale = 1.15f;
                dust2.velocity = Vector2.Zero;
                dust2.noGravity = true;
            }
        }
    }
}
