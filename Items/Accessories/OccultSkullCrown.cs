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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Occult Skull Crown");
            Tooltip.SetDefault("Only a fool would ever wear this...\n" +
                               "You constantly gain rage over time and rage does not fade away when out of combat\n" +
                               "Converts certain debuffs into buffs and extends their durations\n" +
                               "Debuffs affected: Darkness, Blackout, Confused, Slow, Weak,\n" +
                               "Broken Armor, Armor Crunch, Chilled, Ichor and Obstructed\n" +
                               "Adrenaline charges 20% faster\n" +
                               "Increases your max movement speed and acceleration by 5%\n" +
                               "Can also be worn as a helmet\n" +
                               "Revengeance item");
        }

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 62;
            Item.defense = 5;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.laudanum = true;
            modPlayer.heartOfDarkness = true;
            modPlayer.stressPills = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HeartofDarkness>().
                AddIngredient<Laudanum>().
                AddIngredient<StressPills>().
                AddIngredient<NightmareFuel>(20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
