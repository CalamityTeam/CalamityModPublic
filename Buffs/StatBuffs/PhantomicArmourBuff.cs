using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
	class PhantomicArmourBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Phantomic Shield");
            Description.SetDefault("Defense boosted by 10 and damage reduction boosted by 5%\n" +
                "An ephemeral bulwark protects you");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.05f;
            player.statDefense += 10;
        }
    }
}
