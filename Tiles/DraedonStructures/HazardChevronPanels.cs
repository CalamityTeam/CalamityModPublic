using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HazardChevronPanels : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<LaboratoryDoorOpen>());
            CalamityUtils.SetMerge(Type, ModContent.TileType<LaboratoryDoorClosed>());
            CalamityUtils.SetMerge(Type, ModContent.TileType<AgedLaboratoryDoorOpen>());
            CalamityUtils.SetMerge(Type, ModContent.TileType<AgedLaboratoryDoorClosed>());
            CalamityUtils.SetMerge(Type, ModContent.TileType<LaboratoryPanels>());

            HitSound = SoundID.Tink;
            DustType = 19;
            MinPick = 30;
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.HazardChevronPanels> ();
            AddMapEntry(new Color(163, 150, 73));
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFraming.BetterGemsparkFraming(i, j, resetFrame);
        }
    }
}
