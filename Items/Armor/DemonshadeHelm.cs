using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor 
{
	[AutoloadEquip(EquipType.Head)]
	public class DemonshadeHelm : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Helm");
            Tooltip.SetDefault("30% increased damage and critical strike chance, +10 max minions");
        }

	    public override void SetDefaults()
	    {
	        item.width = 18;
	        item.height = 18;
	        item.value = 10000000;
	        item.defense = 50; //15
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(255, 0, 255);
	            }
	        }
	    }
	
	    public override bool IsArmorSet(Item head, Item body, Item legs)
	    {
	        return body.type == mod.ItemType("DemonshadeBreastplate") && legs.type == mod.ItemType("DemonshadeGreaves");
	    }
	    
	    public override void ArmorSetShadows(Player player)
	    {
	    	player.armorEffectDrawShadow = true;
	    	player.armorEffectDrawOutlines = true;
	    }
	
	    public override void UpdateArmorSet(Player player)
	    {
	        player.setBonus = "All attacks inflict the demon flame debuff\n" +
	        	"Shadowbeams and demon scythes will fire down when you are hit\n" +
	        	"A friendly red devil follows you around\n" +
                "Removes the damage and critical strike chance caps\n" +
                "Press Y to enrage nearby enemies with a dark magic spell for 10 seconds\n" +
                "This makes them do 1.5 times more damage but they also take three times as much damage";
	        CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
	        modPlayer.dsSetBonus = true;
	        modPlayer.redDevil = true;
			if (player.whoAmI == Main.myPlayer)
			{
				if (player.FindBuffIndex(mod.BuffType("RedDevil")) == -1)
				{
					player.AddBuff(mod.BuffType("RedDevil"), 3600, true);
				}
				if (player.ownedProjectileCounts[mod.ProjectileType("RedDevil")] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("RedDevil"), 0, 0f, Main.myPlayer, 0f, 0f);
				}
			}
	    }
	    
	    public override void UpdateEquip(Player player)
	    {
	    	player.maxMinions += 10;
			player.meleeDamage += 0.3f;
	       	player.thrownDamage += 0.3f;
		    player.rangedDamage += 0.3f;
	        player.magicDamage += 0.3f;
	        player.minionDamage += 0.3f;
	   	    player.meleeCrit += 30;
			player.magicCrit += 30;
			player.rangedCrit += 30;
			player.thrownCrit += 30;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "ShadowspecBar", 8);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}