using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
	public class HalibutCannon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Halibut Cannon");
            Tooltip.SetDefault("This weapon is overpowered, use at the risk of ruining your playthrough\n" +
				"Revengeance drop");
        }

	    public override void SetDefaults()
	    {
			item.damage = 6;
			item.ranged = true;
			item.width = 108;
			item.height = 54;
			item.useTime = 10;
			item.useAnimation = 20;
			item.useStyle = 5;
            item.rare = 10;
			item.noMelee = true;
			item.knockBack = 1f;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.UseSound = SoundID.Item38;
			item.autoReuse = true;
			item.shoot = 10;
			item.shootSpeed = 12f;
			item.useAmmo = 97;
		}
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-15, 0);
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.hardMode) { item.damage = 12; }
            if (NPC.downedMoonlord) { item.damage = 24; }
            int num6 = Main.rand.Next(25, 36);
            for (int index = 0; index < num6; ++index)
            {
                float num7 = speedX;
                float num8 = speedY;
                float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
                int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                Main.projectile[shot].timeLeft = 180;
            }
            return false;
        }
	}
}