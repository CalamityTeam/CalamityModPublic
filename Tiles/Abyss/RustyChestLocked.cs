using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class RustyChestLocked : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpChest();
            ModTranslation name = CreateMapEntryName("chestRusty");
            name.SetDefault("Rusty Chest");
            AddMapEntry(new Color(57, 48, 83), name, MapChestName);
            dustType = ModContent.DustType<AstralBasic>();
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Containers };
            chest = "Rusty Chest";
            chestDrop = ModContent.ItemType<RustyChest>();
        }

        public override bool HasSmartInteract() => true;

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].frameX / 36 == 1;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            Player closest = Main.player[Player.FindClosest(new Vector2(i, j) * 16f, 1, 1)];
            if (!closest.InventoryHas(ModContent.ItemType<RustyLockpick>()))
                return false;

            for (int k = 0; k < closest.inventory.Length; k++)
            {
                if (closest.inventory[k].type == ModContent.ItemType<RustyLockpick>())
                {
                    if (ItemLoader.ConsumeItem(closest.inventory[k], closest))
                    {
                        closest.inventory[k].stack--;
                    }
                    if (closest.inventory[k].stack <= 0)
                    {
                        closest.inventory[k] = new Item();
                    }
                    break;
                }
            }

            dustType = this.dustType;

            return true;
        }

        public string MapChestName(string name, int i, int j)
        {
            // Bounds check
            if (i < 0 || i >= Main.maxTilesX || j < 0 || j >= Main.maxTilesY)
                return name;

            // Tile null check
            Tile tile = Main.tile[i, j];
            if (tile is null)
                return name;

            int left = i;
            int top = j;
            if (tile.frameX % 36 != 0)
                left--;
            if (tile.frameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            return name + (Main.chest[chest].name != "" ? ": " + Main.chest[chest].name : "");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, chestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool NewRightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int left = i;
            int top = j;

            if (tile.frameX % 36 != 0)
            {
                left--;
            }
            if (tile.frameY != 0)
            {
                top--;
            }
            return CalamityUtils.LockedChestRightClick(IsLockedChest(left, top), left, top, i, j);
        }

        public override void MouseOver(int i, int j)
        {
            if (IsLockedChest(i, j))
                CalamityUtils.ChestMouseOver<RustyLockpick>("Rusty Chest", i, j);
            else
                CalamityUtils.ChestMouseOver<RustyChest>("Rusty Chest", i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            if (IsLockedChest(i, j))
                CalamityUtils.ChestMouseFar<RustyLockpick>("Rusty Chest", i, j);
            else
                CalamityUtils.ChestMouseFar<RustyChest>("Rusty Chest", i, j);
        }
    }
}
