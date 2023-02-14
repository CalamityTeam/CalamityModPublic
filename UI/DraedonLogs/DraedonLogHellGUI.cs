using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogHellGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            switch (Page)
            {
                case 0:
                    return "The Underworld is a realm with history dating back more than a millenia when the Crags were first settled, and acts as a glance into both the modern and ancient world simultaneously. Prior to my joining of Yharim’s forces, Azafure was the center of scientific advancement in this world. Explosives, firearms, smithing techniques, and even entire schools of magic have roots in the city. It is doubtless that the city has been largely aided by the fact that forging is trivial in the Underworld, along with an abundance of food due to Silva’s essence dripping down from the Abyss above. Due to this, it has prospered even with the occasional attack from Demons.";
                case 1:
                    return "Azafure has not existed without consequence, of course. Any society on such a large scale leaves an ecological footprint behind. This can be seen most clearly in the sea far above the city, appropriately known as the Sulphurous Sea. Brimstone fumes became intense enough in the city that they began rising up through the Abyss via a process akin to osmosis, and eventually reached the sea. Reducing the pollution the city creates, particularly from forging, is a goal Yharim wishes for me to prioritize. I see little need in it, yet it is a field of study few in this world have delved into. I refuse to squander an opportunity for scientific breakthrough.";
                default:
                    return "A particular being here has been the subject of my interest: a massive wall created from biological material lies in this place created many years ago by the greatest mages Azafure has to offer. It is a living being, though only just. It lacks both pain and emotion, and brings terror to all who lay their eyes upon it. It is kept protected via high security by the city, and so few aside from the Godseeker’s closest allies have had the opportunity to observe it. For as long as Azafure continues to stand, it is unlikely to ever be truly released. Yet, I do not deny that I would be fascinated to see the effects its release would have on the world.";
            }
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogCragsBiome").Value;
                case 1:
                    return null;
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogWallOfFlesh").Value;
            }
        }
    }
}
