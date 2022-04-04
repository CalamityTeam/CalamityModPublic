using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            int num6 = 3;
            float SpeedX = velocity.X + (float)Main.rand.Next(-20, 21) * 0.05f;
            float SpeedY = velocity.Y + (float)Main.rand.Next(-20, 21) * 0.05f;
            for (int index = 0; index < num6; ++index)
            {
                int projectile = Projectile.NewProjectile(source, position.X, position.Y, SpeedX * 1.05f, SpeedY * 1.05f, ProjectileID.LaserMachinegunLaser, (int)(damage * 0.65), knockback * 0.6f, player.whoAmI, 0f, 0f);
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
