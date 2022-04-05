using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class RampartofDeities : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rampart of Deities");
            Tooltip.SetDefault("Causes stars to fall and gives increased immune time when damaged\n" +
                "Provides life regeneration and reduces the cooldown of healing potions\n" +
                "Absorbs 25% of damage done to players on your team\n" +
                "This effect is only active above 25% life\n" +
                "Grants immunity to knockback\n" +
                "Puts a shell around the owner when below 50% life that reduces damage\n" +
                "The shell becomes more powerful when below 15% life and reduces damage even further");
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 62;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 18;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dAmulet = true;
            modPlayer.rampartOfDeities = true;
            modPlayer.fBulwark = true;
            player.longInvince = true;
            player.lifeRegen += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrigidBulwark>()
                .AddIngredient<DeificAmulet>()
                .AddIngredient<GalacticaSingularity>(5)
                .AddIngredient<DivineGeode>(10)
                .AddIngredient<CosmiliteBar>(10)
                .AddIngredient<AscendantSpiritEssence>(4)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }
}
