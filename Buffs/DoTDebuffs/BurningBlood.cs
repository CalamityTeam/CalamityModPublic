using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DoTDebuffs
{
    public class BurningBlood : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Burning Blood");
            Description.SetDefault("Your blood is on fire");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bBlood = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().bBlood = true;
        }
    }
}
