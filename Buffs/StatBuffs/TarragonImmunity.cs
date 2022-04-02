using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class TarragonImmunity : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Tarragon Immunity");
            Description.SetDefault("You are immune");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().tarragonImmunity = true;
        }
    }
}
