using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class HellionFlowerSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellion Flower Spear");
            Tooltip.SetDefault("Shoots a flower spear tip\n" +
                "Summons petals from the sky on critical hits");
        }

        public override void SetDefaults()
        {
            item.width = 64;
            item.damage = 135;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 20;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<HellionFlowerSpearProjectile>();
            item.shootSpeed = 8f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
