using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class Molten : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Molten");
            Description.SetDefault("Resistant to cold effects");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().molten = true;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
			if (CalamityWorld.death)
				tip += ". Provides cold protection in Death Mode";
		}
    }
}
