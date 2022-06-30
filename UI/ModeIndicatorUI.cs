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

namespace CalamityMod.UI
{
    public class ModeIndicatorUI
    {
        public enum DifficultyMode { None, Revengeance, Death, Malice, Current}
        private static DifficultyMode GetCurrentDifficulty
        {
            get 
            { 
                DifficultyMode mode = DifficultyMode.None;

                if (CalamityWorld.revenge)
                    mode = DifficultyMode.Revengeance;
                if (CalamityWorld.death)
                    mode = DifficultyMode.Death;
                if (CalamityWorld.malice)
                    mode = DifficultyMode.Malice;

                return mode;
            }
        }

        private static Rectangle GetFrame(DifficultyMode mode = DifficultyMode.Current)
        {
            if (mode == DifficultyMode.Current)
                mode = GetCurrentDifficulty;

            int indicatorFrame = (int)mode;

            return new Rectangle(0, 38 * indicatorFrame, 30, 38);
        }

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static Vector2 FrameSize => new Vector2(30f, 38f);
        public static Rectangle MainClickArea => Utils.CenteredRectangle(DrawCenter, FrameSize * MainIconScale);
        public static bool ClickingMouse => Main.mouseLeft && Main.mouseLeftRelease;
        public static Vector2 DrawCenter => new Vector2(Main.screenWidth - 400f, 82f) + FrameSize * 0.5f;

        //Lock icon
        internal const int LockAnimLenght = 30;
        private static int lockClickTime = 0;
        private static bool previousLockStatus = false;
        //Expand and shrink on hover
        internal const float MaxIconHoverScaleBoost = 0.3f;
        internal const float IconHoverScaleIncrement = MaxIconHoverScaleBoost / 6f;
        internal const float IconHoverScaleDecrement = MaxIconHoverScaleBoost / 10f;
        private static float iconHoverScaleBoost = 0f;
        public static float MainIconScale => 1f + iconHoverScaleBoost;
        //Menu state
        private static bool menuOpen = false;
        internal const int MenuAnimLenght = 40;
        private static int menuOpenTransitionTime = 0;

        private static void ClearVariables()
        {
            lockClickTime = 0;
            menuOpenTransitionTime = 0;
            previousLockStatus = false;
            iconHoverScaleBoost = 0f;
            menuOpen = false;
        }

        public static void TickVariables()
        {
            if (lockClickTime > 0)
                lockClickTime--;

            if (menuOpenTransitionTime > 0)
                menuOpenTransitionTime--;

            if (iconHoverScaleBoost > 0)
                iconHoverScaleBoost -= IconHoverScaleDecrement;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            // The mode indicator should only be displayed when the inventory is open, to prevent obstruction.
            if (!Main.playerInventory)
            {
                ClearVariables();
                return;
            }

            Texture2D indicatorTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicator").Value;
            Rectangle indicatorFrameArea = GetFrame();

            GetDifficultyStatus(out string difficultyText);
            GetLockStatus(out string lockText, out bool locked);

            //Grows the icon when hovering it.
            if (iconHoverScaleBoost < MaxIconHoverScaleBoost && MouseScreenArea.Intersects(MainClickArea))
            {
                iconHoverScaleBoost = Math.Min(iconHoverScaleBoost + IconHoverScaleIncrement + IconHoverScaleDecrement, MaxIconHoverScaleBoost);

                if (ClickingMouse && menuOpenTransitionTime == 0  && !locked)
                {
                    menuOpenTransitionTime = MenuAnimLenght;
                    menuOpen = !menuOpen;
                }
            }

            string extraDescText = "";
            if (menuOpenTransitionTime > 0 || menuOpen)
                ManageHexIcons(spriteBatch, out extraDescText);

            //Draw the indicator itself.
            spriteBatch.Draw(indicatorTexture, DrawCenter, indicatorFrameArea, Color.White, 0f, indicatorFrameArea.Size() * 0.5f, MainIconScale, SpriteEffects.None, 0f);

            if (locked)
                DrawLock(spriteBatch);

            //Add the hover text.
            if (difficultyText != "")
            {
                difficultyText += "\n" + lockText;
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText(difficultyText);
            }

            else if (extraDescText != "")
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText(extraDescText);
            }


            TickVariables();
        }

