using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CoralSpout : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Spout");
            Tooltip.SetDefault("Casts coral water spouts that lay on the ground and damage enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 17;
            item.magic = true;
            item.mana = 4;
            item.width = 28;
            item.height = 30;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item17;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CoralSpike>();
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 2);
            recipe.AddIngredient(ItemID.Coral, 5);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
