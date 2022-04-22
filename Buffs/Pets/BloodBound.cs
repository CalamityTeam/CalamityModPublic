using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class BloodBound : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Bound");
            Description.SetDefault("You must be desperate for company");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().perfmini = true;
            bool PetProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<PerforaMini>()] <= 0;
            if (PetProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<PerforaMini>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
