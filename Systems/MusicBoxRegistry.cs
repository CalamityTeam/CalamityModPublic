using CalamityMod.Items.Placeables.MusicBoxes;
using CalamityMod.Tiles.MusicBoxes;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod
{
    public class MusicBoxRegistry : ModSystem
    {
        private static void AddMusicBox(string musicFile, int itemID, int tileID)
        {
            Mod calamity = CalamityMod.Instance;
            int musicID = MusicLoader.GetMusicSlot(calamity, musicFile);
            MusicLoader.AddMusicBox(calamity, musicID, itemID, tileID);
        }

        public override void PostSetupContent()
        {
            if (!Main.dedServ)
            {
                AddMusicBox("Sounds/Music/DraedonAmbience", ModContent.ItemType<DraedonsAmbienceMusicBox>(), ModContent.TileType<DraedonsAmbienceMusicBoxTile>());
            }
        }
    }
}
