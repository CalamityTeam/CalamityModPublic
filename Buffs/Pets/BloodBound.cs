using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class BloodBound : ModBuff
    {
        public override void SetDefaults()
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
                Projectile.NewProjectile(player.position.X + (player.width / 2), player.position.Y + (player.height / 2), 0f, 0f, ModContent.ProjectileType<PerforaMini>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
