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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Archaic Powder");
            Tooltip.SetDefault("20% increased mining speed, 5% damage reduction and +10 defense while underground or in the underworld");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight)
            {
                player.statDefense += 10;
                player.endurance += 0.05f;
                player.pickSpeed -= 0.2f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AncientFossil>().
                AddIngredient<DemonicBoneAsh>().
                AddIngredient<AncientBoneDust>(3).
                AddIngredient(ItemID.Bone, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
