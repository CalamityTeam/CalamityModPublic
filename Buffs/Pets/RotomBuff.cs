using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class RotomBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Electric Troublemaker");
            Description.SetDefault("Lightning never strikes the same place twice");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().rotomPet = true;
            bool PetProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<RotomPet>()] <= 0;
            if (PetProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
				float baseDamage = 10f +
					(Main.hardMode ? 10f : 0f) +
					(NPC.downedMoonlord ? 30f : 0f);
                Projectile.NewProjectile(player.position.X + (player.width / 2), player.position.Y + (player.height / 2), 0f, 0f, ModContent.ProjectileType<RotomPet>(), (int)(baseDamage * (player.allDamage + player.minionDamage - 1f)), 0f, player.whoAmI, 50f, 0f);
            }
        }
    }
}
