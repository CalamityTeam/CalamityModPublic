using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GoldplumeSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goldplume Spear");
            Tooltip.SetDefault("Shoots falling feathers");
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.damage = 48;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 23;
            item.knockBack = 5.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<GoldplumeSpearProjectile>();
            item.shootSpeed = 8f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 10);
            recipe.AddIngredient(ItemID.SunplateBlock, 4);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
