using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogPlanetoidGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            switch (Page)
            {
                case 0:
                    return "High above even the sky islands upheaved long ago lies a fascinating geological phenomenon. Though, unlike the islands, their airborne nature appears to have formed more naturally. I speculate that they formed long ago when the world was in its infancy judging by soil samples I have taken while testing high atmosphere plant growth. Perhaps these “planetoids”, as many refer to them as, formed via meteor strikes tossing matter high into the atmosphere. This would simultaneously explain many geological oddities I have found on the world’s surface.";
                case 1:
                    return "Travel between worlds is a fascination of mine. The planets within reasonable distance to this provide no use to me in terms of materials or study opportunities they could provide. And so, I wish to go further beyond this mere cluster of planets some day. It will likely take many more decades of technological progress, however, as travel between worlds would take years even at the maximum speed physical matter can currently go. Perhaps I will find a way to shatter this limit once this world holds my interest no longer.";
                default:
                    return "A request from Yharim has been delivered. An inquiry to create a suit of armor for the serpent that has joined his forces recently, the “Devourer of Gods” as the serpent calls himself. He is a bothersome yet fascinating being. And so, I have taken the opportunity so that I may learn more of his home dimension. Using materials from his place of origin, I have forged armor of immense size and durability yet extreme flexibility. It is certainly one of the most unique materials I have worked with and has advanced my knowledge greatly. Though, I have learned more than I expected to about the cosmic beast from this. Perhaps more than he wished for me to.";
            }
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogPlanetoid").Value;
                case 1:
                    return null;
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogDoGArmor").Value;
            }
        }
    }
}
