using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Zapper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lazinator");
            Tooltip.SetDefault("Zap");
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.magic = true;
            item.mana = 4;
            item.width = 46;
            item.height = 22;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item12;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurpleLaser;
            item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpaceGun);
            recipe.AddIngredient(ItemID.LaserRifle);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
