using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class HazardChevronPanels : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<LaboratoryDoorOpen>());
            CalamityUtils.SetMerge(Type, ModContent.TileType<LaboratoryDoorClosed>());
            CalamityUtils.SetMerge(Type, ModContent.TileType<AgedLaboratoryDoorOpen>());
            CalamityUtils.SetMerge(Type, ModContent.TileType<AgedLaboratoryDoorClosed>());

            soundType = SoundID.Tink;
            dustType = 19;
            minPick = 30;
            drop = ModContent.ItemType<Items.Placeables.DraedonStructures.HazardChevronPanels> ();
            AddMapEntry(new Color(163, 150, 73));
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFraming.BetterGemsparkFraming(i, j, resetFrame);
        }
    }
}
