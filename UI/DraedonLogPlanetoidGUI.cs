using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class DraedonLogPlanetoidGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            switch (Page)
            {
                case 0:
                    return "Hung low in orbit, masses of ground and various parts of the world provide a secluded and distant point for research. Undeniably optimal for the science of astronomy and otherwise. " +
                           "In my labs here I grow many things, testing their limits against the cold and vacuum of the stratosphere. Though not many survive, the existence of certain creatures here, confirm the " +
                           "capabilities of life simply given more time.";
                case 1:
                    return "I do not care much for the interstellar, or the cosmos. Though I have traversed it, there is still plenty in my own world to manage and discover at this time. Even if I once inhabited a " +
                           "different planet, the Lord's wishes that I provide him machinery were the only condition that I needed to leave it and settle elsewhere. Once I have discovered and dissected every part of " +
                           "this place, perhaps then, I could look up towards the macroscopic.";
                default:
                    return "The bloated cosmic worm, though I understand why the Lord decides to employ it given he can control it, is a disgusting existence. However the idea of creating an armor suited to it in every " +
                           "way, was an offer I could not refuse. Forged from the cosmic steel of my own creation, it resists nearly any attack, yet allows the creature the same flexibility it would have without it, as well " +
                           "as augmenting its dimensional abilities. I remain pleased with the result.";
            }
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonsLogPlanetoid").Value;
                case 1:
                    return null; // The Draedon's backstory page does not have an image, and probably shouldn't.
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonsLogDoGArmor").Value;
            }
        }
    }
}
