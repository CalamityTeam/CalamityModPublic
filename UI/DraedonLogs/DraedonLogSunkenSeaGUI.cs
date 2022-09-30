using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogSunkenSeaGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            switch (Page)
            {
                case 0:
                    return "Preserved for millennia, a paradise for the living beings who sought shelter in prehistoric seas. They remain untouched by evolution, save for their adaptations " +
                           "to the oxygen starved waters and dim crystals, continuing to thrive. One mystery which continues to escape my understanding however, is how large some of the creatures " +
                           "have become. There is a blatant lack of nutrition and oxygen in the caves, and yet...";
                case 1:
                    return "The marine life in these caves do have eyes though they are barely functional, dulled by lack of use and milky white upon observation. On their tough, gnarled hides, " +
                           "crystals find an easy purchase, and grow in great numbers, providing the creatures protection. Perhaps another adaptation to the life they have adopted. The most striking " +
                           "wonder is within their bodies. In specimens dissected, I have noticed that the mineral is buried into their very digestive systems, and perhaps, through some chemical process, " +
                           "pass nutrients into their sluggish hosts. A peculiar yet entirely beneficial interaction.";
                default:
                    return "A specimen which has developed a grand size, and inexplicably, impressive psychic abilities. What is most curious is its strong connection to its " +
                           "lesser kin. Without any noticeable communication, when it comes under threat, other mollusks rally to its aggressor and begin attacking. Is this " +
                           "perhaps the very first signs of a higher life form, the evolutionary link hidden away in the sunken sea? Or a self sacrificial fluke, which would " +
                           "lead to their destruction if they inhabited any area other than these pacified caverns.";
            }
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogSunkenSeaBiome").Value;
                case 1:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogEutrophicRayGhostBell").Value;
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogGiantClam").Value;
            }
        }
    }
}