        public static void GetDifficultyStatus(out string text)
        {
            text = "";
            if (MouseScreenArea.Intersects(MainClickArea))
            {
                string modeToDisplay = "Revengeance";
                bool modeIsActive = CalamityWorld.revenge;
                if (CalamityWorld.death)
                {
                    modeToDisplay = "Death";
                    modeIsActive = CalamityWorld.death;
                }
                if (CalamityWorld.malice)
                {
                    modeToDisplay = "Malice";
                    modeIsActive = CalamityWorld.malice;
                }

                text = $"{modeToDisplay} Mode is {(modeIsActive ? "active" : "not active")}.";
            }
        }

        //Checks if the mode change is available or not, and jiggles the lock icon if the player clicks on it.
        public static void GetLockStatus(out string text, out bool locked)
        {
            locked = false;
            text = "Click to select a difficulty mode";
            if (!Main.expertMode && !CalamityWorld.revenge)
            {
                locked = true;
                text = "Higher difficulty modes can only be toggled in expert mode or above";
            }
            else if (CalamityPlayer.areThereAnyDamnBosses || BossRushEvent.BossRushActive)
            {
                locked = true;
                text = Language.GetTextValue("Mods.CalamityMod.ChangingTheRules");
            }

            if (locked != previousLockStatus && lockClickTime == 0)
                lockClickTime = LockAnimLenght;

            previousLockStatus = locked;

            if (locked && menuOpen)
            {
                menuOpen = false;
                menuOpenTransitionTime = MenuAnimLenght - menuOpenTransitionTime;
            }
        }

        #region Cool drawing stuff
        private static CurveSegment lockGrow = new CurveSegment(SineOutEasing, 0f, 1f, 0.4f);
        private static CurveSegment lockShrink = new CurveSegment(SineInEasing, 0.6f, 1.4f, -0.4f);
        private static CurveSegment lockBump = new CurveSegment(SineBumpEasing, 0.9f, 1f, -0.2f);
        internal static float LockShakeScale => PiecewiseAnimation(lockClickTime / (float)LockAnimLenght, new CurveSegment[] { lockGrow, lockShrink, lockBump });

        private static CurveSegment barExpand = new CurveSegment(SineInOutEasing, 0f, 0f, 1f);
        internal static float BarExpansionProgress => PiecewiseAnimation(menuOpen ? 1 - (menuOpenTransitionTime / (float)MenuAnimLenght) : (menuOpenTransitionTime / (float)MenuAnimLenght), new CurveSegment[] { barExpand });

