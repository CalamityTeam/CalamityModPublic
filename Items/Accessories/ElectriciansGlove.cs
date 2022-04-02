using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class ElectriciansGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrician's Glove");
            Tooltip.SetDefault(@"Stealth strikes summon sparks on enemy hits
Stealth strikes also have +10 armor penetration, deal 10% more damage, and heal for 1 HP");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 40;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.electricianGlove = true;
            modPlayer.bloodyGlove = true;
            modPlayer.filthyGlove = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FilthyGlove>());
            recipe.AddIngredient(ItemID.Wire, 100);
            recipe.AddRecipeGroup("AnyMythrilBar", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstainedGlove>());
            recipe.AddIngredient(ItemID.Wire, 100);
            recipe.AddRecipeGroup("AnyMythrilBar", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
