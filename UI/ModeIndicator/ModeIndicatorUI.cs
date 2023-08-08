using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;
using System;
using Terraria.ID;
using Terraria.Audio;
using CalamityMod.Systems;
using static CalamityMod.Systems.DifficultyModeSystem;
using Terraria.GameInput;
using Terraria.UI.Chat;
using System.Text.RegularExpressions;
using static Terraria.GameContent.FontAssets;
using System.Runtime.InteropServices;

namespace CalamityMod.UI.ModeIndicator
{
    public class ModeIndicatorUI
    {
        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static Vector2 FrameSize => new Vector2(30f, 38f);
        public static Rectangle MainClickArea => Utils.CenteredRectangle(DrawCenter, FrameSize * MainIconScale);
        public static bool ClickingMouse => Main.mouseLeft && Main.mouseLeftRelease;
        public static Vector2 DrawCenter => new Vector2(Main.screenWidth - 400f - WidthForTier(MostAlternateDifficulties) * 0.5f, 82f) + FrameSize * 0.5f;

        private static int GlowFadeAnimLength = 40;
        public static int GlowFadeTime = 0;
        //Lock icon
        internal const int LockAnimLength = 30;
        private static int lockClickTime = 0;
        private static bool previousLockStatus = false;
        //Expand and shrink on hover
        internal const float MaxIconHoverScaleBoost = 0.3f;
        internal const float IconHoverScaleIncrement = MaxIconHoverScaleBoost / 6f;
        internal const float IconHoverScaleDecrement = MaxIconHoverScaleBoost / 10f;
        private static float iconHoverScaleBoost = 0f;
        private static bool previouslyHoveringMainIcon = false;
        public static float MainIconScale => 1f + iconHoverScaleBoost;
        //Menu state
        private static bool menuOpen = false;
        internal static int MenuAnimLength => MostAlternateDifficulties > 1 ? 60 : 40;
        private static int menuOpenTransitionTime = 0;
        private static DifficultyMode previouslyHoveredMode = null;
        public static float WidthForTier(int alts) => (alts - 1) * 40f;

        private static void ClearVariables()
        {
            GlowFadeTime = 0;
            lockClickTime = 0;
            menuOpenTransitionTime = 0;
            previousLockStatus = false;
            iconHoverScaleBoost = 0f;
            menuOpen = false;
            previouslyHoveringMainIcon = false;
            previouslyHoveredMode = null;
        }

        public static void TickVariables()
        {
            if (lockClickTime > 0)
                lockClickTime--;

            if (menuOpenTransitionTime > 0)
                menuOpenTransitionTime--;

            if (iconHoverScaleBoost > 0)
                iconHoverScaleBoost -= IconHoverScaleDecrement;

            if (!menuOpen || menuOpenTransitionTime > 0)
                previouslyHoveredMode = null;

            if (GlowFadeTime > 0)
                GlowFadeTime--;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            // The mode indicator should only be displayed when the inventory is open, to prevent obstruction.
            if (!Main.playerInventory)
            {
                ClearVariables();
                return;
            }

            Texture2D indicatorTexture = GetCurrentDifficulty.Texture.Value;

            GetDifficultyStatus(out LocalizedText difficultyText);
            GetLockStatus(out LocalizedText lockText, out bool locked);

            //Grows the icon when hovering it.
            if (MouseScreenArea.Intersects(MainClickArea))
            {
                if (!_hasCheckedItOutYet)
                {
                    GlowFadeTime = GlowFadeAnimLength;
                    _hasCheckedItOutYet = true;
                }

                if (!previouslyHoveringMainIcon)
                {
                    previouslyHoveringMainIcon = true;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }

                if (iconHoverScaleBoost < MaxIconHoverScaleBoost)
                {
                    iconHoverScaleBoost = Math.Min(iconHoverScaleBoost + IconHoverScaleIncrement + IconHoverScaleDecrement, MaxIconHoverScaleBoost);

                    if (ClickingMouse && menuOpenTransitionTime == 0 && !locked)
                    {
                        SoundEngine.PlaySound(menuOpen ? SoundID.MenuClose : SoundID.MenuOpen);
                        menuOpenTransitionTime = MenuAnimLength;
                        menuOpen = !menuOpen;
                    }
                }
            }
            else
                previouslyHoveringMainIcon = false;

            if (!_hasCheckedItOutYet || GlowFadeTime > 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);

                Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/BloomFlare").Value;
                float opacity = !_hasCheckedItOutYet ? 1f : 1f * GlowFadeTime / (float)GlowFadeAnimLength;
                float scale = 0.4f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.05f;
                float rot = Main.GlobalTimeWrappedHourly * 0.5f;

                spriteBatch.Draw(bloomTex, DrawCenter, null, Color.Crimson * opacity, rot, new Vector2(123, 124), scale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);


                Texture2D outlineTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicatorOutline").Value;
                spriteBatch.Draw(outlineTexture, DrawCenter, null, Color.White * opacity, 0f, outlineTexture.Size() * 0.5f, MainIconScale, SpriteEffects.None, 0f);
            }

