using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Azathoth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Azathoth");
            Tooltip.SetDefault("Fires cosmic orbs that blast nearby enemies with lasers\n" +
            "A very agile yoyo\n" +
            "Destroy the universe in the blink of an eye");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 54;
            item.melee = true;
            item.damage = 50;
            item.knockBack = 6f;
            item.useTime = 20;
            item.useAnimation = 20;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<AzathothYoyo>();
            item.shootSpeed = 16f;

            item.rare = ItemRarityID.Red;
            item.autoReuse = true;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.value = Item.buyPrice(platinum: 5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Terrarian);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
