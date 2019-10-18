using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ArchAmaryllis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arch Amaryllis");
            Tooltip.SetDefault("Casts a beaming bolt that explodes into smaller homing bolts");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 75;
            item.magic = true;
            item.mana = 10;
            item.width = 66;
            item.height = 68;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BeamingBolt>();
            item.shootSpeed = 20f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "GleamingMagnolia");
            recipe.AddIngredient(ItemID.FragmentNebula, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
