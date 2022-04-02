using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DreadmineStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dreadmine Staff");
            Tooltip.SetDefault("Summons a dreadmine turret to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.mana = 10;
            item.width = 44;
            item.height = 44;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item113;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DreadmineTurret>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.sentry = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 30);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
