using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class DraedonLogSnowBiomeGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            switch (Page)
            {
                case 0:
                    return "A freezing tundra, where only creatures entirely adapted to the subzero temperatures exist and thrive. It is a shocking transition from the forests of the purity and " +
                           "the sun baked desert. A climate like this should not exist naturally in this part of the world with ease. The weather patterns seem to shift unnaturally arounds the skies " +
                           "of these icy plains. There is likely a reason for this, which necessitates further research.";
                case 1:
                    return "Intriguing. Though embedded deep into the caverns of ice and worn from centuries of frost and meltwater, I have uncovered several mechanisms which once filled the tunnels here. " +
                           "The ingenuity present is remarkable, and I have found parallels within my own work, as well as devices even I have something to learn from. From where do these come? Why machinery " +
                           "so complex in so sparse and dreary a habitat? Perhaps, they are related to the unnatural conditions.";
                default:
                    return "I am not the only singular being to inhabit this biome. Once before, the Archmage who opposed the Lord resided here, cloaked by constant artificial blizzards of his own creation, " +
                           "which no longer fall. He likely chose this place as a conduit for research into his ice spells, and extended the period of time that this place remained frozen. Deep underground my " +
                           "research and materials lay well protected, but above in the natural storms there are traces of the prison of ice he resides in, still haunting its place of creation.";
            }
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonsLogIceBiome").Value;
                case 1:
                    return null; // No image exists for Daedalus' ruins at the moment.
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonsLogPermafrost").Value;
            }
        }
    }
}
