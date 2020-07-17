using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
	public class BittercoldStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bittercold Staff");
            Tooltip.SetDefault("Fires a spread of homing ice spikes");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.magic = true;
            item.mana = 17;
            item.width = 52;
            item.height = 52;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item46;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<IceSpike>();
            item.shootSpeed = 10f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int spikeAmt = 2;
            if (Main.rand.NextBool(3))
            {
                spikeAmt++;
            }
            if (Main.rand.NextBool(4))
            {
                spikeAmt++;
            }
            if (Main.rand.NextBool(5))
            {
                spikeAmt++;
            }
			CalamityUtils.ProjectileToMouse(player, spikeAmt, item.shootSpeed, 0.05f, 100f, type, damage, knockBack, player.whoAmI, false);
            return false;
        }
    }
}
