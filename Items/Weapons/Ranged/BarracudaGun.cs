using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BarracudaGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barracuda Gun");
            Tooltip.SetDefault("Fires two barracudas that latch onto enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 63;
            item.channel = true;
            item.ranged = true;
            item.width = 54;
            item.height = 28;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item10;
            item.autoReuse = true;
            item.shootSpeed = 15f;
            item.shoot = ModContent.ProjectileType<MechanicalBarracuda>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 2; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PiranhaGun);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 2);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>());
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddIngredient(ItemID.SharkFin, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
