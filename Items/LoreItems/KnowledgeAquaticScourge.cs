using System.Collections.Generic;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// Make sure to have these 2 directives for all Lore Items.
using Microsoft.Xna.Framework;
using static Microsoft.Xna.Framework.Input.Keys;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeAquaticScourge : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Scourge");
            Tooltip.SetDefault("[c/7FFFD4:Whispers echo in your ears...]\n" + "[c/BEBEBE:Press \"Left Shift\" to listen closer...]");
            // The color of the text that will be shown when Left Shift is pressed and then BEBEBE is a gray color.
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AquaticScourgeTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) 
            // For Lore Items, you copy whatever this line is and you input the text and color below.
        {
            if (Main.keyState.IsKeyDown(LeftShift))
            {
                int tooltipIndex = -1;
                int tooltipCount = 0;

                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Name.StartsWith("Tooltip"))
                    {
                        if (tooltipIndex == -1)
                            tooltipIndex = i;

                        tooltipCount++;
                    }
                }

                if (tooltipIndex != -1)
                {
                    tooltips.RemoveRange(tooltipIndex, tooltipCount);

                    TooltipLine extendedLore = new TooltipLine(Mod, "CalamityMod:HiddenTooltip",
                        "Another once grand sea serpent, well-adapted to its harsh environs.\n" +
                        "Unlike the other Scourge, which was half starved and chasing scraps for its next meal, it lived comfortably.\n" +
                        "Microorganisms evolve rapidly, so it was able to maintain its filter feeding habits as the sea putrefied.\n" +
                        "What a stark contrast to the rest of the ecosystem. Nearly every other creature in the Sulphur Sea is hostile.\n" +
                        "A shame that its last bastion of tranquility has fallen.");
                    extendedLore.OverrideColor = new Color(127, 255, 212); // The color of the text shown when Left Shift is pressed.
                    tooltips.Insert(tooltipIndex, extendedLore);
                }
            }
        }
    }
}
