using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
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
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<RotomPet>(), 0, 0f, player.whoAmI);
            }
        }
    }
}
