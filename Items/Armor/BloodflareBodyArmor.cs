using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class BloodflareBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Bloodflare Body Armor");
            Tooltip.SetDefault("12% increased damage and 8% increased critical strike chance\n" +
                       "+40 max life");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.defense = 35;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.Calamity().AllCritBoost(8);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(16).
                AddIngredient<RuinousSoul>(4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
