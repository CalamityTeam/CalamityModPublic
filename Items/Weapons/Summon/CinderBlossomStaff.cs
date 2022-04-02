using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CinderBlossomStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder Blossom Staff");
            Tooltip.SetDefault("Summons a really hot flower over your head\n" +
                "There can only be one flower");
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.mana = 10;
            item.width = 46;
            item.height = 46;
            item.useTime = item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item34;
            item.shoot = ModContent.ProjectileType<CinderBlossom>();
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
            recipe.AddIngredient(ItemID.Fireblossom, 5);
            recipe.AddIngredient(ItemID.LavaBucket, 2);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.AddRecipe();
        }
    }
}
