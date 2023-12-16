using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.NPCs.ExoMechs;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static CalamityMod.UI.DraedonSummoning.DraedonDialogRegistry;

namespace CalamityMod.UI.DraedonSummoning
{
    public partial class CodebreakerUI : ModSystem
    {
        public class DialogEntry
        {
            /// <summary>
            /// Whether this dialog entry is spoken by Draedon or not.
            /// </summary>
            public bool FromDraedon;

            /// <summary>
            /// The text of the dialog.
            /// </summary>
            public string Dialog;

            public DialogEntry(string dialog, bool fromDraedon)
            {
                Dialog = dialog;
                FromDraedon = fromDraedon;
            }
        }

        /// <summary>
        /// The scale of the overall communication panel.
        /// </summary>
        public static float CommunicationPanelScale
        {
            get;
            set;
        }

        /// <summary>
        /// The text creation timer. Once this reaches a value of <see cref="DraedonTextCreationRate"/> this value is set back to 0 and the next character in the dialog sequence is displayed.
        /// </summary>
        public static int DraedonTextCreationTimer
        {
            get;
            set;
        }

        /// <summary>
        /// The currently written text by Draedon. This string's contents are added to character-by-character. For the completed contents of this, refer to <see cref="FullDraedonText"/>.
        /// </summary>
        public static string WrittenDraedonText
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// The text that Draedon is spelling out. For the partially spelt out text, refer to <see cref="WrittenDraedonText"/>.
        /// </summary>
        public static string FullDraedonText
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// The countdown until the next character sound can be played.
        /// </summary>
        public static int DialogSoundDelay
        {
            get;
            set;
        }

        /// <summary>
        /// The intensity of the universal static overlay. This is drawn when communication is initiated, as an indicator of the monitor "powering up".
        /// </summary>
        public static float DraedonScreenStaticInterpolant
        {
            get;
            set;
        } = 1f;

        /// <summary>
        /// The opacity of text in the topic options menu.
        /// </summary>
        public static float OptionsTextOpacity
        {
            get;
            set;
        } = 1f;

        /// <summary>
        /// The vertical offset as a result of scrolling for the dialog.
        /// </summary>
        public static float DialogVerticalOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The vertical offset as a result of scrolling for the topic options.
        /// </summary>
        public static float OptionsTextVerticalOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// How much the <see cref="DialogHeight"/> of the dialog changed last frame.
        /// </summary>
        public static float LatestDialogHeightIncrease
        {
            get;
            set;
        }

        /// <summary>
        /// How much vertical space is taken up by the dialog. This includes space that is cut off and requires scrolling to view.
        /// </summary>
        public static float DialogHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// How much vertical space is taken up by the topic options text. This includes space that is cut off and requires scrolling to view.
        /// </summary>
        public static float OptionsTextHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// The scale of dialog text.
        /// </summary>
        public static Vector2 DialogTextScale => Vector2.One * GeneralScale * 0.75f;

        /// <summary>
        /// The previous character that Draedon spoke.
        /// </summary>
        public static char PreviousTextCharacter => WrittenDraedonText.Length >= 1 ? FullDraedonText[WrittenDraedonText.Length - 1] : ' ';

        /// <summary>
        /// The next character that Draedon will speak.
        /// </summary>
        public static char NextTextCharacter => WrittenDraedonText.Length < FullDraedonText.Length ? FullDraedonText[WrittenDraedonText.Length] : ' ';

        /// <summary>
        /// How much delay in frames there is between now and the next character based what the previous character was.<br></br>
        /// Most characters have a single frame delay, but things like newlines and sentence terminators have longer delays.
        /// </summary>
        public static int DraedonTextCreationRate
        {
            get
            {
                if (PreviousTextCharacter is '\n')
                    return 48;

                if (PreviousTextCharacter is '.' or '?')
                    return 9;

                return 1;
            }
        }

        /// <summary>
        /// The text Draedon starts off with when you interact with the codebreaker. The first time you see him he says nothing, but in successive calls he asks what your inquiry is.
        /// </summary>
        public static string InquiryText => Main.LocalPlayer.Calamity().HasTalkedAtCodebreaker ? "State your inquiry." : "...";

