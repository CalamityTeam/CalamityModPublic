using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Leviathan
{
    public class SirensSong : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Siren's Song");
            Tooltip.SetDefault("Casts slow-moving treble clefs that confuse enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 77;
            item.magic = true;
            item.mana = 7;
            item.width = 56;
            item.height = 50;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("SirensSong");
            item.shootSpeed = 13f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "IOU");
            recipe.AddIngredient(null, "LivingShard");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-15, 16) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-15, 16) * 0.05f;
			float soundPitch = (Main.rand.NextFloat() - 0.5f) * 0.5f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, soundPitch, 0f);
            return false;
        }
    }
}
