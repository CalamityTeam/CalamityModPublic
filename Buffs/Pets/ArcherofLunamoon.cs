using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class ArcherofLunamoon : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archer of Lunamoon");
            Description.SetDefault("You have a personal spotter");
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 2;
            player.Calamity().spiritOriginPet = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<DaawnlightSpiritOriginMinion>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, -Vector2.UnitY * 3f, ModContent.ProjectileType<DaawnlightSpiritOriginMinion>(), 0, 0f, player.whoAmI, 0f, 0f);
        }
    }
}
