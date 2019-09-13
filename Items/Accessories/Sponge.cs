using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class Sponge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Sponge");
            Tooltip.SetDefault("50% increased mining speed and you emit light\n" +
                "10% increased damage reduction and increased life regen\n" +
                "Poison, Freeze, Chill, Frostburn, and Venom immunity\n" +
                "Honey-like life regen with no speed penalty, +20 max life and mana\n" +
                "Most bee/hornet enemies and projectiles do 75% damage to you\n" +
                "120% increased jump speed and 12% increased movement speed\n" +
                "Standing still boosts life and mana regen\n" +
                "Increased defense and damage reduction when submerged in liquid\n" +
                "Increased movement speed when submerged in liquid\n" +
                "Enemies take damage when they hit you\n" +
                "Taking a hit will make you move very fast for a short time\n" +
                "You emit a mushroom spore and spark explosion when you are hit\n" +
                "Enemy attacks will have part of their damage absorbed and used to heal you");
        }

        public override void SetDefaults()
        {
            item.defense = 10;
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.beeResist = true;
            modPlayer.aSpark = true;
            modPlayer.gShell = true;
            modPlayer.fCarapace = true;
            modPlayer.absorber = true;
            modPlayer.aAmpoule = true;
            player.statManaMax2 += 20;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TheAbsorber");
            recipe.AddIngredient(null, "AmbrosialAmpoule");
            recipe.AddIngredient(null, "CosmiliteBar", 15);
            recipe.AddIngredient(null, "Phantoplasm", 15);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
