using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class ThirdSageBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Third Sage");
            Description.SetDefault("Eh? No way it's an oni.");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().thirdSage = true;
            bool PetProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<ThirdSage>()] <= 0;
            if (PetProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<ThirdSage>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
