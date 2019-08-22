using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class ThirdSageBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Third Sage");
			Description.SetDefault("Eh? No way it's an oni.");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.thirdSageH && !player.dead)
			{
				player.statLife = player.statLifeMax2;
				modPlayer.thirdSageH = false;
			}
			player.buffTime[buffIndex] = 18000;
			player.GetModPlayer<CalamityPlayer>(mod).thirdSage = true;
			bool PetProjectileNotSpawned = player.ownedProjectileCounts[mod.ProjectileType("ThirdSage")] <= 0;
			if (PetProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.position.X + (player.width / 2), player.position.Y + (player.height / 2), 0f, 0f, mod.ProjectileType("ThirdSage"), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}
