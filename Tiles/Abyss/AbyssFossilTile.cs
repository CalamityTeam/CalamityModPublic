using CalamityMod.Items.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    public class AbyssFossilTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(29, 37, 58), CalamityUtils.GetItemName<AbyssShellFossil>());
            DustType = 33;
            RegisterItemDrop(ModContent.ItemType<AbyssShellFossil>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<AbyssShellFossil>(), Type, 0);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