        public static void ManageHexIcons(SpriteBatch spriteBatch, out string text)
        {
            float barLenght = 240f * BarExpansionProgress;
            float progress = menuOpen ? 1 - menuOpenTransitionTime / (float)MenuAnimLenght : menuOpenTransitionTime / (float)MenuAnimLenght;
            Vector2 position = DrawCenter + (barLenght / 5f) * Vector2.UnitY;

            Texture2D indicatorTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicator").Value;
            Texture2D outlineTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicatorOutline").Value;

            text = "";

            for (int i = 0; i < 4; i++)
            {
                float usedOpacity = 0.85f;
                Rectangle indicatorFrameArea = GetFrame((DifficultyMode)i);

                //outline the currently selected difficulty.
                if ((DifficultyMode)i == GetCurrentDifficulty)
                {
                    usedOpacity = 1f;
                    spriteBatch.Draw(outlineTexture, position, null, Color.Crimson * 0.8f * progress, 0f, outlineTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                }

                spriteBatch.Draw(indicatorTexture, position, indicatorFrameArea, Color.White * usedOpacity * progress, 0f, indicatorFrameArea.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

                if (menuOpenTransitionTime == 0 && MouseScreenArea.Intersects(Utils.CenteredRectangle(position, indicatorFrameArea.Size())))
                {
                    text = GetDifficultyText((DifficultyMode)i);
                    if (ClickingMouse)
                        SwitchToDifficulty((DifficultyMode)i);
                }

                position += barLenght / 5f * Vector2.UnitY;
            }
        }

        public static void DrawLock(SpriteBatch spriteBatch)
        {
            if (ClickingMouse && lockClickTime == 0)
            {
                if (MouseScreenArea.Intersects(MainClickArea))
                    lockClickTime = LockAnimLenght;
            }

            Texture2D lockTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicatorLock").Value;
            float rotationShift = lockClickTime == 0 ? 0f : (float)Math.Sin((1 - lockClickTime / (float)LockAnimLenght) * MathHelper.TwoPi * 2f) * 0.5f * (lockClickTime / (float)LockAnimLenght);
            spriteBatch.Draw(lockTexture, DrawCenter + Vector2.UnitY * 12 * MainIconScale, null, Color.White, 0f + rotationShift, lockTexture.Size() * 0.5f, LockShakeScale * MainIconScale, SpriteEffects.None, 0f);
        }
        #endregion

        #region Difficulty toggling
        public static string GetDifficultyText(DifficultyMode mode)
        {
            string preface = "";
            if (mode == GetCurrentDifficulty)
                preface = "Currently Selected : ";

            string text = "";
            if (mode == DifficultyMode.None)
            {
                text = "Disabled\nThe classic Terraria experience, with no Calamity difficulty changes";
            }
            if (mode == DifficultyMode.Revengeance)
                text = "Revengeance\n[c/F7412D:Just like in metal gear haha nanomachines son! Standing here i realize!]";
            if (mode == DifficultyMode.Death)
                text = "Death\n[c/C82DF7:Death mode is a pretty cool mode, it does...this. Modify this file with a description of your mod.]";
            if (mode == DifficultyMode.Malice)
                text = "Malice\n[c/F7CF2D:You should Kill yourself..... NOW!]";


            return preface + text;
        }

        public static void SwitchToDifficulty(DifficultyMode mode)
        {
            //No swap on server (although this doesn't matter anymore since it's not an item but dnc.
            //No swap if fsr you're asking to swap to the "current" difficulty
            //No swap if the requested difficulty is the same as the current one
            if (Main.netMode == NetmodeID.MultiplayerClient || mode == DifficultyMode.Current || mode == GetCurrentDifficulty)
                return;

            if (mode == DifficultyMode.Revengeance)
            {
                ToggleRev(true);
                ToggleDeath(false);
                ToggleMalice(false);
            }

            if (mode == DifficultyMode.Death)
            {
                ToggleRev(true, true);
                ToggleDeath(true);
                ToggleMalice(false);
            }

            if (mode == DifficultyMode.Malice)
            {
                ToggleRev(true, true);
                ToggleDeath(true, true);
                ToggleMalice(true);
            }

            if (mode == DifficultyMode.None)
            {
                ToggleMalice(false);
                ToggleDeath(false);
                ToggleRev(false);
            }

            SoundEngine.PlaySound(SoundID.Item119);
            CalamityNetcode.SyncWorld();
        }

        public static void ToggleRev(bool turnOn, bool noText = false)
        {
            if ((turnOn && CalamityWorld.revenge && noText) || !turnOn && !CalamityWorld.revenge)
                return;

            if (turnOn)
            {
                CalamityWorld.revenge = true;
                if (!noText)
                {
                    string key = "Mods.CalamityMod.RevengeText";
                    Color messageColor = Color.Crimson;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }

            else
            {
                CalamityWorld.revenge = false;
                if (!noText)
                {
                    string key = "Mods.CalamityMod.RevengeText2";
                    Color messageColor = Color.Crimson;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }
        }

        public static void ToggleDeath(bool turnOn, bool noText = false)
        {
            if ((turnOn && CalamityWorld.death && noText) || !turnOn && !CalamityWorld.death)
                return;


            if (turnOn)
            {
                CalamityWorld.death = true;
                if (!noText)
                {
                    string key = "Mods.CalamityMod.DeathText";
                    Color messageColor = Color.Crimson;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }

            else
            {
                CalamityWorld.death = false;
                if (!noText)
                {
                    string key = "Mods.CalamityMod.DeathText2";
                    Color messageColor = Color.Crimson;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }
        }

        public static void ToggleMalice(bool turnOn, bool noText = false)
        {
            if ((turnOn && CalamityWorld.malice && noText) || !turnOn && !CalamityWorld.malice)
                return;

            if (turnOn)
            {
                CalamityWorld.malice = true;
                if (!noText)
                {
                    string key = "Mods.CalamityMod.MaliceText";
                    Color messageColor = Color.Crimson;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }

            else
            {
                CalamityWorld.malice = false;
                if (!noText)
                {
                    string key = "Mods.CalamityMod.MaliceText2";
                    Color messageColor = Color.Crimson;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }
        }
#endregion
    }
}
