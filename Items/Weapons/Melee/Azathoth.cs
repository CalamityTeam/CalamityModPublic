using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Azathoth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Azathoth");
            Tooltip.SetDefault("Destroy the universe in the blink of an eye\n" +
                "Fires cosmic orbs that blast nearby enemies with lasers");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Kraken);
            item.damage = 200;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AzathothProjectile>();
            item.Calamity().postMoonLordRarity = 16;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Terrarian);
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddIngredient(null, "CoreofCalamity", 3);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
