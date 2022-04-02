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
            item.damage = 15;
            item.mana = 10;
            item.width = 66;
            item.height = 70;
            item.useTime = item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 0.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item83;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BabyBloodCrawler>();
            item.shootSpeed = 10f;
            item.summon = true;
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
        
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe.AddIngredient(ItemID.TissueSample, 9);
            recipe.AddIngredient(ItemID.Shadewood, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
