using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Shadowflame : ModBuff
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("BuffName.ShadowFlame");
        public override LocalizedText Description => Language.GetOrRegister("BuffDescription.ShadowFlame");
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().shadowflame = true;
        }
    }
}
