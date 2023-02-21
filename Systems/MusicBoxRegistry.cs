using Terraria;
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
                AddMusicBox("Sounds/Music/DraedonsAmbience", ModContent.ItemType<Items.Placeables.MusicBoxes.DraedonsAmbienceMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.DraedonsAmbienceMusicBox>());
            }
        }
    }
}
