using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class ManaBurn : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mana Burn");
            Description.SetDefault("The excess of mana sears your body and mind");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex) => player.Calamity().ManaBurn = true;
    }
}