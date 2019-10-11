using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class AbyssalDivingSuitPlatesBroken : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abyssal Diving Suit Plates Broken");
            Description.SetDefault("The plates are regenerating");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().abyssalDivingSuitCooldown = true;
        }
    }
}
