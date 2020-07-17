using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PhoenixFlameBarrage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phoenix Flame Barrage");
            Tooltip.SetDefault("Baptism by holy fire\n" +
                "Casts a barrage of fire from the sky");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 290;
            item.magic = true;
            item.mana = 20;
            item.width = 72;
            item.height = 70;
            item.useTime = 15;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<HolyFlame>();
            item.shootSpeed = 30f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int flameAmt = 30;
            if (Main.rand.NextBool(3))
            {
                flameAmt++;
            }
            if (Main.rand.NextBool(3))
            {
                flameAmt++;
            }
			CalamityUtils.ProjectileToMouse(player, flameAmt, item.shootSpeed, 0f, 30f, type, damage, knockBack, player.whoAmI, true);
            return false;
        }
    }
}
