using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralChestLocked : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest();

            ModTranslation name = CreateMapEntryName("chestAstral");
            name.SetDefault("Astral Chest");
            AddMapEntry(new Color(174, 129, 92), name, MapChestName);

            name = CreateMapEntryName("chestAstral_Locked");
            name.SetDefault("Locked Astral Chest");
            AddMapEntry(new Color(174, 129, 92), name, MapChestName);

            DustType = ModContent.DustType<AstralBasic>();
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Containers };
            chest = "Astral Chest";
            ChestDrop = ModContent.ItemType<AstralChest>();
        }

        public override bool HasSmartInteract() => true;

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            if (!CalamityWorld.downedAstrageldon)
                return false;

            DustType = this.dustType;

            return true;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public string MapChestName(string name, int i, int j) => CalamityUtils.GetMapChestName(name, i, j);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ChestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];

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
            return CalamityUtils.LockedChestRightClick(IsLockedChest(left, top), left, top, i, j);
        }

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.ChestMouseOver<AstralChest>("Astral Chest", i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.ChestMouseFar<AstralChest>("Astral Chest", i, j);
        }
    }
}
