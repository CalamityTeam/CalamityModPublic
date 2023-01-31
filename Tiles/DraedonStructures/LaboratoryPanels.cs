using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryPanels : ModTile
    {
        public static readonly SoundStyle MinePlatingSound = new("CalamityMod/Sounds/Custom/PlatingMine", 3);
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<HazardChevronPanels>());

            HitSound = MinePlatingSound;
            DustType = 109;
            MinPick = 30;
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryPanels>();
            AddMapEntry(new Color(36, 35, 37));
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
