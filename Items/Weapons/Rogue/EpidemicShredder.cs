using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EpidemicShredder : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Epidemic Shredder");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.damage = 75;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 12;
            item.useTime = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<EpidemicShredderProjectile>();
            item.shootSpeed = 18f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 20);
            recipe.AddIngredient(ItemID.Nanites, 150);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}