using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Starfleet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starfleet");
            Tooltip.SetDefault("Fires a spread of plasma blasts");
        }

        public override void SetDefaults()
        {
            item.damage = 68;
            item.ranged = true;
            item.width = 76;
            item.height = 36;
            item.useTime = 55;
            item.useAnimation = 55;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 15f;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item92;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PlasmaBlast>();
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.FallenStar;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, -11);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 5; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StarCannonEX>());
            recipe.AddIngredient(ItemID.ElectrosphereLauncher);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
