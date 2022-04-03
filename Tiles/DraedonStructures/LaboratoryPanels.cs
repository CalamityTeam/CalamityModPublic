using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryPanels : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            SoundType = SoundID.Tink;
            DustType = 109;
            MinPick = 30;
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryPanels>();
            AddMapEntry(new Color(36, 35, 37));
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
