using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Statigel
{
    [AutoloadEquip(EquipType.Body)]
    public class StatigelArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Statigel Armor");
            Tooltip.SetDefault("5% increased critical strike chance");

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 10;
        }

        public override void UpdateEquip(Player player) => player.GetCritChance<GenericDamageClass>() += 5;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(12).
                AddIngredient<BlightedGel>(12).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
