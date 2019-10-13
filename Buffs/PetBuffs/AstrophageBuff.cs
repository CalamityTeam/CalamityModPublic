using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.PetBuffs
{
    public class AstrophageBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Astrophage");
            Description.SetDefault("Little astral buggy");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().astrophage = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[mod.ProjectileType("Astrophage")] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, mod.ProjectileType("Astrophage"), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
