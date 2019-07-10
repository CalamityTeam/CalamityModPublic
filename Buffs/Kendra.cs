using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;

namespace CalamityMod.Buffs
{
	public class Kendra : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Kendra");
			Description.SetDefault("A loyal friend accompanies you.");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<CalamityPlayer>(mod).kendra = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[mod.ProjectileType("Kendra")] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.position.X + (float)(player.width / 2), (player.position.Y + (float)(player.height / 2)) - 20f, 0f, 0f, mod.ProjectileType("Kendra"), 0, 0f, player.whoAmI, 0f, 0f);
			}
        }
	}
}
