using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class ElementalGauntlet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Gauntlet");
            Tooltip.SetDefault("Melee attacks and projectiles inflict on fire, frostburn and holy flames\n" +
                "15% increased melee speed, damage, and 5% increased melee critical strike chance\n" +
                "20% increased true melee damage\n" +
                "Temporary immunity to lava\n" +
                "Increased melee knockback");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eGauntlet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.FireGauntlet).AddIngredient(ModContent.ItemType<YharimsInsignia>()).AddIngredient(ItemID.LunarBar, 8).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 4).AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
