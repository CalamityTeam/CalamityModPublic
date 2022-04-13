using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class FearmongerPlateMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fearmonger Plate Mail");
            Tooltip.SetDefault("+100 max life and 8% increased damage reduction\n" +
            "5% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(gold: 60);
            Item.defense = 50;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 100;
            player.endurance += 0.08f;
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.Calamity().AllCritBoost(5);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpookyBreastplate).
                AddIngredient<CosmiliteBar>(12).
                AddIngredient(ItemID.SoulofFright, 12).
                AddIngredient<AscendantSpiritEssence>(3).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
