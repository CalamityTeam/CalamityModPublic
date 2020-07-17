using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SkyGlaze : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Glaze");
            Tooltip.SetDefault("Fires feathers from the sky that stick to enemies and tiles and explode");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.magic = true;
            item.mana = 8;
            item.width = 52;
            item.height = 74;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 1;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<StickyFeather>();
            item.shootSpeed = 15f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			CalamityUtils.ProjectileToMouse(player, 3, item.shootSpeed, 0f, 30f, type, damage, knockBack, player.whoAmI, true);
            return false;
        }
    }
}
