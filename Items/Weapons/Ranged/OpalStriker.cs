using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class OpalStriker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Opal Striker");
            Tooltip.SetDefault("Fires a string of opal strikes");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.ranged = true;
            item.width = 64;
            item.height = 16;
            item.useTime = 5;
            item.reuseDelay = 25;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/OpalStrike");
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<OpalStrike>();
            item.shootSpeed = 6f;
            item.useAmmo = 97;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<OpalStrike>(), damage, 0f, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Marble, 20);
            recipe.AddIngredient(ItemID.Amber, 5);
            recipe.AddIngredient(ItemID.Diamond, 3);
            recipe.AddIngredient(ItemID.FlintlockPistol);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
