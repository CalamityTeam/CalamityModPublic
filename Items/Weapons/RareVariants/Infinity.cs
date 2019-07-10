using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.RareVariants
{
	public class Infinity : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Infinity");
			Tooltip.SetDefault("Bad PC\n" +
				"Fires a barrage of energy bolts that split and bounce\n" +
				"Right click to fire a barrage of normal bullets");
		}

	    public override void SetDefaults()
	    {
			item.damage = 30;
			item.ranged = true;
			item.width = 56;
			item.height = 24;
			item.useTime = 2;
			item.reuseDelay = 6;
			item.useAnimation = 18000;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item31;
			item.autoReuse = true;
			item.shoot = 10;
			item.shootSpeed = 12f;
			item.useAmmo = 97;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	    
	    public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int bulletAmt = 2;
			if (player.altFunctionUse == 2)
    		{
			    for (int index = 0; index < bulletAmt; ++index)
			    {
			        float num7 = speedX;
			        float num8 = speedY;
			        float SpeedX = speedX + (float) Main.rand.Next(-15, 16) * 0.05f;
			        float SpeedY = speedY + (float) Main.rand.Next(-15, 16) * 0.05f;
			        int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
			    return false;
			}
			else
			{
			    for (int index = 0; index < bulletAmt; ++index)
			    {
			        float num7 = speedX;
			        float num8 = speedY;
			        float SpeedX = speedX + (float) Main.rand.Next(-15, 16) * 0.05f;
			        float SpeedY = speedY + (float) Main.rand.Next(-15, 16) * 0.05f;
			        int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("ChargedBlast"), damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
			    return false;
			}
		}
	}
}
