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
            Item.width = 24;
            Item.height = 40;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<FilthyGlove>()).AddIngredient(ItemID.Wire, 100).AddRecipeGroup("AnyMythrilBar", 5).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BloodstainedGlove>()).AddIngredient(ItemID.Wire, 100).AddRecipeGroup("AnyMythrilBar", 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
