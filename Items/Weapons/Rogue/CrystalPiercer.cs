using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CrystalPiercer : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Piercer");
        }

        public override void SafeSetDefaults()
        {
            item.width = 62;
            item.damage = 52;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 17;
            item.useStyle = 1;
            item.useTime = 17;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.maxStack = 999;
            item.value = 2500;
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<CrystalPiercerProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
