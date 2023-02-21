using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogSnowBiomeGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            switch (Page)
            {
                case 0:
                    return "A freezing tundra, where only creatures that have adapted to the subzero temperatures thrive. Few sapient beings have ever permanently resided here, save for miners and the Archmage. With this fact in mind, Yharim has requested I observe this place for oddities due to the armies largely having glossed over this location. Additionally, I have been requested by the Archmage to perform studies on his frigid metals. There is little else to do in this frozen wasteland, and so his request has been accepted.";
                case 1:
                    return "Cryonic Ore, as the Archmage simply calls it, is a material seemingly akin to iced over glass. One would assume it is a fragile material at a glance, yet upon testing it is clear that is not the case. Plating made out of it has been found to be more durable and resilient than mythril alloy via flexural and tensile testing, followed by destruction attempts. Yet, it is but a fraction of the weight. Perhaps mass amounts of this material could be used to forge armor for units who disdain for the usual burden of metal plating. I will likely inquire further about the creation methods of Cryonic Ore to achieve this.";
                default:
                    return "As described earlier, Cryonic Ore was developed by the Archmage, Permafrost. Despite his position as the mage of mages, he has little will to engage in combat. He instead acts as advisor to Yharim, assisting in much of the management of cities and towns taken by or sided with the Godkiller. In addition, he seems to have bonded with the girl who arrived seeking Yharim some months ago, acting as a fatherly figure to her. I myself have taken an interest in her immense natural power.";
            }
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogIceBiome").Value;
                case 1:
                    return null;
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogPermafrost").Value;
            }
        }
    }
}
