using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class Fox : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fox Pet");
            Description.SetDefault("Fox Pet? FOX PET");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().fox = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<FoxPet>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<FoxPet>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
