using CalamityMod.Buffs.Placeables;
using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class EffigyOfDecayPlaceable : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 2;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Effigy of Decay");
            AddMapEntry(new Color(113, 90, 71), name);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player is null)
                return;
            if (!player.dead && player.active)
                player.AddBuff(ModContent.BuffType<EffigyOfDecayBuff>(), 20);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 32, ModContent.ItemType<EffigyOfDecay>());
        }
    }
}
