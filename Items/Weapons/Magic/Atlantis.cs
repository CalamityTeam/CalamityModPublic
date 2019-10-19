using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Atlantis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlantis");
            Tooltip.SetDefault("Casts aquatic spears that split as they travel");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.magic = true;
            item.mana = 12;
            item.width = 28;
            item.height = 30;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item34;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AtlantisSpear>();
            item.shootSpeed = 32f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<IOU>());
            recipe.AddIngredient(ModContent.ItemType<LivingShard>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
