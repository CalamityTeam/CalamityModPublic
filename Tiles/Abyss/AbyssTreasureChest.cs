using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class AbyssTreasureChest : ModTile
    {
        public override void SetStaticDefaults() 
        {
            this.SetUpChest(ModContent.ItemType<Items.Placeables.Furniture.AbyssTreasureChest>());
            AddMapEntry(new Color(71, 49, 41), CalamityUtils.GetItemName<Items.Placeables.Furniture.AbyssTreasureChest>(), CalamityUtils.GetMapChestName);
            DustType = 33;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<Items.Placeables.Furniture.AbyssTreasureChest>();
        public override void MouseOver(int i, int j) => CalamityUtils.ChestMouseOver<Items.Placeables.Furniture.AbyssTreasureChest>(i, j);
        public override void MouseOverFar(int i, int j) => CalamityUtils.ChestMouseFar<Items.Placeables.Furniture.AbyssTreasureChest>(i, j);
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);

        // Locked Chest stuff
        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;
        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            //Skeletron must be dead, NPC.downedBoss3 is added as a safety measure for worlds generated before this was placed or in case the first bool gets cleared
            // since tmod has not fixed the modded data disappearing bug
            if (World.Abyss.AbleToUnlockChests || NPC.downedBoss3) 
                return true; 
            else
                return false;
        }

        public override bool RightClick(int i, int j) 
        {
            Tile tile = Main.tile[i, j];

            int left = i;
            int top = j;

            if (tile.TileFrameX % 36 != 0) 
                left--;
            if (tile.TileFrameY != 0) 
                top--;

            return CalamityUtils.LockedChestRightClick(IsLockedChest(left, top), left, top, i, j);
        }
    }
}