        /// <summary>
        /// The text of the previously hovered topic option. This is used to ensure hover clicking sounds only play if the sound type changes.
        /// </summary>
        public static string HoverSoundDialogType
        {
            get;
            set;
        } = null;

        /// <summary>
        /// The history of all dialog spoken since the player latest initiated communication with Draedon. The contents of this can be backtracked via scrolling.
        /// </summary>
        public static List<DialogEntry> DialogHistory
        {
            get;
            set;
        } = new();

        /// <summary>
        /// The scroller responsible for navigating topic options.
        /// </summary>
        public static CodebreakerUIScroller TopicOptionsScroller
        {
            get;
            internal set;
        } = new();

        /// <summary>
        /// The scroller responsible for navigating previously spoken dialog.
        /// </summary>
        public static CodebreakerUIScroller DialogScroller
        {
            get;
            internal set;
        } = new();

        /// <summary>
        /// The font used for displaying dialog.
        /// </summary>
        public static DynamicSpriteFont DialogFont
        {
            get;
            internal set;
        }

        /// <summary>
        /// The dialog hover sound.
        /// </summary>
        public static readonly SoundStyle DialogOptionHoverSound = new("CalamityMod/Sounds/Custom/Codebreaker/DialogOptionHover");

        /// <summary>
        /// Possible sounds Draedon can play when he speaks.
        /// </summary>
        public static readonly SoundStyle[] DraedonTalks = new SoundStyle[]
        {
            new("CalamityMod/Sounds/Custom/Codebreaker/DraedonTalk1"),
            new("CalamityMod/Sounds/Custom/Codebreaker/DraedonTalk2"),
            new("CalamityMod/Sounds/Custom/Codebreaker/DraedonTalk3")
        };

        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                DialogFont = CalamityMod.Instance.Assets.Request<DynamicSpriteFont>("Fonts/CodebreakerDialog", AssetRequestMode.ImmediateLoad).Value;
            else
                DialogFont = FontAssets.MouseText.Value;
        }

        public static void DisplayCommunicationPanel()
        {
            // Draw the background panel. This pops up.
            float panelWidthScale = Utils.Remap(CommunicationPanelScale, 0f, 0.5f, 0.085f, 1f);
            float panelHeightScale = Utils.Remap(CommunicationPanelScale, 0.5f, 1f, 0.085f, 1f);
            Vector2 panelScale = GeneralScale * new Vector2(panelWidthScale, panelHeightScale) * 1.4f;
            Texture2D panelTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonContactPanel").Value;
            float basePanelHeight = GeneralScale * panelTexture.Height * 1.4f;
            Vector2 panelCenter = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f + panelTexture.Height * panelScale.Y * 0.5f - basePanelHeight * 0.5f);
            Rectangle panelArea = Utils.CenteredRectangle(panelCenter, panelTexture.Size() * panelScale);

            Main.spriteBatch.Draw(panelTexture, panelCenter, null, Color.White, 0f, panelTexture.Size() * 0.5f, panelScale, 0, 0f);

            // Draw static if the static interpolant is sufficiently high.
            if (DraedonScreenStaticInterpolant > 0f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

                // Apply a glitch shader.
                MiscShaderData blueStatic = GameShaders.Misc["CalamityMod:BlueStatic"];
                blueStatic.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/SharpNoise"));
                blueStatic.Shader.Parameters["useStaticLine"].SetValue(false);
                blueStatic.Shader.Parameters["coordinateZoomFactor"].SetValue(0.5f);
                blueStatic.Shader.Parameters["useTrueNoise"].SetValue(true);
                blueStatic.Apply();

                float readjustedInterpolant = Utils.GetLerpValue(0.42f, 1f, DraedonScreenStaticInterpolant, true);
                Color staticColor = Color.White * (float)Math.Pow(CalamityUtils.AperiodicSin(readjustedInterpolant * 2.94f) * 0.5f + 0.5f, 0.54) * (float)Math.Pow(readjustedInterpolant, 0.51D);
                Main.spriteBatch.Draw(panelTexture, panelCenter, null, staticColor, 0f, panelTexture.Size() * 0.5f, panelScale, 0, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
            }

            // Disable clicks if hovering over the panel.
            if (panelArea.Intersects(MouseScreenArea))
                Main.blockMouse = Main.LocalPlayer.mouseInterface = true;

            DisplayDraedonFacePanel(panelCenter, panelScale);
            DisplayTextSelectionOptions(panelArea, panelScale);
            DisplayDialogHistory(panelArea, panelScale);
            if (OptionsTextOpacity > 0f && DraedonScreenStaticInterpolant <= 0f)
                DrawExitButton(panelCenter + new Vector2(-16f, 138f) * GeneralScale, OptionsTextOpacity);
        }

