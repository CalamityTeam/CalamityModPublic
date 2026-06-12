using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
    public sealed class ColorTool : GlobalItem
    {
        public static int RarityCosmicPurple => ModContent.GetInstance<CosmicPurple>().Type;
        public static int RarityBurnishedAuric => ModContent.GetInstance<BurnishedAuric>().Type;
        public static int RarityCalamityRed => ModContent.GetInstance<CalamityRed>().Type;
        public static int RarityExoticRainbow => ModContent.GetInstance<ExoticRainbow>().Type;


        public override bool PreDrawTooltipLine(Item Item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName" && CalamityClientConfig.Instance.TextEffects)
            {
                if (Item.rare == RarityCosmicPurple)
                {
                    CosmicPurple.Draw(Item, line);
                    return false;
                }
                if (Item.rare == RarityBurnishedAuric)
                {
                    BurnishedAuric.Draw(Item, line);
                    return false;
                }
                if (Item.rare == RarityCalamityRed)
                {
                    CalamityRed.Draw(Item, line);
                    return false;
                }

                if (Item.rare == RarityExoticRainbow)
                {
                    ExoticRainbow.Draw(Item, line);
                    return false;
                }
            }
            return true;
        }
        public static Color colorLerps(Color[] colors, float time)
        {
            int index = (int)time;
            return Color.Lerp(colors[index % colors.Length], colors[(index + 1) % colors.Length], time % 1f);
        }

        public static Color Rainbowing(float position) => colorLerps(
            [ new Color(255, 50, 50, 255), new Color(255, 128, 50, 255), new Color(230, 255, 50, 255), new Color(80, 255, 60, 255),
                new Color(50, 80, 250, 255), new Color(200, 50, 250, 255), new Color(255, 50, 230, 255), ], position);
    }
}