            string extraDescText = string.Empty;
            if (menuOpenTransitionTime > 0 || menuOpen)
                ManageHexIcons(spriteBatch, out extraDescText);

            //Draw the indicator itself.
            spriteBatch.Draw(indicatorTexture, DrawCenter, null, Color.White, 0f, indicatorTexture.Size() * 0.5f, MainIconScale, SpriteEffects.None, 0f);

            if (locked)
                DrawLock(spriteBatch);

            if (difficultyText != LocalizedText.Empty || extraDescText != string.Empty)
            {
                string textToDisplay = difficultyText.ToString();
                if (difficultyText != LocalizedText.Empty)
                {
                    if (lockText != LocalizedText.Empty)
                        textToDisplay += "\n" + lockText.ToString();
                }

                else
                    textToDisplay = extraDescText;

                //Get the size of the textbox
                Vector2 boxSize = MouseText.Value.MeasureString(textToDisplay);

                //Get a "regexed" size which matches the text properly.
                //Indeed, there is some scuffery in the code that makes it so that chat tags still get accounted as extra size, so we have to use 2 different values
                //One for the displacement, which is the improper size vanilla uses, and another for the actual visual size, which is the one the textbox will use
                int numLines = 1 + Regex.Matches(textToDisplay, "\n").Count; //It's inconsistent. Adding one by default makes some textboxes too large, not adding one can make some too small
                string heightCalculator = string.Concat(Enumerable.Repeat("mis nuevos los gatos \n", numLines));
                Vector2 regexedBoxSize = new Vector2(ChatManager.GetStringSize(MouseText.Value, textToDisplay, Vector2.One).X, ChatManager.GetStringSize(MouseText.Value, heightCalculator, Vector2.One).Y);

                Vector2 textboxStart = new Vector2(Main.mouseX, Main.mouseY) + Vector2.One * 14;
                if (Main.ThickMouse)
                    textboxStart += Vector2.One * 6;

                if (!Main.mouseItem.IsAir)
                    textboxStart.X += 34;

                if (textboxStart.X + boxSize.X + 4f > (float)Main.screenWidth)
                    textboxStart.X = Main.screenWidth - boxSize.X - 4f;

                if (textboxStart.Y + regexedBoxSize.Y + 4f > (float)Main.screenHeight)
                    textboxStart.Y = Main.screenHeight - regexedBoxSize.Y - 4f;

                //It'd be great to be able to add a background to it but i don't think i know how to get the position of the text for that.
                //Also the "get string size" thing breaks with colored lines so :(
                Utils.DrawInvBG(spriteBatch, new Rectangle((int)textboxStart.X - 10, (int)textboxStart.Y - 10, (int)regexedBoxSize.X + 20, (int)regexedBoxSize.Y + 16), new Color(50, 20, 35) * 0.925f);
                
                //Add the hover text.
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText(textToDisplay);
            }
            TickVariables();
        }

        public static void GetDifficultyStatus(out LocalizedText text)
        {
            text = LocalizedText.Empty;
            if (MouseScreenArea.Intersects(MainClickArea))
            {
                //Display the first non-none difficulty by default
                string modeToDisplay = Difficulties[1].Name.ToString();
                bool anyActiveMode = false;

                for (int i = 1; i < Difficulties.Length; i++)
                {
                    if (GetCurrentDifficulty == Difficulties[i])
                    {
                        modeToDisplay = Difficulties[i].Name.ToString();
                        anyActiveMode = true;
                    }
                }
                string modeStr = CalamityUtils.GetTextValue("UI.ModeAppend");
                string activeText = CalamityUtils.GetTextValue("UI." + (anyActiveMode ? "Active" : "NotActive"));
                text = CalamityUtils.GetText("UI.DifficultyStatusText").WithFormatArgs(modeToDisplay + modeStr, activeText.ToLower());
            }
        }

