using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using Terraria.ID;
using CalamityMod.Items;
namespace CalamityMod.Tiles.Astral
{
    public class AstralChestLocked : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpChest(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Astral Chest");
            AddMapEntry(new Color(174, 129, 92), name, MapChestName);
            name = CreateMapEntryName(Name + "_Locked"); // With multiple map entries, you need unique translation keys.
            name.SetDefault("Locked Astral Chest");
            AddMapEntry(new Color(174, 129, 92), name, MapChestName);
            dustType = ModContent.DustType<AstralBasic>();
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Containers };
            chest = "Astral Chest";
            chestDrop = ModContent.ItemType<AstralChest>();
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].frameX / 36);

        public override bool HasSmartInteract() => true;

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].frameX / 36 == 1;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            if (!CalamityWorld.downedAstrageldon)
                return false;

            dustType = this.dustType;

            return true;
        }

        public string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];

            if (tile.frameX % 36 != 0)
                left--;
            if (tile.frameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            if (Main.chest[chest].name == "")
                return name;
            else
                return name + ": " + Main.chest[chest].name;
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
            CalamityUtils.ChestMouseOver("AstralChest", "Astral Chest", i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.ChestMouseFar("AstralChest", "Astral Chest", i, j);
        }
    }
}
