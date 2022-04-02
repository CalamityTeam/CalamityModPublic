using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Whirlpool : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Riptide");
            Tooltip.SetDefault("Sprays a spiral of aqua streams in random directions\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 44;
            item.melee = true;
            item.damage = 16;
            item.knockBack = 1f;
            item.useTime = 25;
            item.useAnimation = 25;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<RiptideYoyo>();
            item.shootSpeed = 18f;

            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(gold: 2);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 7);
            recipe.AddIngredient(ModContent.ItemType<Navystone>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
