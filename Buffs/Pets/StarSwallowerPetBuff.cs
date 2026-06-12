using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class StarSwallowerPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref player.Calamity().starSwallowerPetFroge, ModContent.ProjectileType<StarSwallowerPet>());
    }
}
