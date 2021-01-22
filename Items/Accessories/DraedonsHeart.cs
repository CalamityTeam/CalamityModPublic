using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DraedonsHeart : ModItem
    {
        // The percentage of a full Rage bar that is gained every second with Draedon's Heart equipped.
        public const float MinRagePerSecond = 0.015f;
        public const float MaxRagePerSecond = 0.045f; // 3x rage generation at 0% health, aka +200% rage generation

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Heart");
            Tooltip.SetDefault("Boosts your damage by 5% and max movement speed and acceleration by 5%\n" +
                "You generate rage over time and rage does not fade away out of combat\n" +
                "Passive rage generation increases drastically as health decreases\n" +
                "Converts certain debuffs into buffs and extends their durations\n" +
                "Debuffs affected: Darkness, Blackout, Confused, Slow, Weak, Broken Armor,\n" +
                "Armor Crunch, War Cleave, Chilled, Ichor and Obstructed\n" +
                "Receiving a hit causes you to only lose half of your max adrenaline rather than all of it\n" +
                "Reduces the amount of defense stat damage you take by 50%\n" +
                "Standing still regenerates your life quickly, reduces your damage by 50% and boosts your defense by 75%\n" +
                "Nanomachines, son");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 7));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.laudanum = true;
            modPlayer.stressPills = true;
            modPlayer.heartOfDarkness = true;
            modPlayer.draedonsHeart = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<HeartofDarkness>());
            recipe.AddIngredient(ModContent.ItemType<StressPills>());
            recipe.AddIngredient(ModContent.ItemType<Laudanum>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>());
            recipe.AddIngredient(ItemID.Nanites, 250);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
