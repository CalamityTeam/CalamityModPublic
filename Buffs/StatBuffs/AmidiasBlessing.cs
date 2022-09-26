using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AmidiasBlessing : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amidias' Blessing");
            Description.SetDefault("You are blessed by Amidias" +
                                   "\nLets you breathe underwater, even in the Abyss!" +
                                   "\nJust don't get hit...");
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().amidiasBlessing = true;
            player.breath = player.breathMax + 91;
        }
    }
}
