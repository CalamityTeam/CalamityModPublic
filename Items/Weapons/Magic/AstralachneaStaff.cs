using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AstralachneaStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astralachnea Staff");
            Tooltip.SetDefault("Fires a spread of homing astral spider fangs");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.mana = 19;
            item.width = 52;
            item.height = 52;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item46;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AstralachneaFang>();
            item.shootSpeed = 13f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int spikeAmount = 4;
            if (Main.rand.NextBool(3))
            {
                spikeAmount++;
            }
            if (Main.rand.NextBool(4))
            {
                spikeAmount++;
            }
            if (Main.rand.NextBool(5))
            {
                spikeAmount += 2;
            }
			CalamityUtils.ProjectileToMouse(player, spikeAmount, item.shootSpeed, 0.05f, 400f, type, damage, knockBack, player.whoAmI, false);
            return false;
        }
    }
}
