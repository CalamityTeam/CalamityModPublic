using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WeakPetrification : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Weak Petrification");
            Description.SetDefault("Your vertical movement is weakened");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().weakPetrification = true;
        }
    }
}
