using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

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
            item.useTime = 36;
            item.useAnimation = 36;
            item.scale = 0.85f;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.DankCreeper>();
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

        public override bool AltFunctionUse(Player player)
        {
            return true;
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

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.MinionNPCTargetAim();
            }
            return base.UseItem(player);
        }
    }
}
