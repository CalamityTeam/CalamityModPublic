using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AccretionDisk : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Disk");
            Tooltip.SetDefault("Throws a disk that has a chance to generate several disks if enemies are near it");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 157;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.height = 38;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<AccretionDiskProj>();
            item.shootSpeed = 13f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MangroveChakram>());
            recipe.AddIngredient(ModContent.ItemType<FlameScythe>());
            recipe.AddIngredient(ModContent.ItemType<SeashellBoomerang>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
