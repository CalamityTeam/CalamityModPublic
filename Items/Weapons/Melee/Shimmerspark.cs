using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Shimmerspark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shimmerspark");
            Tooltip.SetDefault("Fires stars when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 36;
            item.melee = true;
            item.damage = 41;
            item.knockBack = 3.5f;
            item.useTime = 25;
            item.useAnimation = 25;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<ShimmersparkYoyo>();
            item.shootSpeed = 12f;

            item.rare = ItemRarityID.Pink;
            item.value = Item.buyPrice(gold: 36);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
