using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PetBuffs
{
	public class BloodBound : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Blood Bound");
			Description.SetDefault("You must be desperate for company");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 18000;
			player.GetModPlayer<CalamityPlayer>(mod).perfmini = true;
			bool PetProjectileNotSpawned = player.ownedProjectileCounts[mod.ProjectileType("PerforaMini")] <= 0;
			if (PetProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.position.X + (player.width / 2), player.position.Y + (player.height / 2), 0f, 0f, mod.ProjectileType("PerforaMini"), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}
