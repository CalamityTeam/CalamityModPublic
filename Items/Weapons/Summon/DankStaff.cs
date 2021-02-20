using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DankStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dank Staff");
            Tooltip.SetDefault("Summons a dank creeper to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 7;
            item.mana = 10;
            item.width = 58;
            item.height = 58;
            item.useTime = item.useAnimation = 30;
            item.scale = 0.85f;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DankCreeperMinion>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RottenChunk, 3);
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>(), 7);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