        //Checks if the mode change is available or not, and jiggles the lock icon if the player clicks on it.
        public static void GetLockStatus(out LocalizedText text, out bool locked)
        {
            locked = false;
            text = CalamityUtils.GetText("UI.DifficultyClickText");
            if (!Main.expertMode && GetCurrentDifficulty == Difficulties[0])
            {
                locked = true;
                text = CalamityUtils.GetText("UI.ExpertDifficultyLock");
            }
            else if (CalamityPlayer.areThereAnyDamnBosses || BossRushEvent.BossRushActive)
            {
                locked = true;
                text = CalamityUtils.GetText("UI.ChangingTheRules");
            }

            //Shakes the lock if it automatically changed, because a boss was summoned
            if (locked != previousLockStatus && lockClickTime == 0)
                lockClickTime = LockAnimLength;

            previousLockStatus = locked;

            //Close the menu if its locked
            if (locked && menuOpen)
            {
                menuOpen = false;
                menuOpenTransitionTime = MenuAnimLength - menuOpenTransitionTime;
            }

            //Click handling
            if (locked && ClickingMouse && lockClickTime == 0)
            {
                if (MouseScreenArea.Intersects(MainClickArea))
                {
                    lockClickTime = LockAnimLength;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }
        }

        #region Cool drawing stuff
        private static CurveSegment lockGrow = new CurveSegment(SineOutEasing, 0f, 1f, 0.4f);
        private static CurveSegment lockShrink = new CurveSegment(SineInEasing, 0.6f, 1.4f, -0.4f);
        private static CurveSegment lockBump = new CurveSegment(SineBumpEasing, 0.9f, 1f, -0.2f);
        internal static float LockShakeScale => PiecewiseAnimation(lockClickTime / (float)LockAnimLength, new CurveSegment[] { lockGrow, lockShrink, lockBump });

        private static CurveSegment barExpand = new CurveSegment(SineInOutEasing, 0f, 0f, 1f);
        internal static float BarExpansionProgress
        {
            get
            {
                float animLength = MostAlternateDifficulties == 1 ? (float)MenuAnimLength : MenuAnimLength * 2 / 3f;
                float progress = (menuOpenTransitionTime / animLength);
                if (menuOpen)
                    progress = 1 - progress;

                return PiecewiseAnimation(progress, new CurveSegment[] { barExpand });
            }
        }


        private static CurveSegment barWidthExpand = new CurveSegment(SineInOutEasing, 0f, 0f, 1.2f);
        private static CurveSegment barWidthRetract = new CurveSegment(SineInEasing, 0.6f, 1.2f, -0.2f);

        internal static float BarWidthExpansionProgress
        {
            get
            {
                float progress = Math.Max(menuOpenTransitionTime - MenuAnimLength * 2 / 3f, 0f) / (MenuAnimLength / 3f);
                if (menuOpen)
                    progress = 1 - progress;

                return PiecewiseAnimation(progress, new CurveSegment[] { barWidthExpand, barWidthRetract });
            }
        }

        public static void ManageHexIcons(SpriteBatch spriteBatch, out string text)
        {
            int tiers = DifficultyTiers.Count();
            float barLength = 60 * tiers * BarExpansionProgress;
            float progress = menuOpen ? 1 - menuOpenTransitionTime / (float)MenuAnimLength : menuOpenTransitionTime / (float)MenuAnimLength;
            Vector2 basePosition = DrawCenter + (barLength / (float)(tiers + 1f)) * Vector2.UnitY;

            Texture2D outlineTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicatorOutline").Value;

            text = string.Empty;
            bool modeHovered = false;
            Vector2 positionOffset = (barLength / (float)(tiers + 1f)) * Vector2.UnitY;
            float progressMult = 0.8f * progress;
            Color progressColor = Color.White * progress;

            for (int i = 0; i < tiers; i++)
            {
                int modesAtTier = DifficultyTiers[i].Length;
                float width = WidthForTier(modesAtTier) * 0.5f;
                for (int j = 0; j < modesAtTier; j++)
                {
                    DifficultyMode mode = DifficultyTiers[i][j];
                    Texture2D hexIcon = mode.Texture.Value;
                    Vector2 hexIconSize = hexIcon.Size();

                    // Get position.
                    Vector2 iconPosition = basePosition + positionOffset * i;
                    if (modesAtTier > 1)
                        iconPosition += Vector2.UnitX * MathHelper.Lerp(width * -1f, width, j / (float)(modesAtTier - 1)) * BarWidthExpansionProgress;

                    bool hovered = MouseScreenArea.Intersects(Utils.CenteredRectangle(iconPosition, hexIconSize));

                    float usedOpacity = mode.Enabled ? 0.85f : 0.55f;
                    if (hovered)
                        usedOpacity = MathHelper.Lerp(usedOpacity, 1f, 0.7f);

                    // Outline the currently selected difficulty.
                    if (mode == GetCurrentDifficulty)
                    {
                        usedOpacity = 1f;
                        spriteBatch.Draw(outlineTexture, iconPosition, null, mode.ChatTextColor * progressMult, 0f, outlineTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                    }

                    spriteBatch.Draw(hexIcon, iconPosition, null, progressColor * usedOpacity, 0f, hexIconSize * 0.5f, 1f, SpriteEffects.None, 0f);

                    if (menuOpenTransitionTime == 0 && hovered)
                    {
                        if (previouslyHoveredMode != mode)
                            SoundEngine.PlaySound(SoundID.MenuTick);

                        previouslyHoveredMode = mode;
                        modeHovered = true;

                        text = GetDifficultyText(mode);
                        if (ClickingMouse)
                            SwitchToDifficulty(mode);
                    }
                }
            }

            if (!modeHovered)
                previouslyHoveredMode = null;
        }

        public static void DrawLock(SpriteBatch spriteBatch)
        {
            Texture2D lockTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicatorLock").Value;
            float rotationShift = lockClickTime == 0 ? 0f : (float)Math.Sin((1 - lockClickTime / (float)LockAnimLength) * MathHelper.TwoPi * 2f) * 0.5f * (lockClickTime / (float)LockAnimLength);
            spriteBatch.Draw(lockTexture, DrawCenter + Vector2.UnitY * 12 * MainIconScale, null, Color.White, 0f + rotationShift, lockTexture.Size() * 0.5f, LockShakeScale * MainIconScale, SpriteEffects.None, 0f);
        }
        #endregion

        #region Difficulty toggling
        public static string GetDifficultyText(DifficultyMode mode)
        {
            LocalizedText preface = mode.Name;
            if (mode == GetCurrentDifficulty)
                preface = CalamityUtils.GetText("UI.CurrentlySelected").WithFormatArgs(mode.Name.ToString());

            string text = "\n" + mode.ShortDescription.ToString();

            // Not scuffed anymore.
            if (mode.ExpandedDescription != LocalizedText.Empty)
            {
                // Show the description either if the player is holding shift.
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    text += "\n" + mode.ExpandedDescription.ToString();

                else
                    text += "\n" + CalamityUtils.GetTextValue("UI.DifficultyShiftText");
            }

            return preface.ToString() + text;
        }

        public static void SwitchToDifficulty(DifficultyMode mode)
        {
            //No swap on server (although this doesn't matter anymore since it's not an item but dnc.
            //No swap if the requested difficulty is the same as the current one.
            if (mode == GetCurrentDifficulty)
                return;

            // Todo, maybe in the future having a way to have multiple difficulty options on the same tier that can coexist, and it works in branching pathes? Not very necessary for cal & addons.
            // But would be super useful so other mods can let their own difficulties go there.

            // Disable difficulties.
            for (int i = 0; i < Difficulties.Length; i++)
            {
                // Disable all difficulties at the same tier / at a tier above itself.
                if (Difficulties[i]._difficultyTier >= mode._difficultyTier && Difficulties[i] != mode)
                {
                    if (Difficulties[i].Enabled)
                    {
                        if (Difficulties[i].DeactivationTextKey != string.Empty)
                            DisplayLocalizedText(Difficulties[i].DeactivationTextKey, Difficulties[i].ChatTextColor);

                        Difficulties[i].Enabled = false;
                    }
                }
            }

            // Look at all the lower tiers.
            for (int i = 0; i < mode._difficultyTier; i++)
            {
                // If there are no alternate difficulties at that tier, no need to ask, just choose that one.
                if (DifficultyTiers[i].Length == 1)
                {
                    DifficultyTiers[i][0].Enabled = true;
                }
                // If there are alternate difficulties, we ask nicely to know which one is the preffered one.
                else
                {
                    // First off, disable them all to avoid conflicts if one was already selected before.
                    for (int j = 0; j < DifficultyTiers[i].Length; j++)
                        DifficultyTiers[i][j].Enabled = false;

                    // Enable the one favored by the mode.
                    DifficultyTiers[i][mode.FavoredDifficultyAtTier(i)].Enabled = true;
                }
            }

            if (mode.ActivationTextKey != string.Empty)
                DisplayLocalizedText(mode.ActivationTextKey, mode.ChatTextColor);

            mode.Enabled = true;

            SoundEngine.PlaySound(mode.ActivationSound);
            CalamityNetcode.SyncCalamityWorldDifficulties(Main.myPlayer);

            menuOpen = false;
            menuOpenTransitionTime = MenuAnimLength;
        }

        #endregion
    }
}
