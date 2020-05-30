using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class Horror : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Horror");
            Description.SetDefault("The horror...");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().horror = true;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];
			if (player.Calamity().laudanum || player.Calamity().draedonsHeart)
			{
				tip = "The horror fuels your strength";
			}
		}
    }
}