        public static void DisplayDraedonFacePanel(Vector2 panelCenter, Vector2 panelScale)
        {
            // Draw a panel that has Draedon's face.
            Texture2D iconTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonIconBorder").Value;
            Texture2D iconTextureInner = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonIconBorderInner").Value;
            float draedonIconDrawInterpolant = Utils.GetLerpValue(0.51f, 0.36f, DraedonScreenStaticInterpolant, true);
            Vector2 draedonIconDrawTopRight = panelCenter + new Vector2(-218f, -130f) * panelScale;
            draedonIconDrawTopRight += new Vector2(24f, 4f) * panelScale;

            Vector2 draedonIconScale = panelScale * 0.5f;
            Vector2 draedonIconCenter = draedonIconDrawTopRight + iconTexture.Size() * new Vector2(0.5f, 0.5f) * draedonIconScale;
            Rectangle draedonIconArea = Utils.CenteredRectangle(draedonIconCenter, iconTexture.Size() * draedonIconScale * 0.9f);
            Main.spriteBatch.Draw(iconTexture, draedonIconDrawTopRight, null, Color.White * draedonIconDrawInterpolant, 0f, Vector2.Zero, draedonIconScale, 0, 0f);

            // Draw Draedon's face inside the panel.
            // This involves restarting the sprite batch with a rasterizer state that can cut out Draedon's face if it exceeds the icon area.
            Main.spriteBatch.EnforceCutoffRegion(draedonIconArea, Matrix.Identity, SpriteSortMode.Immediate);

            // Apply a glitch shader.
            MiscShaderData teleportEffect = GameShaders.Misc["CalamityMod:TeleportDisplacement"];
            teleportEffect.UseOpacity(0.04f);
            teleportEffect.UseSecondaryColor(Color.White * 0.75f);
            teleportEffect.UseSaturation(0.75f);
            teleportEffect.Shader.Parameters["frameCount"].SetValue(Vector2.One);
            teleportEffect.Apply();

            Vector2 draedonScale = new Vector2(draedonIconDrawInterpolant, 1f) * Main.UIScale * 1.32f;
            SpriteEffects draedonDirection = SpriteEffects.FlipHorizontally;
            Texture2D draedonFaceTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/HologramDraedon").Value;

            Main.spriteBatch.Draw(draedonFaceTexture, draedonIconCenter, null, Color.White * draedonIconDrawInterpolant, 0f, draedonFaceTexture.Size() * 0.5f, draedonScale, draedonDirection, 0f);
            Main.spriteBatch.ReleaseCutoffRegion(Matrix.Identity, SpriteSortMode.Immediate);

            // Draw a glitch effect over the panel and Draedon's icon.
            MiscShaderData blueStatic = GameShaders.Misc["CalamityMod:BlueStatic"];
            blueStatic.UseColor(Color.Cyan);
            blueStatic.UseImage1("Images/Misc/noise");
            blueStatic.Shader.Parameters["useStaticLine"].SetValue(true);
            blueStatic.Shader.Parameters["coordinateZoomFactor"].SetValue(1f);
            blueStatic.Apply();
            Main.spriteBatch.Draw(iconTextureInner, draedonIconDrawTopRight, null, Color.White * draedonIconDrawInterpolant, 0f, Vector2.Zero, draedonIconScale, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
        }

        public static void DisplayTextSelectionOptions(Rectangle panelArea, Vector2 panelScale)
        {
            // Draw the outline for the text selection options.
            float selectionOptionsDrawInterpolant = Utils.GetLerpValue(0.3f, 0f, DraedonScreenStaticInterpolant, true);
            Texture2D selectionOutline = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonSelectionOutline").Value;
            Vector2 selectionCenter = panelArea.BottomLeft() - new Vector2(selectionOutline.Width * -0.5f - 24f, selectionOutline.Height * 0.5f + 24f) * panelScale;
            Rectangle selectionArea = Utils.CenteredRectangle(selectionCenter, selectionOutline.Size() * panelScale);
            Main.spriteBatch.Draw(selectionOutline, selectionCenter, null, Color.White * selectionOptionsDrawInterpolant, 0f, selectionOutline.Size() * 0.5f, panelScale, 0, 0f);

            // Update the options opacity.
            bool canChooseQuery = WrittenDraedonText.Length == FullDraedonText.Length;
            OptionsTextOpacity = MathHelper.Clamp(OptionsTextOpacity + canChooseQuery.ToDirectionInt() * 0.1f, 0f, 1f);

            // Enforce a cutoff on everything.
            Rectangle textCutoffRegion = selectionArea;
            textCutoffRegion.Y += 6;
            textCutoffRegion.Height -= 10;
            var rasterizer = Main.Rasterizer;
            rasterizer.ScissorTestEnable = true;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizer, null, Matrix.Identity);
            Main.spriteBatch.GraphicsDevice.ScissorRectangle = textCutoffRegion;

            // Draw and update the text scroller if there is some cut off text.
            Vector2 textTopLeft = selectionArea.TopLeft() + new Vector2(20f, 12f) * panelScale + Vector2.UnitY * OptionsTextVerticalOffset;
            float cutoffDistance = OptionsTextHeight / GeneralScale - selectionOutline.Height;
            if (cutoffDistance > 0f && OptionsTextOpacity > 0f)
            {
                TopicOptionsScroller.PositionYInterpolant = MathHelper.Clamp(OptionsTextVerticalOffset / -cutoffDistance, 0f, 1f);
                TopicOptionsScroller.Draw(selectionArea.Top + GeneralScale * 62f, selectionArea.Bottom - GeneralScale * 62f, selectionArea.Right - GeneralScale * 12f, GeneralScale * 0.8f, OptionsTextOpacity);
                OptionsTextVerticalOffset = TopicOptionsScroller.PositionYInterpolant * -cutoffDistance;
            }

            // Reset the options text height.
            OptionsTextHeight = 0f;

            bool hoveringOverAnyOption = false;
            float opacity = OptionsTextOpacity * (1f - DraedonScreenStaticInterpolant);
            Texture2D markerTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonInquirySelector").Value;
            Vector2 markerScale = panelScale * 0.24f;
            Vector2 markerDrawPositionOffset = Vector2.UnitX * markerTexture.Width * markerScale.X * 0.52f;
            Vector2 markerTextureSize = markerTexture.Size() * markerScale;
            float verticalOffsetPerOption = panelScale.Y * 12f;

            // Display text options in the box.
            foreach (var dialog in DialogOptions.Where(d => d.Condition()))
            {
                // Skip/isolate the introduction text as needed.
                string inquiry = dialog.Inquiry;
                if (!Main.LocalPlayer.Calamity().HasTalkedAtCodebreaker && !DialogHistory.Any(d => d.Dialog == DialogOptions[0].Response))
                {
                    if (inquiry != DialogOptions[0].Inquiry)
                        continue;

                    while (DialogHistory.Count < 1)
                        DialogHistory.Add(new(string.Empty, true));
                }
                else if (inquiry == DialogOptions[0].Inquiry)
                    continue;

                // Draw the text marker.
                Vector2 markerDrawPosition = textTopLeft - markerDrawPositionOffset;
                markerDrawPosition.Y += markerScale.Y * 22f;

                Color textColor = Color.Cyan;
                Color markerColor = Color.White;
                Vector2 textArea = DialogFont.MeasureString(inquiry) * GeneralScale;
                Rectangle textAreaRect = new((int)textTopLeft.X, (int)textTopLeft.Y, (int)(textArea.X * 0.9f), (int)textArea.Y);
                Rectangle markerArea = Utils.CenteredRectangle(markerDrawPosition, markerTextureSize);
                textAreaRect.Y = markerArea.Y;
                textAreaRect.Height = markerArea.Height;

                // Draw a bloom flare if the dialog hasn't been seen yet.
                dialog.Update();
                if (dialog.BloomOpacity > 0f)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Matrix.Identity);

                    // Collect bloom flare draw information and draw based on it.
                    Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/BloomFlare").Value;
                    float bloomTexScale = MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.05f + 0.26f;
                    float bloomTexRotation = Main.GlobalTimeWrappedHourly * 0.5f;
                    Main.spriteBatch.Draw(bloomTex, markerDrawPosition, null, Color.SteelBlue * dialog.BloomOpacity * opacity, bloomTexRotation, new Vector2(123f, 124f), bloomTexScale, 0, 0f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
                }

                // Check if the player clicks on the text or hovers over it.
                // If they're hovering over it, change the color to a yellow hover.
                // If they clicked, have the player select that option as the dialog to ask Draedon about.
                if ((MouseScreenArea.Intersects(textAreaRect) || MouseScreenArea.Intersects(markerArea)) && opacity >= 1f)
                {
                    textColor = Color.Lerp(textColor, Color.Yellow, 0.5f);
                    markerColor = Color.Yellow;

                    // Play the hover sound.
                    hoveringOverAnyOption = true;
                    if (HoverSoundDialogType != inquiry)
                    {
                        HoverSoundDialogType = inquiry;
                        SoundEngine.PlaySound(DialogOptionHoverSound);
                    }

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        if (!Main.LocalPlayer.Calamity().HasTalkedAtCodebreaker)
                        {
                            DialogHistory.Insert(0, new(string.Empty, true));
                            OptionsTextOpacity = 0f;
                        }

                        if (inquiry == DialogOptions[0].Inquiry)
                            DraedonTextCreationTimer = -72;

                        if (DialogHistory.Count <= 0)
                        {
                            DialogHistory.Add(new(inquiry, false));
                            DialogHistory.Add(new(string.Empty, true));
                        }
                        else
                        {
                            DialogHistory[^1] = new(inquiry, false);
                            DialogHistory.Add(new(string.Empty, true));
                        }

                        FullDraedonText = dialog.Response.Replace("\\n", "\n");
                        WrittenDraedonText = string.Empty;

                        // Ensure that the player starts at the bottom of the scoller, now that new text is generating there.
                        Texture2D dialogOutline = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonDialogOutline").Value;
                        Rectangle dialogArea = Utils.CenteredRectangle(selectionCenter, dialogOutline.Size() * panelScale);
                        DialogVerticalOffset = dialogArea.Height - DialogHeight * GeneralScale;
                        DialogScroller.PositionYInterpolant = 1f;
                        if (DialogVerticalOffset > 0f)
                            DialogVerticalOffset = 0f;

                        if (!Main.LocalPlayer.Calamity().SeenDraedonDialogs.Contains(dialog.ID))
                            Main.LocalPlayer.Calamity().SeenDraedonDialogs.Add(dialog.ID);
                    }
                }

