using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonSchematicPlanetoidGUI : DraedonsLogGUI
    {
        public override int TotalPages => 1;
        public override string GetTextByPage()
        {
            return "Within an army, as weapons do, the soldiers serve different purposes.\n" +
                "That distinction is crucial, as the wrong tool in the wrong hands—no matter how potent—may as well be a wooden club.\n" +
                "Addendum: Seek out my base of operations closest to the Lihzahrd's home.\n" + 
                "I wish you the best of luck with all sincerity, for it has been a long time since I have had a worthy test subject.";
        }
        public override Texture2D GetTextureByPage()
        {
            return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogPlanetoid").Value;
        }
    }
}
