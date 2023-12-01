using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        public static void RightClickBreak(int i, int j)
        {
            if (Main.tile[i, j] != null && Main.tile[i, j].HasTile)
            {
                WorldGen.KillTile(i, j, false, false, false);
                if (!Main.tile[i, j].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                }
            }
        }

        public static bool BedRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int spawnX = i - tile.TileFrameX / 18 + (tile.TileFrameX >= 72 ? 5 : 2);
            int spawnY = j + 2;
            if (tile.TileFrameY % 38 != 0)
            {
                spawnY--;
            }

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
            {
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                {
                    player.GamepadEnableGrappleCooldown();
                    player.sleeping.StartSleeping(player, i, j);
                }
            }
            else
            {
                player.FindSpawn();

                if (player.SpawnX == spawnX && player.SpawnY == spawnY)
                {
                    player.RemoveSpawn();
                    Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
                }
                else if (Player.CheckSpawn(spawnX, spawnY))
                {
                    player.ChangeSpawn(spawnX, spawnY);
                    Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
                }
            }

            return true;
        }

        #region Sitting in Chairs
        // fat is for 2 tile chairs like Exo Chair and Exo Toilet
        public static void ChairSitInfo(int i, int j, ref TileRestingInfo info, int nextStyleHeight = 40, bool fat = false, bool hasOffset = false, bool shitter = false)
        {
            if (hasOffset)
            {
                info.DirectionOffset = 0;
                info.VisualOffset = new Vector2(-8f, 0f);
            }

            Tile tile = Framing.GetTileSafely(i, j);
            bool frameCheck = fat ? tile.TileFrameX >= 35 : tile.TileFrameX != 0;

            if (shitter)
                info.ExtraInfo.IsAToilet = true;

            info.TargetDirection = -1;
            if (frameCheck)
            {
                info.TargetDirection = 1;
            }

            if (fat)
            {
                int xPos = tile.TileFrameX / 18;
                if (xPos == 1)
                    i--;
                if (xPos == 2)
                    i++;
            }

            info.AnchorTilePosition.X = i;
            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % nextStyleHeight == 0)
            {
                info.AnchorTilePosition.Y++;
            }
        }

        public static bool ChairRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }
            return true;
        }

        public static void ChairMouseOver(int i, int j, int itemID, bool fat = false)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = itemID;

            bool frameCheck = fat ? Main.tile[i, j].TileFrameX <= 35 : Main.tile[i, j].TileFrameX / 18 < 0;
            if (frameCheck)
            {
                player.cursorItemIconReversed = true;
            }
        }

        public static void MouseOver(int i, int j, int itemID)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = itemID;
        }
        #endregion

        #region Sitting in Sofas/Benches
        public static void BenchSitInfo(int i, int j, ref TileRestingInfo info, int nextStyleHeight = 40)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Player player = Main.LocalPlayer;

            info.DirectionOffset = 0;
            float offset = 0f;
            if (tile.TileFrameX < 17 && player.direction == 1)
                offset = 8f;
            if (tile.TileFrameX < 17 && player.direction == -1)
                offset = -8f;
            if (tile.TileFrameX > 34 && player.direction == 1)
                offset = -8f;
            if (tile.TileFrameX > 34 && player.direction == -1)
                offset = 8f;
            info.VisualOffset = new Vector2(offset, 0f);
            info.TargetDirection = player.direction;

            info.AnchorTilePosition.X = i;
            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % nextStyleHeight == 0)
            {
                info.AnchorTilePosition.Y++;
            }
        }

        public static void BenchMouseOver(int i, int j, int itemID)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = itemID;
        }
        #endregion

        public static bool ChestRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }
            if (tile.TileFrameY != 0)
            {
                top--;
            }
            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    Main.stackSplit = 600;
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        player.chest = chest;
                        Main.playerInventory = true;
                        Main.recBigList = false;
                        player.chestX = left;
                        player.chestY = top;
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                    }
                    Recipe.FindRecipes();
                }
            }
            return true;
        }

        public static void ChestMouseOver<T>(int i, int j) where T : ModItem
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            string chestName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }
            if (tile.TileFrameY != 0)
            {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chest < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
                if (player.cursorItemIconText == chestName)
                {
                    player.cursorItemIconID = ItemType<T>();
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public static void ChestMouseFar<T>(int i, int j) where T : ModItem
        {
            ChestMouseOver<T>(i, j);
            Player player = Main.LocalPlayer;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        public static bool ClockRightClick()
        {
            string text = "AM";

            // Get Terraria's current strange time variable
            double time = Main.time;

            // Correct for night time (which for some reason isn't just a different number) by adding 54000.
            if (!Main.dayTime)
                time += 54000D;

            // Divide by seconds in an hour
            time /= 3600D;

            // Terraria night starts at 7:30 PM, so offset accordingly
            time -= 19.5;

            // Offset time to ensure it is not negative, then change to PM if necessary
            if (time < 0D)
                time += 24D;
            if (time >= 12D)
                text = "PM";

            // Get the decimal (smaller than hours, so minutes) component of time.
            int intTime = (int)time;
            double deltaTime = time - intTime;

            // Convert decimal time into an exact number of minutes.
            deltaTime = (int)(deltaTime * 60D);

            string minuteText = deltaTime.ToString();

            // Ensure minutes has a leading zero
            if (deltaTime < 10D)
                minuteText = "0" + minuteText;

            // Convert from 24 to 12 hour time (PM already handled earlier)
            if (intTime > 12)
                intTime -= 12;
            // 12 AM is actually hour zero in 24 hour time
            if (intTime == 0)
                intTime = 12;

            // Create an overall time readout and send it to chat
            var newText = string.Concat("Time: ", intTime, ":", minuteText, " ", text);
            Main.NewText(newText, 255, 240, 20);
            return true;
        }

        public static bool DresserRightClick()
        {
            Player player = Main.LocalPlayer;
            if (Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY == 0)
            {
                Main.CancelClothesWindow(true);

                int left = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameX / 18);
                left %= 3;
                left = Player.tileTargetX - left;
                int top = Player.tileTargetY - (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY / 18);
                if (player.sign > -1)
                {
                    SoundEngine.PlaySound(SoundID.MenuClose);
                    player.sign = -1;
                    Main.editSign = false;
                    Main.npcChatText = string.Empty;
                }
                if (Main.editChest)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.editChest = false;
                    Main.npcChatText = string.Empty;
                }
                if (player.editedChestName)
                {
                    NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                    player.editedChestName = false;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (left == player.chestX && top == player.chestY && player.chest != -1)
                    {
                        player.chest = -1;
                        Recipe.FindRecipes();
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
                        Main.stackSplit = 600;
                    }
                    return true;
                }
                else
                {
                    player.piggyBankProjTracker.Clear();
                    player.voidLensChest.Clear();
                    int dresserChestID = Chest.FindChest(left, top);
                    if (dresserChestID != -1)
                    {
                        Main.stackSplit = 600;
                        if (dresserChestID == player.chest)
                        {
                            player.chest = -1;
                            Recipe.FindRecipes();
                            SoundEngine.PlaySound(SoundID.MenuClose);
                        }
                        else if (dresserChestID != player.chest && player.chest == -1)
                        {
                            player.chest = dresserChestID;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            SoundEngine.PlaySound(SoundID.MenuOpen);
                            player.chestX = left;
                            player.chestY = top;
                        }
                        else
                        {
                            player.chest = dresserChestID;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            SoundEngine.PlaySound(SoundID.MenuTick);
                            player.chestX = left;
                            player.chestY = top;
                        }
                        Recipe.FindRecipes();
                        return true;
                    }
                }
            }
            else
            {
                Main.playerInventory = false;
                player.chest = -1;
                Recipe.FindRecipes();
                Main.interactedDresserTopLeftX = Player.tileTargetX;
                Main.interactedDresserTopLeftY = Player.tileTargetY;
                Main.OpenClothesWindow();
                return true;
            }

            return false;
        }

        public static void DresserMouseFar<T>() where T : ModItem
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            string chestName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            int left = Player.tileTargetX;
            int top = Player.tileTargetY;
            left -= (int)(tile.TileFrameX % 54 / 18);
            if (tile.TileFrameY % 36 != 0)
            {
                top--;
            }
            int chestIndex = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chestIndex < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                if (Main.chest[chestIndex].name != "")
                {
                    player.cursorItemIconText = Main.chest[chestIndex].name;
                }
                else
                {
                    player.cursorItemIconText = chestName;
                }
                if (player.cursorItemIconText == chestName)
                {
                    player.cursorItemIconID = ItemType<T>();
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        public static void DresserMouseOver<T>() where T : ModItem
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            string chestName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            int left = Player.tileTargetX;
            int top = Player.tileTargetY;
            left -= (int)(tile.TileFrameX % 54 / 18);
            if (tile.TileFrameY % 36 != 0)
            {
                top--;
            }
            int chestIndex = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chestIndex < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                if (Main.chest[chestIndex].name != "")
                {
                    player.cursorItemIconText = Main.chest[chestIndex].name;
                }
                else
                {
                    player.cursorItemIconText = chestName;
                }
                if (player.cursorItemIconText == chestName)
                {
                    player.cursorItemIconID = ItemType<T>();
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY > 0)
            {
                player.cursorItemIconID = ItemID.FamiliarShirt;
            }
        }

        public static bool LockedChestRightClick(bool isLocked, int left, int top, int i, int j)
        {
            Player player = Main.LocalPlayer;

            // If the player right clicked the chest while editing a sign, finish that up
            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }

            // If the player right clicked the chest while editing a chest, finish that up
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }

            // If the player right clicked the chest after changing another chest's name, finish that up
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
            {
                // Right clicking the chest you currently have open closes it. This counts as interaction.
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }

                // Right clicking this chest opens it if it's not already open. This counts as interaction.
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
                    Main.stackSplit = 600;
                }
                return true;
            }

            else
            {
                if (isLocked)
                {
                    // If you right click the locked chest and you can unlock it, it unlocks itself but does not open. This counts as interaction.
                    if (Chest.Unlock(left, top))
                    {
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, (float)left, (float)top);
                        }
                        return true;
                    }
                }
                else
                {
                    int chest = Chest.FindChest(left, top);
                    if (chest >= 0)
                    {
                        Main.stackSplit = 600;

                        // If you right click the same chest you already have open, it closes. This counts as interaction.
                        if (chest == player.chest)
                        {
                            player.chest = -1;
                            SoundEngine.PlaySound(SoundID.MenuClose);
                        }

                        // If you right click this chest when you have a different chest selected, that one closes and this one opens. This counts as interaction.
                        else
                        {
                            player.chest = chest;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            player.chestX = left;
                            player.chestY = top;
                            SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        }

                        Recipe.FindRecipes();
                        return true;
                    }
                }
            }

            // This only occurs when the chest is locked and cannot be unlocked. You did not interact with the chest.
            return false;
        }

        public static void LockedChestMouseOver<K, C>(int i, int j)
            where K : ModItem where C : ModItem
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            string chestName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }
            if (tile.TileFrameY != 0)
            {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chest < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
                if (player.cursorItemIconText == chestName)
                {
                    player.cursorItemIconID = ItemType<C>();
                    if (Main.tile[left, top].TileFrameX / 36 == 1)
                        player.cursorItemIconID = ItemType<K>();
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public static void LockedChestMouseOverFar<K, C>(int i, int j)
            where K : ModItem where C : ModItem
        {
            LockedChestMouseOver<K, C>(i, j);
            Player player = Main.LocalPlayer;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a bar.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="mapColor">The color of the bar on the minimap.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to true like vanilla bars.</param>
        internal static void SetUpBar(this ModTile mt, int itemDropID, Color mapColor, bool lavaImmune = true)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileShine[mt.Type] = 1100;
            Main.tileSolid[mt.Type] = true;
            Main.tileSolidTop[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            // Vanilla bars are labeled as "Metal Bar" on the minimap
            mt.AddMapEntry(mapColor, Language.GetText("MapObject.MetalBar"));
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a bathtub.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpBathtub(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(mt.Type);

            // All bathtubs count as tables.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            mt.AddMapEntry(new Color(144, 148, 144), Language.GetText("ItemName.Bathtub"));
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a bed.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpBed(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileID.Sets.CanBeSleptIn[mt.Type] = true;
            TileID.Sets.InteractibleByNPCs[mt.Type] = true;
            TileID.Sets.IsValidSpawnPoint[mt.Type] = true;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(mt.Type);

            // All beds count as chairs.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.Bed"));
            mt.AdjTiles = new int[] { TileID.Beds };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a bookcase.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        /// <param name="solidTop">Whether this tile is supposed to have a solid top. Defaults to true.</param>
        /// <param name="autoBookcase">Whether this tile is automatically registered as a bookcase and table with proper map entry. Defaults to true.</param>
        internal static void SetUpBookcase(this ModTile mt, int itemDropID, bool lavaImmune = false, bool solidTop = true, bool autoBookcase = true)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileSolidTop[mt.Type] = solidTop;
            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileTable[mt.Type] = solidTop;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            // Skip this for special bookcases such as Monolith Amalgam
            if (autoBookcase)
            {
                mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
                mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.Bookcase"));
                mt.AdjTiles = new int[] { TileID.Bookcases };
            }
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a candelabra.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpCandelabra(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);
            
            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(mt.Type);

            // All candelabras count as light sources.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            mt.AddMapEntry(new Color(253, 221, 3), Language.GetText("ItemName.Candelabra"));
            mt.AdjTiles = new int[] { TileID.Candelabras };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a candle.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        /// <param name="autoMapEntry">Whether this tile is supposed to use normal map entries. Defaults to true.</param>
        /// <param name="offset">The vertical offset of the tile. Defaults to -4.</param>
        internal static void SetUpCandle(this ModTile mt, int itemDropID, bool lavaImmune = false, bool autoMapEntry = true, int offset = -4)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.DrawYOffset = offset;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(mt.Type);

            // All candles count as light sources.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            if (autoMapEntry)
                mt.AddMapEntry(new Color(253, 221, 3), Language.GetText("ItemName.Candle"));
            
            mt.AdjTiles = new int[] { TileID.Candles };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a chair.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpChair(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileFrameImportant[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.CanBeSatOnForNPCs[mt.Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[mt.Type] = true;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(mt.Type);

            // As you could probably guess, all chairs count as chairs.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("MapObject.Chair"));
            mt.AdjTiles = new int[] { TileID.Chairs };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a chandelier.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpChandelier(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(mt.Type);

            // All chandeliers count as light sources.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            mt.AddMapEntry(new Color(235, 166, 135), Language.GetText("MapObject.Chandelier"));
            mt.AdjTiles = new int[] { TileID.Chandeliers };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a chest.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="offset">If true, uses the parameter offsetAmt to decide the Y draw offset.</param>
        /// <param name="offsetAmt">If offset is true, this is the Y draw offset to use. Otherwise it is ignored.</param>
        internal static void SetUpChest(this ModTile mt, int itemDropID, bool offset = false, int offsetAmt = 4)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileSpelunker[mt.Type] = true;
            Main.tileContainer[mt.Type] = true;
            Main.tileShine2[mt.Type] = true;
            Main.tileShine[mt.Type] = 1200;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileOreFinderPriority[mt.Type] = 500;
            TileID.Sets.BasicChest[mt.Type] = true;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            if (offset)
                TileObjectData.newTile.DrawYOffset = offsetAmt;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(mt.Type);

            mt.AdjTiles = new int[] { TileID.Containers };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a clock.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpClock(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileFrameImportant[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16,
                16,
                16
            };
            TileObjectData.newTile.Origin = new Point16(0, 4);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.GrandfatherClock"));
            mt.AdjTiles = new int[] { TileID.GrandfatherClocks };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a closed door.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpDoorClosed(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileFrameImportant[mt.Type] = true;
            Main.tileBlockLight[mt.Type] = true;
            Main.tileSolid[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.NotReallySolid[mt.Type] = true;
            TileID.Sets.DrawsWalls[mt.Type] = true;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(mt.Type);

            // As you could probably guess, all closed doors count as doors.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            mt.AddMapEntry(new Color(119, 105, 79), Language.GetText("MapObject.Door"));
            mt.AdjTiles = new int[] { TileID.ClosedDoor };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be an open door.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpDoorOpen(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileFrameImportant[mt.Type] = true;
            Main.tileSolid[mt.Type] = false;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            Main.tileNoSunLight[mt.Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 0);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 1);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 2);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(mt.Type);
            TileID.Sets.HousingWalls[mt.Type] = true;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;

            // As you could probably guess, all open doors count as doors.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            mt.AddMapEntry(new Color(119, 105, 79), Language.GetText("MapObject.Door"));
            mt.AdjTiles = new int[] { TileID.OpenDoor };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a dresser.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        internal static void SetUpDresser(this ModTile mt, int itemDropID)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileSolidTop[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileTable[mt.Type] = true;
            Main.tileContainer[mt.Type] = true;
            Main.tileWaterDeath[mt.Type] = false;
            Main.tileLavaDeath[mt.Type] = false;
            TileID.Sets.BasicDresser[mt.Type] = true;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(mt.Type);

            // All dressers count as tables.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            mt.AdjTiles = new int[] { TileID.Dressers };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a fountain.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="mapColor">The map color of the tile.</param>
        internal static void SetUpFountain(this ModTile mt, int itemDropID, Color mapColor)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = false;
            Main.tileWaterDeath[mt.Type] = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addTile(mt.Type);
            TileID.Sets.HasOutlines[mt.Type] = true;

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 3);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(mt.Type);

            mt.AddMapEntry(mapColor, Language.GetText("MapObject.WaterFountain"));
            mt.AnimationFrameHeight = 72;
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a floor lamp.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpLamp(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(mt.Type);

            // All floor lamps count as light sources.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            mt.AddMapEntry(new Color(253, 221, 3), Language.GetText("MapObject.FloorLamp"));
            mt.AdjTiles = new int[] { TileID.Lamps };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a hanging lantern.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        /// <param name="autoMapEntry">Whether this tile is supposed to use normal map entries. Defaults to true.</param>
        internal static void SetUpLantern(this ModTile mt, int itemDropID, bool lavaImmune = false, bool autoMapEntry = true)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
    		TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
	    	TileObjectData.newAlternate.DrawYOffset = -10;
		    TileObjectData.addAlternate(0);
            TileObjectData.addTile(mt.Type);

            // All hanging lanterns count as light sources.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            if (autoMapEntry)
                mt.AddMapEntry(new Color(251, 235, 127), Language.GetText("MapObject.Lantern"));

            mt.AdjTiles = new int[] { TileID.HangingLanterns };
        }

        // Allow hanging lanterns to move up when hung on platforms
        internal static void PlatformHangOffset(int i, int j, ref int offsetY)
        {
            Tile tile = Main.tile[i, j];
            TileObjectData data = TileObjectData.GetTileData(tile);
            int topLeftX = i - tile.TileFrameX / 18 % data.Width;
            int topLeftY = j - tile.TileFrameY / 18 % data.Height;
            if (WorldGen.IsBelowANonHammeredPlatform(topLeftX, topLeftY))
            {
                offsetY -= 8;
            }
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a piano.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        /// <param name="solidTop">Whether this tile is supposed to have a solid top. Defaults to true.</param>
        internal static void SetUpPiano(this ModTile mt, int itemDropID, bool lavaImmune = false, bool solidTop = true)
        {
            mt.RegisterItemDrop(itemDropID);
            
            Main.tileTable[mt.Type] = solidTop;
            Main.tileSolidTop[mt.Type] = solidTop;
            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            // All pianos count as tables.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.Piano"));
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a platform.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpPlatform(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);
            
            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileSolidTop[mt.Type] = true;
            Main.tileSolid[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileTable[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            TileID.Sets.Platforms[mt.Type] = true;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 27;
            TileObjectData.newTile.StyleWrapLimit = 27;
            TileObjectData.newTile.UsesCustomCanPlace = false;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            // All platforms count as doors (so that you may have top-or-bottom entry/exit rooms)
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            mt.AddMapEntry(new Color(191, 142, 111));
            mt.AdjTiles = new int[] { TileID.Platforms };
        }

        /// <summary>
        /// Extension which initializes a ModPylon to be a simple pylon.<br />
        /// ModPylon appears to make it easier to create a modded pylon extending ModTile.<br />
        /// <br />
        /// Note: This is used for pylons that follow general vanilla functionality.<br />
        /// Pylons that have vastly different custom behavior require a different setup (refer to ExamplePylonTileAdvanced in ExampleMod).<br />
        /// </summary>
        /// <param name="mt">The ModPylon which is being initialized.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpPylon(this ModPylon mp, TEModdedPylon pylonHook, bool lavaImmune = false, int offset = 2)
        {
            Main.tileLighted[mp.Type] = true;
            Main.tileFrameImportant[mp.Type] = true;
            Main.tileLavaDeath[mp.Type] = !lavaImmune;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);

            // These definitions allow for vanilla's pylon TileEntities to be placed.
            // tModLoader has a built in Tile Entity specifically for modded pylons, which is extended through TECalamityPylon.
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(pylonHook.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(pylonHook.Hook_AfterPlacement, -1, 0, false);

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.DrawYOffset = offset;
            TileObjectData.addTile(mp.Type);

            // Adds functionality for proximity of pylons; if this is true, then being near this tile will count as being near a pylon for the teleportation process.
            mp.AddToArray(ref TileID.Sets.CountsAsPylon);
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a sink.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        /// <param name="water">Whether this tile counts as a water source. Defaults to true.</param>
        /// <param name="lava">Whether this tile counts as a lava source. Defaults to false.</param>
        /// <param name="honey">Whether this tile counts as a honey source. Defaults to false.</param>
        internal static void SetUpSink(this ModTile mt, int itemDropID, bool lavaImmune = false, bool water = true, bool lava = false, bool honey = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.CountsAsWaterSource[mt.Type] = water;
            TileID.Sets.CountsAsHoneySource[mt.Type] = lava;
            TileID.Sets.CountsAsLavaSource[mt.Type] = honey;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("MapObject.Sink"));
            if (water)
                mt.AdjTiles = new int[] { TileID.Sinks };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a sofa.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        /// <param name="bench">Whether this tile is displayed as bench instead of sofa on the map. Defaults to false.</param>
        internal static void SetUpSofa(this ModTile mt, int itemDropID, bool lavaImmune = false, bool bench = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.CanBeSatOnForPlayers[mt.Type] = true;
            TileID.Sets.HasOutlines[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            // All sofas count as chairs.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            mt.AddMapEntry(new Color(191, 142, 111), bench ? Language.GetText("ItemName.Bench") : Language.GetText("ItemName.Sofa"));
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a table.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpTable(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileSolidTop[mt.Type] = true;
            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileTable[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            // As you could probably guess, all tables count as tables.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("MapObject.Table"));
            mt.AdjTiles = new int[] { TileID.Tables };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a torch.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="waterImmune">Whether this tile is supposed to be immune to water. Defaults to false.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpTorch(this ModTile mt, int itemDropID, bool waterImmune = false, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileLighted[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileSolid[mt.Type] = false;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileNoFail[mt.Type] = true;
            Main.tileWaterDeath[mt.Type] = !waterImmune;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileID.Sets.Torch[mt.Type] = true;
            TileID.Sets.FramesOnKillWall[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newTile.WaterDeath = !waterImmune;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.WaterPlacement = waterImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.WaterDeath = !waterImmune;
            TileObjectData.newAlternate.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.WaterPlacement = waterImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.WaterDeath = !waterImmune;
            TileObjectData.newAlternate.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.WaterPlacement = waterImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(2);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.WaterDeath = !waterImmune;
            TileObjectData.newAlternate.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.WaterPlacement = waterImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(mt.Type);

            // All torches count as light sources.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            mt.AddMapEntry(new Color(253, 221, 3), Language.GetText("ItemName.Torch"));
            mt.AdjTiles = new int[] { TileID.Torches };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a trophy.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        internal static void SetUpTrophy(this ModTile mt)
        {
            // TODO -- how to force trophy drops correctly? they all have zero code in them
            
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.addTile(mt.Type);
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileID.Sets.FramesOnKillWall[mt.Type] = true;

            mt.AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
            mt.DustType = 7;
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a work bench.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="itemDropID">The ID of the item this tile drops when broken.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUpWorkBench(this ModTile mt, int itemDropID, bool lavaImmune = false)
        {
            mt.RegisterItemDrop(itemDropID);

            Main.tileSolidTop[mt.Type] = true;
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileNoAttach[mt.Type] = true;
            Main.tileTable[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileID.Sets.DisableSmartCursor[mt.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);

            // All work benches count as tables.
            mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            mt.AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.WorkBench"));
            mt.AdjTiles = new int[] { TileID.WorkBenches };
        }

        /// <summary>
        /// Extension which initializes a ModTile to be a 6x6 Painting.
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
        internal static void SetUp6x6Painting(this ModTile mt, bool lavaImmune = false)
        {
            Main.tileFrameImportant[mt.Type] = true;
            Main.tileLavaDeath[mt.Type] = !lavaImmune;
            Main.tileWaterDeath[mt.Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.Origin = new Point16(2, 2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.LavaPlacement = lavaImmune ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.addTile(mt.Type);
        }
    }
}
