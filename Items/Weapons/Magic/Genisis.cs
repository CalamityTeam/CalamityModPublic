using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Genisis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Genisis");
            Tooltip.SetDefault("Fires a Y-shaped beam of destructive energy and a spread of lasers");
        }

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 74;
            Item.height = 28;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<BigBeamofDeath>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            int num6 = 3;
            float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
            for (int index = 0; index < num6; ++index)
            {
                int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX * 1.05f, SpeedY * 1.05f, ProjectileID.LaserMachinegunLaser, (int)(damage * 0.65), knockBack * 0.6f, player.whoAmI, 0f, 0f);
                Main.projectile[projectile].timeLeft = 120;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.LaserMachinegun).AddIngredient(ItemID.LunarBar, 5).AddIngredient(ModContent.ItemType<BarofLife>(), 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
