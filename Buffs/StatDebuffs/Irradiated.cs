using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class Irradiated : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Irradiated");
            Description.SetDefault("Your skin is burning off");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().irradiated = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().irradiated < npc.buffTime[buffIndex])
                npc.Calamity().irradiated = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
