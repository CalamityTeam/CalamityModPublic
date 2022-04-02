using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TerraLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Lance");
            Tooltip.SetDefault("Fires a lance beam");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.damage = 88;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 17;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<TerraLanceProjectile>();
            item.shootSpeed = 11f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChlorophytePartisan);
            recipe.AddIngredient(ItemID.DarkLance);
            recipe.AddIngredient(ItemID.Gungnir);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
