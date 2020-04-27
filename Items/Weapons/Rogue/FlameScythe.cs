using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Hybrid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FlameScythe : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subduction Slicer");
            Tooltip.SetDefault("Throws a scythe that explodes on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 50;
			item.height = 48;
            item.damage = 90;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
			item.useStyle = 1;
			item.useTime = 21;
            item.useAnimation = 21;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(gold: 80);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<FlameScytheProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
