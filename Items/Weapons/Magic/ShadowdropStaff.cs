using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ShadowdropStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowdrop Staff");
            Tooltip.SetDefault("Summons dark aura rain from the sky");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 19;
            item.magic = true;
            item.mana = 13;
            item.width = 44;
            item.height = 44;
            item.useTime = 9;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item66;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AuraRain>();
            item.shootSpeed = 8f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(7, 7);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RottenChunk, 5);
            recipe.AddIngredient(ItemID.DemoniteBar, 5);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>(), 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			CalamityUtils.ProjectileToMouse(player, 10, item.shootSpeed, 0f, 120f, type, damage, knockBack, player.whoAmI, true);
            return false;
        }
    }
}
