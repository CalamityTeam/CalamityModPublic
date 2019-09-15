using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PetBuffs
{
    public class StrangeOrb : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Young Siren");
			Description.SetDefault("Small and cute");
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

		public override void Update(Player player, ref int buffIndex)
		{
            player.buffTime[buffIndex] = 18000;
            player.GetCalamityPlayer().sirenPet = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[mod.ProjectileType("SirenYoung")] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, mod.ProjectileType("SirenYoung"), 0, 0f, player.whoAmI, 0f, 0f);
            }
		}
	}
}
