using System;
using System.Linq;
using System.Text;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.TileEntities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.UI.DraedonSummoning
{
    public partial class CodebreakerUI : ModSystem
    {
        public static int ViewedTileEntityID
        {
            get;
            set;
        } = -1;

        public static bool AwaitingCloseConfirmation
        {
            get;
            set;
        } = false;

        public static bool AwaitingDecryptionTextClose
        {
            get;
            set;
        } = false;

        public static bool DisplayingCommunicationText
        {
            get;
            set;
        } = false;

        public static float VerificationButtonScale
        {
            get;
            set;
        } = 1f;

        public static float ExitButtonScale
        {
            get;
            set;
        } = 1f;

        public static float ContactButtonScale
        {
            get;
            set;
        } = 1f;

        public static float CommunicateButtonScale
        {
            get;
            set;
        } = 1f;

        public static float CancelButtonScale
        {
            get;
            set;
        } = 1f;

        public static float MechIconScale
        {
            get;
            set;
        } = 1f;

        // This variable is currently permanently set to false due to being deemed unfinished and not fit for release.
        // It used to be a local variable but has been moved to a property so that addon mods can easily enable it.
        public static bool DraedonTalkFeatureEnabled
        {
            get;
            set;
        }

        public static Vector2 BackgroundCenter => new(500f, Main.screenHeight * 0.5f + 115f);

        public static float GeneralScale => MathHelper.Lerp(1f, 0.7f, Utils.GetLerpValue(1325f, 750f, Main.screenWidth, true)) * Main.UIScale;

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static readonly SoundStyle SummonSound = new("CalamityMod/Sounds/Custom/CodebreakerBeam");

        public static readonly SoundStyle BloodSound = new("CalamityMod/Sounds/Custom/Codebreaker/BloodForHekate");

        public static void Draw(SpriteBatch spriteBatch)
        {
            // If not viewing the specific tile entity's interface anymore, if the ID is for some reason invalid, or if the player is not equipped to continue viewing the UI
            // don't do anything other than resetting necessary data.
            if (!TileEntity.ByID.ContainsKey(ViewedTileEntityID) || TileEntity.ByID[ViewedTileEntityID] is not TECodebreaker codebreakerTileEntity || !Main.LocalPlayer.WithinRange(codebreakerTileEntity.Center, 270f) || !Main.playerInventory)
            {
                VerificationButtonScale = 1f;
                CancelButtonScale = 0.75f;
                ContactButtonScale = 1f;
                CommunicateButtonScale = 1f;
                ExitButtonScale = 1f;
                CommunicationPanelScale = 0f;
                ViewedTileEntityID = -1;
                AwaitingCloseConfirmation = false;
                DisplayingCommunicationText = false;
                MechIconScale = 1f;
                DialogScroller.Reset();
                TopicOptionsScroller.Reset();
                DialogVerticalOffset = 0f;
                OptionsTextVerticalOffset = 0f;
                DialogHeight = 0f;
                LatestDialogHeightIncrease = 0f;
                return;
            }

            // Draw the background.
            Texture2D backgroundTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonDecrypterBackground").Value;
            spriteBatch.Draw(backgroundTexture, BackgroundCenter, null, Color.White, 0f, backgroundTexture.Size() * 0.5f, GeneralScale * (1f - CommunicationPanelScale), 0, 0f);

            Rectangle backgroundArea = Utils.CenteredRectangle(BackgroundCenter, backgroundTexture.Size() * GeneralScale);
            if (MouseScreenArea.Intersects(backgroundArea) && !DisplayingCommunicationText)
                Main.blockMouse = Main.LocalPlayer.mouseInterface = true;

            // Display communication stuff as necessary.
            if (DisplayingCommunicationText && CommunicationPanelScale == 0f)
            {
                CommunicationPanelScale = 1f;
                DraedonScreenStaticInterpolant = 1f;
            }
            if (!DisplayingCommunicationText && CommunicationPanelScale != 0f)
            {
                CommunicationPanelScale = 0f;
                DraedonScreenStaticInterpolant = 0f;
            }

            // Disable the codebreaker UI's typical functions if currently speaking with Draedon, ignoring everything else in this method.
            if (DisplayingCommunicationText)
            {
                DisplayCommunicationPanel();
                DraedonScreenStaticInterpolant = MathHelper.Clamp(DraedonScreenStaticInterpolant - 0.01408f, 0f, 1f);
                return;
            }

            // Reset communication things.
            DraedonTextCreationTimer = 0;
            if (!string.IsNullOrEmpty(WrittenDraedonText) && FullDraedonText == DraedonDialogRegistry.DialogOptions[0].Inquiry)
                Main.LocalPlayer.Calamity().HasTalkedAtCodebreaker = true;

            WrittenDraedonText = FullDraedonText = string.Empty;
            DialogHistory.Clear();

            bool canSummonDraedon = codebreakerTileEntity.ReadyToSummonDraedon && CalamityWorld.AbleToSummonDraedon;
            bool canTalkToDraedon = codebreakerTileEntity.ReadyToSummonDraedon && DownedBossSystem.downedExoMechs && DraedonTalkFeatureEnabled;
            Vector2 backgroundTopLeft = BackgroundCenter - backgroundTexture.Size() * GeneralScale * 0.5f;

            // Draw the cell payment slot icon.
            Texture2D emptyCellIconTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonsArsenal/PowerCellSlot_Empty").Value;
            Texture2D occupiedCellIconTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonsArsenal/PowerCellSlot_Filled").Value;
            Texture2D bloodSampleIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/BloodSample").Value;
            Texture2D cellTexture = emptyCellIconTexture;
            if (codebreakerTileEntity.InputtedCellCount > 0)
            {
                if (codebreakerTileEntity.ContainsBloodSample)
                {
                    cellTexture = bloodSampleIconTexture;
                }
                else
                {
                    cellTexture = occupiedCellIconTexture;
                }
            }
            Vector2 cellDrawCenter = backgroundTopLeft + Vector2.One * GeneralScale * 60f;

            Vector2 schematicSlotDrawCenter = cellDrawCenter + Vector2.UnitY * GeneralScale * 70f;
            Vector2 costDisplayLocation = schematicSlotDrawCenter + Vector2.UnitY * GeneralScale * 20f;
            Vector2 costVerificationLocation = costDisplayLocation + Vector2.UnitY * GeneralScale * 60f;
            Vector2 summonButtonCenter = backgroundTopLeft + new Vector2(58f, backgroundTexture.Height - 48f) * GeneralScale;
            Vector2 talkButtonCenter = summonButtonCenter + Vector2.UnitX * GeneralScale * 172f;

            // Display some error text if the codebreaker isn't strong enough to decrypt the schematic.
            if (codebreakerTileEntity.HeldSchematicID != 0 && !codebreakerTileEntity.CanDecryptHeldSchematic)
                DisplayNotStrongEnoughErrorText(schematicSlotDrawCenter + new Vector2(-24f, 56f));

            // Handle decryption costs.
            else if (codebreakerTileEntity.HeldSchematicID != 0 && codebreakerTileEntity.DecryptionCountdown == 0 && !codebreakerTileEntity.ContainsBloodSample)
            {
                int cost = codebreakerTileEntity.DecryptionCellCost;
                DisplayCostText(costDisplayLocation, cost);

                if (codebreakerTileEntity.InputtedCellCount >= cost)
                {
                    if (canSummonDraedon)
                    {
                        costVerificationLocation.X -= GeneralScale * 15f;
                        summonButtonCenter.X += GeneralScale * 15f;
                    }
                    DrawCostVerificationButton(codebreakerTileEntity, costVerificationLocation);
                }
            }
            else if (codebreakerTileEntity.DecryptionCountdown > 0)
                DisplayDecryptCancelButton(codebreakerTileEntity, costVerificationLocation - Vector2.UnitY * GeneralScale * 30f);

            if (canSummonDraedon)
                HandleDraedonSummonButton(codebreakerTileEntity, summonButtonCenter);
            if (canTalkToDraedon)
                HandleDraedonTalkButton(talkButtonCenter);

            if (codebreakerTileEntity.DecryptionCountdown > 0 || AwaitingDecryptionTextClose)
                HandleDecryptionStuff(codebreakerTileEntity, backgroundTexture, backgroundTopLeft, schematicSlotDrawCenter + Vector2.UnitY * GeneralScale * 80f);
            if (codebreakerTileEntity.DecryptionCountdown > 0 && AwaitingCloseConfirmation)
                DrawDecryptCancelConfirmationText(costVerificationLocation);

            // Draw the schematic icon.
            Texture2D schematicIconBG = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/EncryptedSchematicSlotBackground").Value;
            Texture2D schematicIconTexture = schematicIconBG;
            int schematicType = 0;

            if (codebreakerTileEntity.HeldSchematicID > 0)
                schematicType = CalamityLists.EncryptedSchematicIDRelationship[codebreakerTileEntity.HeldSchematicID];

            if (schematicType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicPlanetoid").Value;
            if (schematicType == ModContent.ItemType<EncryptedSchematicJungle>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicJungle").Value;
            if (schematicType == ModContent.ItemType<EncryptedSchematicHell>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicHell").Value;
            if (schematicType == ModContent.ItemType<EncryptedSchematicIce>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicIce").Value;

            spriteBatch.Draw(schematicIconBG, schematicSlotDrawCenter, null, Color.White, 0f, schematicIconBG.Size() * 0.5f, GeneralScale, 0, 0f);
            if (codebreakerTileEntity.HeldSchematicID != 0)
                spriteBatch.Draw(schematicIconTexture, schematicSlotDrawCenter, null, Color.White, 0f, schematicIconTexture.Size() * 0.5f, GeneralScale, 0, 0f);
            HandleSchematicSlotInteractions(codebreakerTileEntity, schematicSlotDrawCenter, cellTexture.Size() * GeneralScale);

            // Create a temporary item for drawing purposes.
            // If cells are present, make the item reflect that.
            Item temporaryPowerCell = new Item();
            if (codebreakerTileEntity.ContainsBloodSample)
                temporaryPowerCell.SetDefaults(ModContent.ItemType<BloodSample>());
            else
                temporaryPowerCell.SetDefaults(ModContent.ItemType<DraedonPowerCell>());

            // Copy the cell stack from the amount of cells in the tile entity.
            temporaryPowerCell.stack = codebreakerTileEntity.InputtedCellCount;

            Vector2 cellInteractionArea = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonsArsenal/PowerCellSlot_Empty").Value.Size() * GeneralScale;
            CalamityUtils.DrawPowercellSlot(spriteBatch, temporaryPowerCell, cellDrawCenter, GeneralScale);
            HandleCellSlotInteractions(codebreakerTileEntity, temporaryPowerCell, cellDrawCenter, cellInteractionArea);

            // Draw the exit button.
            // The prevent confusion, this does not draw if the player is attempting to cancel an ongoing decryption.
            if (!AwaitingCloseConfirmation)
                DrawExitButton(Vector2.Lerp(summonButtonCenter, talkButtonCenter, 0.5f), 1f);
        }

        public static void DrawExitButton(Vector2 drawPosition, float opacity)
        {
            Texture2D cancelButton = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DecryptCancelIcon").Value;
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, cancelButton.Size() * VerificationButtonScale);

            // Check if the mouse is hovering over the exit button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                ExitButtonScale = MathHelper.Clamp(ExitButtonScale + 0.035f, 1f, 1.4f);

                // If a click is done, leave the Codebreaker UI.
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    ViewedTileEntityID = -1;
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
                ExitButtonScale = MathHelper.Clamp(ExitButtonScale - 0.05f, 1f, 1.4f);

            // Draw the exit button.
            Main.spriteBatch.Draw(cancelButton, drawPosition, null, Color.White, 0f, cancelButton.Size() * 0.5f, ExitButtonScale * GeneralScale, 0, 0f);

            // And display a text indicator that describes the function of the button.
            string exitText = CalamityUtils.GetTextValue("UI.Exit");

            // Center the draw position.
            drawPosition.X -= FontAssets.MouseText.Value.MeasureString(exitText).X * GeneralScale * 0.5f;
            drawPosition.Y += GeneralScale * 20f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, exitText, drawPosition.X, drawPosition.Y, Color.Red * opacity, Color.Black * opacity, Vector2.Zero, GeneralScale);
        }

        public static void HandleCellSlotInteractions(TECodebreaker codebreakerTileEntity, Item temporaryItem, Vector2 cellIconCenter, Vector2 area)
        {
            Rectangle clickArea = Utils.CenteredRectangle(cellIconCenter, area);

            // If the mouse is not in the specific area don't do anything else.
            if (!MouseScreenArea.Intersects(clickArea))
                return;

            ref Item playerHandItem = ref Main.mouseItem;

            if (!temporaryItem.IsAir)
                Main.HoverItem = temporaryItem;

            if (Main.mouseLeft && Main.mouseLeftRelease && codebreakerTileEntity.DecryptionCountdown <= 0)
            {
                int powercellID = ModContent.ItemType<DraedonPowerCell>();
                int sampleID = ModContent.ItemType<BloodSample>();
                short cellStackDiff = 0;
                bool shouldPlaySound = true;

                // If the player is holding shift and has space for the power cells, just spawn all of them on his or her face, up to the max-stack limit.
                if (Main.keyState.PressingShift() && Main.LocalPlayer.ItemSpace(temporaryItem).CanTakeItemToPersonalInventory)
                {
                    cellStackDiff = (short)-Math.Min(temporaryItem.stack, temporaryItem.maxStack);
                    Player p = Main.LocalPlayer;
                    var source = p.GetSource_TileInteraction(codebreakerTileEntity.Position.X, codebreakerTileEntity.Position.Y);
                    p.QuickSpawnItem(source, codebreakerTileEntity.ContainsBloodSample ? sampleID : powercellID, -cellStackDiff);

                    // Do not play a sound in this situation. The player is going to pick up the dropped cells in a few frames, which will make sound.
                    shouldPlaySound = false;
                }

                // If the slot is normally clicked, behavior depends on whether the player is holding power cells.
                else
                {
                    bool holdingPowercell = playerHandItem.type == powercellID || (playerHandItem.type == sampleID && Main.zenithWorld);
                    bool powercellsinserted = !codebreakerTileEntity.ContainsBloodSample && temporaryItem.stack > 0;
                    bool cansummon = codebreakerTileEntity.ReadyToSummonDraedon && CalamityWorld.AbleToSummonDraedon;

                    // If the player's held power cells can be stacked on top of what's already in the codeberaker, then stack them.
                    if (holdingPowercell && temporaryItem.stack < TECodebreaker.MaxCellCapacity)
                    {
                        // If theres no power cells inside, it's GFB, and the player has a blood sample, it can be inserted
                        if (playerHandItem.type == sampleID && Main.zenithWorld && !powercellsinserted && cansummon)
                        {
                            // Play a gross sound if no samples are in yet
                            if (temporaryItem.stack == 0)
                            {
                                SoundEngine.PlaySound(BloodSound, codebreakerTileEntity.Center);
                            }
                            codebreakerTileEntity.ContainsBloodSample = true;

                            int spaceLeft = TECodebreaker.MaxCellCapacity - temporaryItem.stack;

                            // If the player has more cells than there is space left, insert as many as possible. Otherwise insert all the cells.
                            int cellsToInsert = Math.Min(playerHandItem.stack, spaceLeft);
                            cellStackDiff = (short)cellsToInsert;
                            playerHandItem.stack -= cellsToInsert;
                            if (playerHandItem.stack == 0)
                                playerHandItem.TurnToAir();
                            AwaitingDecryptionTextClose = false;
                        }
                        // If theres nothing inside or there are cells inside, cells can be inserted
                        if (playerHandItem.type == powercellID && (temporaryItem.stack == 0 || powercellsinserted))
                        {
                            codebreakerTileEntity.ContainsBloodSample = false;

                            int spaceLeft = TECodebreaker.MaxCellCapacity - temporaryItem.stack;

                            // If the player has more cells than there is space left, insert as many as possible. Otherwise insert all the cells.
                            int cellsToInsert = Math.Min(playerHandItem.stack, spaceLeft);
                            cellStackDiff = (short)cellsToInsert;
                            playerHandItem.stack -= cellsToInsert;
                            if (playerHandItem.stack == 0)
                                playerHandItem.TurnToAir();
                            AwaitingDecryptionTextClose = false;
                        }
                    }

                    // If the player is holding nothing, then pick up all the power cells (if any exist), up to the max-stack limit.
                    else if (playerHandItem.IsAir && temporaryItem.stack > 0)
                    {
                        cellStackDiff = (short)-temporaryItem.stack;
                        if (cellStackDiff < -temporaryItem.maxStack)
                            cellStackDiff = (short)-temporaryItem.maxStack;

                        playerHandItem.SetDefaults(temporaryItem.type);
                        playerHandItem.stack = -cellStackDiff;
                        temporaryItem.TurnToAir();
                        AwaitingDecryptionTextClose = false;
                        codebreakerTileEntity.ContainsBloodSample = false;
                    }
                }

                if (cellStackDiff != 0)
                {
                    if (shouldPlaySound)
                        SoundEngine.PlaySound(SoundID.Grab);
                    AwaitingDecryptionTextClose = false;
                    codebreakerTileEntity.InputtedCellCount += cellStackDiff;
                    codebreakerTileEntity.SyncContainedStuff();
                }
            }

            // Force the hover item to be drawn.
            // Since HoverItem is active, we don't need to input anything into this method.
            if (temporaryItem.stack > 0)
                Main.instance.MouseTextHackZoom(string.Empty);
        }

        public static void HandleSchematicSlotInteractions(TECodebreaker codebreakerTileEntity, Vector2 schematicIconCenter, Vector2 area)
        {
            Rectangle clickArea = Utils.CenteredRectangle(schematicIconCenter, area);

            // If the mouse is not in the specific area don't do anything else.
            if (!MouseScreenArea.Intersects(clickArea))
                return;

            ref Item playerHandItem = ref Main.mouseItem;

            // Handle mouse click interactions.
            if (Main.mouseLeft && Main.mouseLeftRelease && codebreakerTileEntity.DecryptionCountdown <= 0)
            {
                // If the player's hand item is empty and the codebreaker has a schematic, grab it.
                // This doesn't work if the Codebreaker is busy decrypting the schematic in question.
                if (playerHandItem.IsAir && CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(codebreakerTileEntity.HeldSchematicID) && codebreakerTileEntity.DecryptionCountdown <= 0)
                {
                    playerHandItem.SetDefaults(CalamityLists.EncryptedSchematicIDRelationship[codebreakerTileEntity.HeldSchematicID]);
                    codebreakerTileEntity.HeldSchematicID = 0;
                    codebreakerTileEntity.DecryptionCountdown = 0;
                    codebreakerTileEntity.SyncContainedStuff();
                    SoundEngine.PlaySound(SoundID.Grab);

                    AwaitingDecryptionTextClose = false;
                }

                // Otherwise, if the player has an encrypted schematic and the Codebreaker doesn't, insert it into the machine.
                else if (CalamityLists.EncryptedSchematicIDRelationship.ContainsValue(playerHandItem.type) && codebreakerTileEntity.HeldSchematicID == 0)
                {
                    codebreakerTileEntity.HeldSchematicID = CalamityLists.EncryptedSchematicIDRelationship.First(i => i.Value == Main.mouseItem.type).Key;
                    playerHandItem.TurnToAir();
                    codebreakerTileEntity.SyncContainedStuff();
                    SoundEngine.PlaySound(SoundID.Grab);

                    AwaitingDecryptionTextClose = false;
                }

                // Lastly, if the player has an encrypted schematic but so does the Codebreaker, swap the two.
                else if (CalamityLists.EncryptedSchematicIDRelationship.ContainsValue(playerHandItem.type) && codebreakerTileEntity.HeldSchematicID != 0)
                {
                    int previouslyHeldSchematic = CalamityLists.EncryptedSchematicIDRelationship[codebreakerTileEntity.HeldSchematicID];

                    // If the schematics are the same, don't actually do anything, just play the sound as an illusion, to prevent having to send a packet.
                    SoundEngine.PlaySound(SoundID.Grab);
                    if (playerHandItem.type != previouslyHeldSchematic)
                    {
                        codebreakerTileEntity.HeldSchematicID = CalamityLists.EncryptedSchematicIDRelationship.First(i => i.Value == Main.mouseItem.type).Key;
                        playerHandItem.SetDefaults(previouslyHeldSchematic);
                        codebreakerTileEntity.SyncContainedStuff();
                        AwaitingDecryptionTextClose = false;
                    }
                }
            }
        }

        public static void DisplayCostText(Vector2 drawPosition, int totalCellsCost)
        {
            // Display the cost text.
            string text = CalamityUtils.GetTextValue("UI.Cost");
            drawPosition.X -= GeneralScale * 30f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, drawPosition.X, drawPosition.Y + GeneralScale * 20f, Color.White * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, GeneralScale);

            // And draw the cells to the right of the text.
            Texture2D cellTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/DraedonPowerCell").Value;
            Vector2 offsetDrawPosition = new Vector2(drawPosition.X + ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One, -1f).X * GeneralScale + GeneralScale * 15f, drawPosition.Y + GeneralScale * 30f);
            Main.spriteBatch.Draw(cellTexture, offsetDrawPosition, null, Color.White, 0f, cellTexture.Size() * 0.5f, GeneralScale, 0, 0f);

            // Display the cell quantity numerically below the drawn cells.
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.ItemStack.Value, totalCellsCost.ToString(), offsetDrawPosition.X - GeneralScale * 11f, offsetDrawPosition.Y, Color.White, Color.Black, new Vector2(0.3f), GeneralScale * 0.75f);
        }

        public static void DrawCostVerificationButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition)
        {
            Texture2D confirmationTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DecryptIcon").Value;
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, confirmationTexture.Size() * VerificationButtonScale);

            // Check if the mouse is hovering over the cost button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                VerificationButtonScale = MathHelper.Clamp(VerificationButtonScale + 0.035f, 1f, 1.35f);

                // If a click is done, begin the decryption process.
                // This will "lock" various things and make the Codebreaker unbreakable, to prevent complications with lost items.
                // Also play a cool sound.
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    SoundEngine.PlaySound(SoundID.Zombie67, Main.LocalPlayer.Center);
                    AwaitingDecryptionTextClose = true;
                    codebreakerTileEntity.InitialCellCountBeforeDecrypting = codebreakerTileEntity.InputtedCellCount;
                    codebreakerTileEntity.DecryptionCountdown = codebreakerTileEntity.DecryptionTotalTime;
                    codebreakerTileEntity.SyncContainedStuff();
                    codebreakerTileEntity.SyncDecryptCountdown();
                }
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
                VerificationButtonScale = MathHelper.Clamp(VerificationButtonScale - 0.05f, 1f, 1.35f);

            // Draw the confirmation icon.
            Main.spriteBatch.Draw(confirmationTexture, drawPosition, null, Color.White, 0f, confirmationTexture.Size() * 0.5f, VerificationButtonScale * GeneralScale, 0, 0f);
        }

        public static void DisplayDecryptCancelButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition)
        {
            bool clickingMouse = Main.mouseLeft && Main.mouseLeftRelease;
            Texture2D cancelTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DecryptCancelIcon").Value;
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, cancelTexture.Size() * CancelButtonScale * 1.2f);

            // Check if the mouse is hovering over the decrypt button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                CancelButtonScale = MathHelper.Clamp(CancelButtonScale + 0.035f, 0.9f, 1.2f);

                // If a click is done, cancel the decryption process.
                // This will cause already consumed cells to be lost.
                if (clickingMouse)
                {
                    if (AwaitingCloseConfirmation)
                    {
                        SoundEngine.PlaySound(SoundID.Item94, Main.LocalPlayer.Center);

                        AwaitingDecryptionTextClose = false;
                        codebreakerTileEntity.InitialCellCountBeforeDecrypting = 0;
                        codebreakerTileEntity.DecryptionCountdown = 0;
                        codebreakerTileEntity.SyncContainedStuff();
                        codebreakerTileEntity.SyncDecryptCountdown();
                        AwaitingCloseConfirmation = false;
                    }
                    else
                        AwaitingCloseConfirmation = true;
                }
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
            {
                CancelButtonScale = MathHelper.Clamp(CancelButtonScale - 0.05f, 0.9f, 1.2f);
                if (clickingMouse)
                    AwaitingCloseConfirmation = false;
            }

            // Draw the cancel icon.
            Main.spriteBatch.Draw(cancelTexture, drawPosition, null, Color.White, 0f, cancelTexture.Size() * 0.5f, CancelButtonScale * GeneralScale, 0, 0f);
        }

        public static void DrawDecryptCancelConfirmationText(Vector2 drawPosition)
        {
            Texture2D textPanelTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonDecrypterScreen").Value;
            drawPosition.X += GeneralScale * 196f;

            Vector2 scale = new Vector2(1f, 0.3f) * GeneralScale;
            Main.spriteBatch.Draw(textPanelTexture, drawPosition, null, Color.White, 0f, textPanelTexture.Size() * 0.5f, scale, 0, 0);

            string confirmationText = CalamityUtils.GetTextValue("UI.ConfirmationText");
            Vector2 confirmationTextPosition = drawPosition - FontAssets.MouseText.Value.MeasureString(confirmationText) * GeneralScale * 0.5f + Vector2.UnitY * GeneralScale * 4f;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, confirmationText, confirmationTextPosition, Color.Red, 0f, Vector2.Zero, Vector2.One * GeneralScale);
        }

        public static void HandleDecryptionStuff(TECodebreaker codebreakerTileEntity, Texture2D backgroundTexture, Vector2 backgroundTopLeft, Vector2 barCenter)
        {
            Texture2D textPanelTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/DraedonDecrypterScreen").Value;
            Vector2 textPanelCenter = backgroundTopLeft + Vector2.UnitX * backgroundTexture.Width * GeneralScale + textPanelTexture.Size() * new Vector2(-0.5f, 0.5f) * GeneralScale;
            Main.spriteBatch.Draw(textPanelTexture, textPanelCenter, null, Color.White, 0f, textPanelTexture.Size() * 0.5f, GeneralScale, 0, 0f);

            // Generate gibberish and use slowly insert the real text.
            // When decryption is done the gibberish will go away and only the underlying text will remain.
            int textPadding = 6;
            string trueMessage = codebreakerTileEntity.UnderlyingSchematicText;
            StringBuilder text = new StringBuilder(codebreakerTileEntity.DecryptionCountdown == 0 ? trueMessage : CalamityUtils.GenerateRandomAlphanumericString(500));

            // Messing with whitespace characters so can cause the word-wrap to "jump" around.
            // As a result, changes to whitespace characters in the true text do not stay.
            for (int i = 0; i < trueMessage.Length; i++)
            {
                if (char.IsWhiteSpace(trueMessage[i]))
                    text[i] = trueMessage[i];
            }

            // Insert the necessary amount of true text.
            for (int i = 0; i < (int)(trueMessage.Length * codebreakerTileEntity.DecryptionCompletion); i++)
                text[i] = trueMessage[i];

            // Define the initial text draw position.
            Vector2 currentTextDrawPosition = backgroundTopLeft + new Vector2(backgroundTexture.Width - textPanelTexture.Width + textPadding, 6f) * GeneralScale;

            // Draw the lines of text. A maximum of 10 may be drawn and the vertical offset per line is 16 pixels.
            foreach (string line in Utils.WordwrapString(text.ToString(), FontAssets.MouseText.Value, (int)(textPanelTexture.Width * 1.5 - textPadding * 2), 10, out _))
            {
                // If a line is null or empty for some reason, don't attempt to draw it or move to the next line position.
                if (string.IsNullOrEmpty(line))
                    continue;

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, line, currentTextDrawPosition, Color.Cyan, 0f, Vector2.Zero, new Vector2(0.6f) * GeneralScale);
                currentTextDrawPosition.Y += GeneralScale * 16f;
            }

            // Handle special drawing when decryption is ongoing.
            // If it isn't, return; the below logic is unnecessary.
            if (codebreakerTileEntity.DecryptionCountdown <= 0)
                return;

            // Draw a small bar at the bottom to indicate how much work is left.
            Texture2D borderTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/CodebreakerDecyptionBar").Value;
            Texture2D barTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/CodebreakerDecyptionBarCharge").Value;
            Main.spriteBatch.Draw(borderTexture, barCenter, null, Color.White, 0f, borderTexture.Size() * 0.5f, GeneralScale, 0, 0);
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * codebreakerTileEntity.DecryptionCompletion), barTexture.Width);
            Main.spriteBatch.Draw(barTexture, barCenter, barRectangle, Color.White, 0f, barTexture.Size() * 0.5f, GeneralScale, 0, 0);

            // Display a completion percentage below the bar as a more precise indicator.
            string completionText = $"{codebreakerTileEntity.DecryptionCompletion * 100f:n2}%";
            Vector2 textDrawPosition = barCenter + new Vector2(-FontAssets.MouseText.Value.MeasureString(completionText).X * 0.5f, 10f) * GeneralScale;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, completionText, textDrawPosition.X, textDrawPosition.Y, Color.Cyan * 1.2f, Color.Black, Vector2.Zero, GeneralScale);
        }

        public static void HandleDraedonSummonButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition)
        {
            Texture2D contactButton = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/ContactIcon").Value;

            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, contactButton.Size() * VerificationButtonScale);

            float iconrotation = codebreakerTileEntity.ContainsBloodSample ? Main.GlobalTimeWrappedHourly * 20f : 0f;

            // Check if the mouse is hovering over the contact button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                ContactButtonScale = MathHelper.Clamp(ContactButtonScale + 0.035f, 1f, 1.35f);

                // If a click is done, do a check.
                // Prepare the summoning process by defining the countdown and current summon position. The mech will be summoned by the Draedon NPC.
                // Also play a cool sound.
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    CalamityWorld.DraedonSummonCountdown = CalamityWorld.DraedonSummonCountdownMax;
                    CalamityWorld.DraedonSummonPosition = codebreakerTileEntity.Center + new Vector2(-8f, -100f);
                    if (Main.zenithWorld && codebreakerTileEntity.ContainsBloodSample)
                    {
                        CalamityWorld.DraedonMechdusa = true;
                    }
                    SoundEngine.PlaySound(SummonSound, CalamityWorld.DraedonSummonPosition);

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.CodebreakerSummonStuff);
                        netMessage.Write(CalamityWorld.DraedonSummonCountdown);
                        netMessage.WriteVector2(CalamityWorld.DraedonSummonPosition);
                        netMessage.Write(CalamityWorld.DraedonMechdusa);
                        netMessage.Send();
                    }
                }
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
                ContactButtonScale = MathHelper.Clamp(ContactButtonScale - 0.05f, 1f, 1.35f);

            // Draw the contact button.
            Main.spriteBatch.Draw(contactButton, drawPosition, null, Color.White, iconrotation, contactButton.Size() * 0.5f, ContactButtonScale * GeneralScale, 0, 0f);

            // And display a text indicator that describes the function of the button.
            // The color of the text cycles through the exo mech crystal palette.
            string contactTextKey = "Contact";
            if (DownedBossSystem.downedExoMechs)
                contactTextKey = "Summon";
            if (codebreakerTileEntity.ContainsBloodSample)
                contactTextKey = "Evoke";
            string contactText = CalamityUtils.GetTextValue("UI." + contactTextKey);

            Color contactTextColor = CalamityUtils.MulticolorLerp((float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.7f) * 0.5f + 0.5f, CalamityUtils.ExoPalette);

            // Center the draw position.
            drawPosition.X -= FontAssets.MouseText.Value.MeasureString(contactText).X * GeneralScale * 0.5f;
            drawPosition.Y += GeneralScale * 20f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, contactText, drawPosition.X, drawPosition.Y, contactTextColor, Color.Black, Vector2.Zero, GeneralScale);
        }

        public static void HandleDraedonTalkButton(Vector2 drawPosition)
        {
            Texture2D communicateButton = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/CommunicateIcon").Value;

            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, communicateButton.Size() * VerificationButtonScale);

            // Check if the mouse is hovering over the communicate button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                CommunicateButtonScale = MathHelper.Clamp(CommunicateButtonScale + 0.035f, 1f, 1.35f);

                // If a click is done, do a check. This triggers the communicate panel opening animation.
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    DisplayingCommunicationText = true;
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
                CommunicateButtonScale = MathHelper.Clamp(CommunicateButtonScale - 0.05f, 1f, 1.35f);

            // Draw the communication button.
            Main.spriteBatch.Draw(communicateButton, drawPosition, null, Color.White, 0f, communicateButton.Size() * 0.5f, CommunicateButtonScale * GeneralScale, 0, 0f);

            // And display a text indicator that describes the function of the button.
            // The color of the text is the same as Draedon's talk color.
            string communicateText = CalamityUtils.GetTextValue("UI.Communicate");

            // Center the draw position.
            drawPosition.X -= FontAssets.MouseText.Value.MeasureString(communicateText).X * GeneralScale * 0.5f;
            drawPosition.Y += GeneralScale * 20f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, communicateText, drawPosition.X, drawPosition.Y, Draedon.TextColor, Color.Black, Vector2.Zero, GeneralScale);
        }

        public static void DisplayNotStrongEnoughErrorText(Vector2 drawPosition)
        {
            string text = CalamityUtils.GetTextValue("UI.UpgradesRequired");
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, drawPosition.X, drawPosition.Y, Color.IndianRed * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, GeneralScale);
        }
    }
}
