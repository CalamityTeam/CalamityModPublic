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
            Item.width = 44;
            Item.height = 48;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Summon;
            Item.UseSound = SoundID.Item89;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.autoReuse = true;

            Item.knockBack = 4f;
            Item.mana = 10;
            Item.damage = 28;
            Item.useTime = Item.useAnimation = 25;
            Item.shoot = ModContent.ProjectileType<ApexShark>();
            Item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddRecipeGroup("AnyAdamantiteBar", 5).AddIngredient(ItemID.AncientBattleArmorMaterial, 2).AddIngredient(ItemID.Amethyst, 4).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                player.itemTime = Item.useTime;
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                vector2.X = Main.mouseX + Main.screenPosition.X;
                vector2.Y = Main.mouseY + Main.screenPosition.Y;
                Projectile.NewProjectile(vector2, Vector2.Zero, ModContent.ProjectileType<ApexShark>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
