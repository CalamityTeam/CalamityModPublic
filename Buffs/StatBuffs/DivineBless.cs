using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class DivineBless : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Divine Bless");
            Description.SetDefault("Increased health regen and minions inflict Banishing Fire");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().divineBless = true;
        }
    }
}
