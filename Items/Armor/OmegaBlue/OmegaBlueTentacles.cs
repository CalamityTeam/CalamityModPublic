using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.OmegaBlue
{
    [AutoloadEquip(EquipType.Legs)]
    [LegacyName("OmegaBlueLeggings")]
    public class OmegaBlueTentacles : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Omega Blue Tentacles");
            Tooltip.SetDefault(@"12% increased movement speed
12% increased damage and critical strike chance");

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.defense = 22;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.GetCritChance<GenericDamageClass>() += 12;
            player.moveSpeed += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ReaperTooth>(10).
                AddIngredient<Lumenyl>(6).
                AddIngredient<Tenebris>(6).
                AddIngredient<RuinousSoul>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
