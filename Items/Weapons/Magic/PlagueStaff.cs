using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PlagueStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Staff");
            Tooltip.SetDefault("Fires a spread of plague fangs");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 78;
            item.magic = true;
            item.mana = 22;
            item.width = 46;
            item.height = 46;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item43;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PlagueFang>();
            item.shootSpeed = 16f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int fangAmt = 6;
            if (Main.rand.NextBool(3))
            {
                fangAmt++;
            }
            if (Main.rand.NextBool(4))
            {
                fangAmt++;
            }
            if (Main.rand.NextBool(5))
            {
                fangAmt++;
            }
			CalamityUtils.ProjectileToMouse(player, fangAmt, item.shootSpeed, 0.05f, 120f, type, damage, knockBack, player.whoAmI, false);
            return false;
        }
    }
}
