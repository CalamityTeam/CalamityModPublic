using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ForgottenApexWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forgotten Apex Wand");
            Tooltip.SetDefault("Summons ancient mineral sharks to take on your foes. \n" +
                               "Seems to have lost its jaw some time in the past");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 48;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.summon = true;
            item.UseSound = SoundID.Item89;
            item.noMelee = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.autoReuse = true;

            item.knockBack = 4f;
            item.mana = 10;
            item.damage = 28;
            item.useTime = item.useAnimation = 25;
            item.shoot = ModContent.ProjectileType<ApexShark>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddRecipeGroup("AnyAdamantiteBar", 5);
            recipe.AddIngredient(ItemID.Amethyst, 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                player.itemTime = item.useTime;
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                vector2.X = Main.mouseX + Main.screenPosition.X;
                vector2.Y = Main.mouseY + Main.screenPosition.Y;
                Projectile.NewProjectile(vector2, Vector2.Zero, ModContent.ProjectileType<ApexShark>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
