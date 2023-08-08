using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Skies
{
    public class BossRushSky : CustomSky
    {
        public bool CurrentlyActive;
        public float Intensity = 0f;
        public static float IdleTimer = 0f;
        public static float CurrentInterest = 0f;
        public static float IncrementalInterest = 0f;
        public static float CurrentInterestMin = 0f;
        public static bool ShouldDrawRegularly;

        public static Color GeneralColor => Color.Lerp(Color.LightGray, Color.Black, BossRushEvent.WhiteDimness) * 0.2f;

        public static bool DetermineDrawEligibility()
        {
            bool useEffect = ((BossRushEvent.BossRushActive && BossRushEvent.StartTimer > 100) || ShouldDrawRegularly) && !Main.gameMenu;

            if (SkyManager.Instance["CalamityMod:BossRush"] != null && useEffect != SkyManager.Instance["CalamityMod:BossRush"].IsActive())
            {
                if (useEffect)
                    SkyManager.Instance.Activate("CalamityMod:BossRush");
                else
                    SkyManager.Instance.Deactivate("CalamityMod:BossRush", new object[0]);
            }

            if (useEffect != Filters.Scene["CalamityMod:BossRush"].IsActive())
            {
                if (useEffect)
                    Filters.Scene.Activate("CalamityMod:BossRush");
                else
                    Filters.Scene["CalamityMod:BossRush"].Deactivate(new object[0]);
            }

            return useEffect;
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentlyActive)
            {
                if (Intensity < 1f)
                    Intensity += 0.03f;

                CurrentInterest = MathHelper.Clamp(CurrentInterest - 0.005f, CurrentInterestMin, 1f);
                IncrementalInterest = MathHelper.Lerp(IncrementalInterest, CurrentInterest, 0.085f);
                IdleTimer += MathHelper.Lerp(0.04f, 0.1f, IncrementalInterest);
            }
            else if (!CurrentlyActive && Intensity > 0f)
            {
                Intensity -= 0.03f;
                CurrentInterest = 0f;
                IncrementalInterest = 0f;
                IdleTimer = 0f;
            }
        }

        private float GetIntensity()
        {
            if (ShouldDrawRegularly)
                return 1f;

            float fadeRatio = BossRushEvent.StartTimer / (float)BossRushEvent.StartEffectTotalTime;
            return Utils.GetLerpValue(0.57f, 1f, fadeRatio, true);
        }

        public override Color OnTileColor(Color inColor) => new Color(Vector4.Lerp(GeneralColor.ToVector4() * 0.5f, inColor.ToVector4(), 1f - GetIntensity()));

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (GetIntensity() == 0f)
                return;

            if (maxDepth >= 0 && minDepth < 0 && GetIntensity() > 0f)
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), GeneralColor * GetIntensity() * 0.5f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            // Make the entire background fade to white at the end of the event.
            if (BossRushEvent.EndTimer >= 100f)
            {
                Texture2D whiteTexture = ModContent.Request<Texture2D>("CalamityMod/Skies/XerocLight").Value;
                Vector2 screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                float fadeToWhite = Utils.GetLerpValue(110f, 140f, BossRushEvent.EndTimer, true);
                screenCenter += new Vector2(Main.screenWidth, Main.screenHeight) * (Main.GameViewMatrix.Zoom - Vector2.One) * 0.5f;
                fadeToWhite *= Utils.GetLerpValue(BossRushEvent.EndVisualEffectTime - 5f, BossRushEvent.EndVisualEffectTime - 25f, BossRushEvent.EndTimer, true);
                float backScale = MathHelper.Lerp(0.01f, 8f, fadeToWhite);
                Color backFadeColor = Color.White * fadeToWhite * 0.64f;
                spriteBatch.Draw(whiteTexture, screenCenter, null, backFadeColor, 0f, whiteTexture.Size() * 0.5f, backScale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

            // Draw the Xeroc eye at the back of the sky.
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue && (BossRushEvent.EndTimer < BossRushEvent.EndVisualEffectTime - 40f || ShouldDrawRegularly))
            {
                Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                screenCenter += new Vector2(Main.screenWidth, Main.screenHeight) * (Main.GameViewMatrix.Zoom - Vector2.One) * 0.5f;
                float scale = MathHelper.Lerp(0.8f, 0.9f, IncrementalInterest) + (float)Math.Sin(IdleTimer) * 0.01f;
                Vector2 drawWorldPosition = new Vector2(Main.LocalPlayer.Center.X, 1120f);
                Vector2 drawPosition = (drawWorldPosition - screenCenter) * 0.097f + screenCenter - Main.screenPosition - Vector2.UnitY * 100f;

                Texture2D eyeTexture = ModContent.Request<Texture2D>("CalamityMod/Skies/XerocEye").Value;
                Color baseColorDraw = Color.Lerp(Color.White, Color.Red, IncrementalInterest);
                Vector2 origin = eyeTexture.Size() * 0.5f;

                spriteBatch.Draw(eyeTexture, drawPosition, null, baseColorDraw, 0f, origin, scale, SpriteEffects.None, 0f);
                Color fadedColor = Color.Lerp(baseColorDraw, Color.Coral, 0.3f) * MathHelper.Lerp(0.18f, 0.3f, IncrementalInterest);
                fadedColor.A = 0;

                float backEyeOutwardness = MathHelper.Lerp(8f, 4f, IncrementalInterest);
                int backInstances = (int)MathHelper.Lerp(6f, 24f, IncrementalInterest);
                float fourPi = MathHelper.TwoPi * 2f;
                float time = Main.GlobalTimeWrappedHourly * 2.1f;
                for (int i = 0; i < backInstances; i++)
                {
                    Vector2 drawOffset = (fourPi * i / backInstances + time).ToRotationVector2() * backEyeOutwardness;
                    spriteBatch.Draw(eyeTexture, drawPosition + drawOffset, null, fadedColor, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }

            if (ShouldDrawRegularly)
                ShouldDrawRegularly = false;
        }

        public override float GetCloudAlpha() => 1f - GetIntensity();

        public override void Activate(Vector2 position, params object[] args) => CurrentlyActive = true;

        public override void Deactivate(params object[] args) => CurrentlyActive = false;

        public override void Reset() => CurrentlyActive = false;

        public override bool IsActive() => CurrentlyActive || Intensity > 0f;
    }
}
