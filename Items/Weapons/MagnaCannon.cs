using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class MagnaCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magna Cannon");
            Tooltip.SetDefault("Fires a concentrated blast of energy");
        }

        public override void SetDefaults()
        {
            item.damage = 19;
            item.magic = true;
            item.mana = 12;
            item.width = 56;
            item.height = 34;
            item.useTime = 32;
            item.useAnimation = 32;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item117;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<MagnaBlast>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 3; ++index)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(double)damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Granite, 25);
            recipe.AddIngredient(ItemID.Obsidian, 15);
            recipe.AddIngredient(ItemID.Amber, 5);
            recipe.AddIngredient(ItemID.SpaceGun);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
