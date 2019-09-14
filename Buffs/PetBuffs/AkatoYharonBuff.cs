using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PetBuffs
{
    public class AkatoYharonBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Akato");
			Description.SetDefault("'Looks like you'll have to take care of it now'");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

		public override void Update(Player player, ref int buffIndex)
		{
            player.buffTime[buffIndex] = 18000;
            player.GetCalamityPlayer().akato = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[mod.ProjectileType("Akato")] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, mod.ProjectileType("Akato"), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
	}
}
