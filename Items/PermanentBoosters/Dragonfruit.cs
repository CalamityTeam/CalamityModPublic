using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.Items.PermanentBoosters
{
    public class Dragonfruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonfruit");
            Tooltip.SetDefault("Though somewhat bland, what taste can be described is unlike any other experienced\n" +
                               "Permanently increases maximum life by 25\n" +
                               "Can only be used if the max amount of life fruit has been consumed");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.dFruit)
            {
                string key = "Mods.CalamityMod.DragonfruitText";
                Color messageColor = Color.Cyan;
                CalamityUtils.DisplayLocalizedText(key, messageColor);

                return false;
            }
            else if (player.statLifeMax < 500)
            {
                return false;
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                if (Main.myPlayer == player.whoAmI)
                {
                    player.HealEffect(25);
                }
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.dFruit = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null && Main.LocalPlayer.Calamity().dFruit)
                line.Text = "[c/8a8a8a:You have already consumed this item]";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LifeFruit, 5).
                AddIngredient(ItemID.SkyBlueFlower).
                AddIngredient(ItemID.FragmentSolar, 15).
                AddIngredient<YharonSoulFragment>(2).
                AddIngredient<AscendantSpiritEssence>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