                // Draw the marker texture next to the text.
                Vector2 markerTextureOrigin = markerTexture.Size() * 0.5f;
                Main.spriteBatch.Draw(markerTexture, markerDrawPosition, null, markerColor * opacity, 0f, markerTextureOrigin, markerScale, 0, 0f);

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, DialogFont, inquiry, textTopLeft, textColor * opacity, 0f, Vector2.Zero, Vector2.One * GeneralScale * 0.85f);
                textTopLeft.Y += verticalOffsetPerOption;
                OptionsTextHeight += verticalOffsetPerOption;
            }

            if (!hoveringOverAnyOption)
                HoverSoundDialogType = null;

            // Return the sprite batch to its default state.
            Main.spriteBatch.ReleaseCutoffRegion(Matrix.Identity);
        }

        public static void DisplayDialogHistory(Rectangle panelArea, Vector2 panelScale)
        {
            // Draw the outline for the dialog history.
            float dialogHistoryDrawInterpolant = Utils.GetLerpValue(0.3f, 0f, DraedonScreenStaticInterpolant, true);
            Texture2D dialogOutline = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonDialogOutline").Value;
            Vector2 selectionCenter = panelArea.TopRight() - new Vector2(dialogOutline.Width * 0.5f + 16f, dialogOutline.Height * -0.5f - 6.5f) * panelScale;
            Rectangle dialogArea = Utils.CenteredRectangle(selectionCenter, dialogOutline.Size() * panelScale);
            Main.spriteBatch.Draw(dialogOutline, selectionCenter, null, Color.White * dialogHistoryDrawInterpolant, 0f, dialogOutline.Size() * 0.5f, panelScale, 0, 0f);

            // Enforce a cutoff on everything.
            Rectangle textCutoffRegion = dialogArea;
            textCutoffRegion.Y += 6;
            textCutoffRegion.Height -= 10;
            var rasterizer = Main.Rasterizer;
            rasterizer.ScissorTestEnable = true;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizer, null, Matrix.Identity);
            Main.spriteBatch.GraphicsDevice.ScissorRectangle = textCutoffRegion;

            // Intialize Draedon's dialog if necessary.
            if (string.IsNullOrEmpty(FullDraedonText))
                FullDraedonText = InquiryText;

            // Type out Draedon text.
            if (DialogSoundDelay > 0)
                DialogSoundDelay--;
            if (DraedonScreenStaticInterpolant <= 0f)
                DraedonTextCreationTimer++;
            if (DraedonTextCreationTimer >= DraedonTextCreationRate && WrittenDraedonText.Length < FullDraedonText.Length)
            {
                char nextLetter = FullDraedonText[WrittenDraedonText.Length];
                WrittenDraedonText += nextLetter;
                DraedonTextCreationTimer = 0;

                // Initialize the dialog history if it's empty.
                if (DialogHistory.Count <= 0)
                    DialogHistory.Add(new(string.Empty, true));

                // Update the last dialog history entry as Draedon types.
                DialogHistory[^1].Dialog += nextLetter;

                // Shift the text downward if it exceeds the current bottom.
                if (DialogHeight * 1.2f >= textCutoffRegion.Height && !string.IsNullOrEmpty(nextLetter.ToString()) && LatestDialogHeightIncrease > 0f)
                    DialogVerticalOffset -= LatestDialogHeightIncrease;

                // Move to the next index in the dialog history once Draedon is finished speaking.
                if (WrittenDraedonText.Length >= FullDraedonText.Length)
                {
                    if (WrittenDraedonText == DialogOptions[0].Response)
                        Main.LocalPlayer.Calamity().HasTalkedAtCodebreaker = true;
                    DialogHistory.Add(new(string.Empty, true));
                }

                // Play a small dialog sound, similar to that of Undertale.
                if (DialogSoundDelay <= 0 && nextLetter != ' ' && nextLetter != '\n')
                {
                    SoundStyle playThisSound = Main.rand.Next(DraedonTalks);
                    SoundEngine.PlaySound(playThisSound with { Volume = 0.4f }, Main.LocalPlayer.Center);

                    DialogSoundDelay = 4;
                }
            }

            // Draw and update the text scroller if there is some cut off text.
            Vector2 textTopLeft = dialogArea.TopLeft() + new Vector2(20f, 14f) * panelScale;
            float cutoffDistance = DialogHeight - dialogOutline.Height;
            if (cutoffDistance > 0f && OptionsTextOpacity > 0f)
            {
                DialogScroller.PositionYInterpolant = MathHelper.Clamp(DialogVerticalOffset / -cutoffDistance, 0f, 1f);
                DialogScroller.Draw(dialogArea.Top + GeneralScale * 66f, dialogArea.Bottom - GeneralScale * 66f, dialogArea.Right - GeneralScale * 12f, GeneralScale * 0.8f, OptionsTextOpacity);
                DialogVerticalOffset = DialogScroller.PositionYInterpolant * -cutoffDistance;
            }

            // Display text in the box.
            var dialogEntries = DialogHistory.Where(d => !string.IsNullOrEmpty(d.Dialog));
            Texture2D markerTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonInquirySelector").Value;
            Vector2 markerScale = panelScale * 0.24f;
            Vector2 markerDrawPositionOffset = Vector2.UnitX * markerTexture.Width * markerScale.X * 0.6f;
            float markerDrawPositionOffsetY = markerScale.Y * 24f;
            float localTextOffsetY = markerScale.Y * 4f;
            Vector2 markerTextureOrigin = markerTexture.Size() * 0.5f;
            float panelOffsetPerLine = panelScale.Y * 10f;
            float panelOffsetPerEntry = panelScale.Y * 16f;

            // Store the top and bottom position of the text. This starts out as floating point extremes but is whittled down to the true
            // vertical range of the text in the loop below.
            float top = float.MaxValue;
            float bottom = float.MinValue;

            foreach (var entry in dialogEntries)
            {
                int lineIndex = 0;
                foreach (string line in Utils.WordwrapString(entry.Dialog, DialogFont, 336, 100, out _))
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    // Define a bunch of variables for text. These vary based on whether it's Draedon speaking or not.
                    bool textIsFromDraedon = entry.FromDraedon;
                    Color dialogColor = Draedon.TextColor * 1.25f;
                    Vector2 localTextTopLeft = textTopLeft + Vector2.UnitY * DialogVerticalOffset;
                    Vector2 markerDrawPosition = textTopLeft - markerDrawPositionOffset + Vector2.UnitY * DialogVerticalOffset;
                    markerDrawPosition.Y += markerDrawPositionOffsetY;
                    SpriteEffects markerDirection = SpriteEffects.None;
                    if (!textIsFromDraedon)
                    {
                        // Flip positions to the other side of the dialog outline if the text is being said by the player.
                        Vector2 anchorPoint = new(dialogArea.Center.X, markerDrawPosition.Y);
                        markerDrawPosition.X = anchorPoint.X + (anchorPoint.X - markerDrawPosition.X) - GeneralScale * 12f;
                        localTextTopLeft.X = anchorPoint.X + (anchorPoint.X - localTextTopLeft.X) - GeneralScale * 14f;
                        localTextTopLeft.X -= DialogFont.MeasureString(line).X * DialogTextScale.X;
                        localTextTopLeft.Y -= localTextOffsetY;

                        // Use a neutral grey-ish color if text is being said by the player.
                        dialogColor = Color.LightGray;

                        markerDirection = SpriteEffects.FlipHorizontally;
                    }

                    // Draw the text marker.
                    if (lineIndex <= 0)
                        Main.spriteBatch.Draw(markerTexture, markerDrawPosition, null, Color.White * dialogHistoryDrawInterpolant, 0f, markerTextureOrigin, markerScale, markerDirection, 0f);

                    // Draw the text itself.
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, DialogFont, line, localTextTopLeft, dialogColor * dialogHistoryDrawInterpolant, 0f, Vector2.Zero, DialogTextScale);

                    textTopLeft.Y += panelOffsetPerLine;
                    lineIndex++;

                    // Improve the calculation of the top and bottom of the dialog.
                    top = MathF.Min(top, localTextTopLeft.Y);
                    bottom = MathF.Max(bottom, localTextTopLeft.Y);
                }

                textTopLeft.Y += panelOffsetPerEntry;
            }

            // Store the dialog text height, along with how much the height changed this frame.
            LatestDialogHeightIncrease = bottom - top - DialogHeight;
            DialogHeight = bottom - top;

            // Return the sprite batch to its default state.
            Main.spriteBatch.ReleaseCutoffRegion(Matrix.Identity);
        }
    }
}
