using CalamityMod.CalPlayer;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class ProfanedBabs : ModBuff //Buff name is reference to how I refer to the guardians as my babs ~Amber
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Profaned Soul");
            Description.SetDefault("The healer will heal your wounds!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.gHealer || modPlayer.profanedCrystalBuffs)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];
            bool offense = player.Calamity().gOffense;
            bool defense = player.Calamity().gDefense;
            if (player.Calamity().profanedCrystal && (!player.Calamity().profanedCrystalBuffs && !CalamityWorld.downedSCal))
            {
                tip = "The Profaned Babs will accompany you!";
            }
            else if (offense && defense)
            {
                tip = "The Profaned Babs will fight for and defend you!";
            }
            else if (offense || defense)
            {
                tip = "The " + (offense ? "Offensive" : "Defensive") + " Duo will " + (offense ? "fight for and heal you!" : "protect and heal you!");
            }
        }
    }
}
