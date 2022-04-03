using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Placeables
{
    public class PurpleDefenseCandle : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resilience");
            Description.SetDefault("Neither rain nor wind can snuff its undying flame");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().purpleCandle = true;
        }
    }
}
