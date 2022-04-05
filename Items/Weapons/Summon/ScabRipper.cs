using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ScabRipper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scab Ripper");
            Tooltip.SetDefault("Summons a baby blood crawler to protect you");
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item83;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BabyBloodCrawler>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                velocity.X = 0;
                velocity.Y = 0;
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.CrimtaneBar, 5).AddIngredient(ItemID.TissueSample, 9).AddIngredient(ItemID.Shadewood, 20).AddTile(TileID.Anvils).Register();
        }
    }
}
