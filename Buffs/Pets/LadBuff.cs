using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class LadBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Father");
            Description.SetDefault("A father of many floats around you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().ladShark = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<LadShark>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<LadShark>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
