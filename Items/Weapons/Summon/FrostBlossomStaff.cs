using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class FrostBlossomStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Blossom Staff");
            Tooltip.SetDefault("Summons a frozen flower over your head\n" +
                "There can only be one flower");
        }

        public override void SetDefaults()
        {
            item.damage = 10;
            item.mana = 10;
            item.width = 34;
            item.height = 24;
            item.useTime = item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item28;
            item.shoot = ModContent.ProjectileType<FrostBlossom>();
            item.shootSpeed = 10f;
            item.summon = true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Shiverthorn, 5);
            recipe.AddRecipeGroup("AnySnowBlock", 50);
            recipe.AddRecipeGroup("AnyIceBlock", 50);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
