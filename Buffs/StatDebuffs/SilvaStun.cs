using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class SilvaStun : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Silva Stun");
            Description.SetDefault("Can't move");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().silvaStun = true;
        }
    }
}
