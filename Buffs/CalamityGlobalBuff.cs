using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Buffs
{
	public class CalamityGlobalBuff : GlobalBuff
	{
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.Shine)
            {
                player.GetModPlayer<CalamityPlayer>(mod).shine = true;
            }
            else if (type == BuffID.IceBarrier)
            {
                player.endurance -= 0.1f;
            }
			else if (type == BuffID.ObsidianSkin)
			{
				player.lavaMax += 420;
			}
			else if (type == BuffID.Rage)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 10;
            }
            else if (type == BuffID.Wrath)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
            }
            else if (type == BuffID.WellFed)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
            }
            else if (type >= BuffID.NebulaUpDmg1 && type <= BuffID.NebulaUpDmg3)
            {
                float nebulaDamage = 0.075f * (float)player.nebulaLevelDamage; //7.5% to 22.5%
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += nebulaDamage;
				player.meleeDamage -= nebulaDamage;
				player.rangedDamage -= nebulaDamage;
				player.magicDamage -= nebulaDamage;
				player.minionDamage -= nebulaDamage;
			}
			else if (type >= BuffID.NebulaUpLife1 && type <= BuffID.NebulaUpLife3)
			{
				player.lifeRegen -= 5 * player.nebulaLevelLife; //10 to 30 changed to 5 to 15
			}
			else if (type == BuffID.Warmth)
            {
                player.buffImmune[mod.BuffType("GlacialState")] = true;
                player.buffImmune[BuffID.Frozen] = true;
                player.buffImmune[BuffID.Chilled] = true;
            }
        }

		public override void ModifyBuffTip(int type, ref string tip, ref int rare)
		{
			if (type == BuffID.NebulaUpDmg1)
				tip = "7.5% increased damage";
			if (type == BuffID.NebulaUpDmg2)
				tip = "15% increased damage";
			if (type == BuffID.NebulaUpDmg3)
				tip = "22.5% increased damage";
			if (type == BuffID.ChaosState)
			{
				if (CalamityWorld.revenge)
				{
					int closestPlayer = (int)Player.FindClosest(new Vector2((float)(Main.maxTilesX / 2), (float)Main.worldSurface / 2f) * 16f, 0, 0);
					if (!Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).normalityRelocator)
						tip = "Using the Rod of Discord will take life and damage taken is increased by 50%";
					else
						tip = "Using the Rod of Discord or Normality Relocator will take life";
				}
				else
				{
					tip = "Using the Rod of Discord or Normality Relocator will take life";
				}
			}
		}
	}
}
