using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Cnidarian : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cnidarian");
            Tooltip.SetDefault("Fires a seashell when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 26;
            item.melee = true;
            item.damage = 11;
            item.knockBack = 3f;
            item.useTime = 25;
            item.useAnimation = 25;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<CnidarianYoyo>();
            item.shootSpeed = 10f;

            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(gold: 2);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
