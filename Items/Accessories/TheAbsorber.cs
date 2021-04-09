using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheAbsorber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Absorber");
            Tooltip.SetDefault("6% increased movement speed\n" +
                "12% increased jump speed\n" +
                "+20 max life and mana\n" +
                "Standing still boosts life and mana regen\n" +
                "Increased defense, movement speed and damage reduction while submerged in liquid\n" +
                "Enemies take damage when they hit you\n" +
                "Taking a hit will make you move very fast for a short time\n" +
                "You emit a mushroom spore and spark explosion when you are hit\n" +
                "5% increased damage reduction\n" +
                "Enemy attacks will have part of their damage absorbed and used to heal you");
        }

        public override void SetDefaults()
        {
            item.defense = 6;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.accessory = true;
        }

		public override void ModifyTooltips(List<TooltipLine> list)
		{
			bool autoJump = Main.player[Main.myPlayer].autoJump;
			string jumpAmt = autoJump ? "3" : "12";
			foreach (TooltipLine line2 in list)
			{
				if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					line2.text = jumpAmt + "% increased jump speed";
			}
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aSpark = true;
            modPlayer.gShell = true;
            modPlayer.fCarapace = true;
            modPlayer.seaShell = true;
            modPlayer.absorber = true;
            player.statManaMax2 += 20;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GrandGelatin>());
            recipe.AddIngredient(ModContent.ItemType<SeaShell>());
            recipe.AddIngredient(ModContent.ItemType<CrawCarapace>());
            recipe.AddIngredient(ModContent.ItemType<FungalCarapace>());
            recipe.AddIngredient(ModContent.ItemType<GiantTortoiseShell>());
            recipe.AddIngredient(ModContent.ItemType<AmidiasSpark>());
            recipe.AddIngredient(ModContent.ItemType<RoverDrive>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GrandGelatin>());
            recipe.AddIngredient(ModContent.ItemType<SeaShell>());
            recipe.AddIngredient(ModContent.ItemType<FungalCarapace>());
            recipe.AddIngredient(ModContent.ItemType<GiantShell>());
            recipe.AddIngredient(ModContent.ItemType<GiantTortoiseShell>());
            recipe.AddIngredient(ModContent.ItemType<AmidiasSpark>());
            recipe.AddIngredient(ModContent.ItemType<RoverDrive>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
