using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WitherDebuff : ModBuff
    {
        public static int DefenseReduction = 20;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wither");
            Description.SetDefault("Withered...\n" +
                "Defense reduced by 20"); // Literally Ichor but patron.  Also not to be confused with Withered Armor/Weapons
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().wither < npc.buffTime[buffIndex])
                npc.Calamity().wither = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().wither = true;
        }
    }
}
