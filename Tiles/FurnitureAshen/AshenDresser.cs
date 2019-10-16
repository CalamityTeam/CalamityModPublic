using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class AshenDresser : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ashen Dresser");
            AddMapEntry(new Color(191, 142, 111), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Dressers };
            dresser = "Ashen Dresser";
            dresserDrop = ModContent.ItemType<AshenDresser>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool NewRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY == 0)
            {
                Main.CancelClothesWindow(true);

                // with 0.11.5 changes this should no longer be necessary
                // Main.mouseRightRelease = false;

                int left = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18);
                left %= 3;
                left = Player.tileTargetX - left;
                int top = Player.tileTargetY - (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18);
                if (player.sign > -1)
                {
                    Main.PlaySound(SoundID.MenuClose);
                    player.sign = -1;
                    Main.editSign = false;
                    Main.npcChatText = string.Empty;
                }
                if (Main.editChest)
                {
                    Main.PlaySound(SoundID.MenuTick);
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
                        Main.PlaySound(SoundID.MenuClose);
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
                    player.flyingPigChest = -1;
                    int num213 = Chest.FindChest(left, top);
                    if (num213 != -1)
                    {
                        Main.stackSplit = 600;
                        if (num213 == player.chest)
                        {
                            player.chest = -1;
                            Recipe.FindRecipes();
                            Main.PlaySound(SoundID.MenuClose);
                        }
                        else if (num213 != player.chest && player.chest == -1)
                        {
                            player.chest = num213;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            Main.PlaySound(SoundID.MenuOpen);
                            player.chestX = left;
                            player.chestY = top;
                        }
                        else
                        {
                            player.chest = num213;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            Main.PlaySound(SoundID.MenuTick);
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
                Main.dresserX = Player.tileTargetX;
                Main.dresserY = Player.tileTargetY;
                Main.OpenClothesWindow();
                return true;
            }

            return false;
        }

        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            int left = Player.tileTargetX;
            int top = Player.tileTargetY;
            left -= (int)(tile.frameX % 54 / 18);
            if (tile.frameY % 36 != 0)
            {
                top--;
            }
            int chestIndex = Chest.FindChest(left, top);
            player.showItemIcon2 = -1;
            if (chestIndex < 0)
            {
                player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                if (Main.chest[chestIndex].name != "")
                {
                    player.showItemIconText = Main.chest[chestIndex].name;
                }
                else
                {
                    player.showItemIconText = chest;
                }
                if (player.showItemIconText == chest)
                {
                    player.showItemIcon2 = ModContent.ItemType<AshenDresser>();
                    player.showItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.showItemIcon = true;
            if (player.showItemIconText == "")
            {
                player.showItemIcon = false;
                player.showItemIcon2 = 0;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            int left = Player.tileTargetX;
            int top = Player.tileTargetY;
            left -= (int)(tile.frameX % 54 / 18);
            if (tile.frameY % 36 != 0)
            {
                top--;
            }
            int num138 = Chest.FindChest(left, top);
            player.showItemIcon2 = -1;
            if (num138 < 0)
            {
                player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                if (Main.chest[num138].name != "")
                {
                    player.showItemIconText = Main.chest[num138].name;
                }
                else
                {
                    player.showItemIconText = chest;
                }
                if (player.showItemIconText == chest)
                {
                    player.showItemIcon2 = ModContent.ItemType<AshenDresser>();
                    player.showItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.showItemIcon = true;
            if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY > 0)
            {
                player.showItemIcon2 = ItemID.FamiliarShirt;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 32, dresserDrop);
            Chest.DestroyChest(i, j);
        }
    }
}
