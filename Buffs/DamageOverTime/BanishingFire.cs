using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class BanishingFire : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banishing Fire");
            Description.SetDefault("You shall not be forgiven for your sins");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().banishingFire = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().banishingFire < npc.buffTime[buffIndex])
                npc.Calamity().banishingFire = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
