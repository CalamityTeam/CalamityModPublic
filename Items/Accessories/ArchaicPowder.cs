using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ArchaicPowder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archaic Powder");
            Tooltip.SetDefault("20% increased mining speed, 7% damage reduction, and +3 defense while underground or in the underworld");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight)
            {
                player.statDefense += 3;
                player.endurance += 0.07f;
                player.pickSpeed -= 0.2f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AncientFossil>());
            recipe.AddIngredient(ModContent.ItemType<DemonicBoneAsh>());
            recipe.AddIngredient(ModContent.ItemType<AncientBoneDust>(), 3);
            recipe.AddIngredient(ItemID.Bone, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
