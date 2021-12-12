using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Head)]
    public class OccultSkullCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Occult Skull Crown");
            Tooltip.SetDefault("Only a fool would ever wear this...\n" +
                               "You constantly gain rage over time and rage does not fade away when out of combat\n" +
                               "Converts certain debuffs into buffs and extends their durations\n" +
                               "Debuffs affected: Darkness, Blackout, Confused, Slow, Weak, Broken Armor,\n" +
                               "Armor Crunch, War Cleave, Chilled, Ichor and Obstructed\n" +
                               "Getting hit causes you to only lose half of your max adrenaline rather than all of it\n" +
                               "Boosts your defense by 5 and max movement speed and acceleration by 5%\n" +
                               "Can also be worn as a helmet\n" +
                               "Revengeance item");
        }

        public override void SetDefaults()
        {
            item.width = 82;
            item.height = 62;

            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.statDefense += 5;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.laudanum = true;
            modPlayer.heartOfDarkness = true;
            modPlayer.stressPills = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<HeartofDarkness>());
            recipe.AddIngredient(ModContent.ItemType<Laudanum>());
            recipe.AddIngredient(ModContent.ItemType<StressPills>());
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
