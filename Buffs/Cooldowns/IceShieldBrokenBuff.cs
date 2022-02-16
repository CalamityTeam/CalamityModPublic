using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class IceShieldBrokenBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Ice Shield Cooldown");
            Description.SetDefault("The shield is regenerating");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}
