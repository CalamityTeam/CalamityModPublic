using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.Items.PermanentBoosters
{
    public class Elderberry : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elderberry");
            Tooltip.SetDefault("A tangy, tart flavor with a somewhat earthen touch\n" +
                               "Permanently increases maximum life by 25\n" +
                               "Can only be used if the max amount of life fruit has been consumed");
            SacrificeTotal = 1;
			// For some reason Life/Mana boosting items are in this set (along with Magic Mirror+)
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 18; // Life Fruit
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 34;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.eBerry)
            {
                string key = "Mods.CalamityMod.ElderberryText";
                Color messageColor = Color.RoyalBlue;
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
                modPlayer.eBerry = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null && Main.LocalPlayer.Calamity().eBerry)
                line.Text = "[c/8a8a8a:You have already consumed this item]";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LifeFruit, 5).
                AddIngredient(ItemID.BlueBerries).
                AddIngredient<UelibloomBar>(10).
                AddIngredient<DivineGeode>(10).
                AddIngredient<UnholyEssence>(20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
