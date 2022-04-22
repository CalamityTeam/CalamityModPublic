using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class PlaguebringerBabBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer Bab");
            Description.SetDefault("The baby plaguebringer sees you as the queen");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().plaguebringerBab = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<PlaguebringerBab>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                int bee = Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<PlaguebringerBab>(), 0, 0f, player.whoAmI);
                Main.projectile[bee].frame = 2;
            }
        }
    }
}
