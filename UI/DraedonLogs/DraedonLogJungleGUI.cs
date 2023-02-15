using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogJungleGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            switch (Page)
            {
                case 0:
                    return "It would not be a stretch to call the jungle the hub of this planet. All is centered around it, and none know not of it. It brings me no small amount of unease to see the uncomfortable, raw forms of the living beings who pass through on their journeys above. Fortunately, these labs provide everything I need in my research and more. There is little need to visit the surface, save for Yharim’s summons.";
                case 1:
                    return "Few travel down here, with far fewer making the expedition to reach as far down as this research station. As such, I have taken to using this isolation to develop advanced nanotechnology for use in controlling the minds of biologicals. I yearn for it to be capable of spreading from organism to organism, a perfected blend of a virus and nanotechnology. Yet, it has been an arduous task even with my newly developed technologies. Further experimentation and research are required.";
                default:
                    return "Continued experiments have led to significant results. With a mechanically modified Queen Bee as a primary host, it may act as the core of a hive mind of those my plague has touched. However, Yharim was outraged upon learning of my experiments here and decried them as inhumane. After a brief verbal conflict, he wordlessly left and I have not heard from him since. Fortunately, I need not rely on the slayer of gods any longer, nor have I needed to for years.";
            }
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogJungleBiome").Value;
                case 1:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogPlagueCell").Value;
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogPlaguebringerGoliath").Value;
            }
        }
    }
}
