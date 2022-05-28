using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class BurningBlood : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Blood");
            Description.SetDefault("Your blood is on fire");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bBlood = true;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.025f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().bBlood < npc.buffTime[buffIndex])
                npc.Calamity().bBlood = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
