using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
	public class DepthBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Depth Blade");
			Tooltip.SetDefault("Hitting enemies will cause the crush depth debuff\n" +
				"The lower the enemies' defense the more damage they take from this debuff");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.damage = 22;
			item.melee = true;
			item.useAnimation = 22;
			item.useTime = 22;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 5.25f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 40;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
		}

	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
	        }
	    }

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	target.AddBuff(mod.BuffType("CrushDepth"), 180);
		}
	}
}
