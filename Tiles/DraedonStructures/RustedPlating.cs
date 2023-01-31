using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class RustedPlating : ModTile
    {
        public static readonly SoundStyle MinePlatingSound = new("CalamityMod/Sounds/Custom/PlatingMine", 3);
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            HitSound = MinePlatingSound;
            DustType = 32;
            MinPick = 30;
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.RustedPlating>();
            AddMapEntry(new Color(128, 90, 77));
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFraming.BetterGemsparkFraming(i, j, resetFrame);
        }
    }
}
