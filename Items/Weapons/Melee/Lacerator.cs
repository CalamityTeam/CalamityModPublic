using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lacerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacerator");
            Tooltip.SetDefault("Enemies that are hit by the yoyo will have their life drained\n" +
                "Someone thought this was a viable weapon against DoG at one point lol");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheEyeOfCthulhu);
            item.damage = 150;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LaceratorProjectile>();
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
