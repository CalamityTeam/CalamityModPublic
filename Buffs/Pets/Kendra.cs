using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class Kendra : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kendra");
            Description.SetDefault("A loyal friend accompanies you.");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().kendra = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<KendraPet>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<KendraPet>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
