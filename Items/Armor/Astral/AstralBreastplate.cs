using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Astral
{
    [AutoloadEquip(EquipType.Body)]
    public class AstralBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Astral Breastplate");
            Tooltip.SetDefault("+80 max mana and +20 max life\n" +
                               "Creature detection");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 25;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.statManaMax2 += 80;
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(12).
                AddIngredient(ItemID.MeteoriteBar, 9).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
