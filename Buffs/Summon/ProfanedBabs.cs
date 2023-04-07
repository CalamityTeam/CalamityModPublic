using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class ProfanedBabs : ModBuff //Buff name is reference to how I refer to the guardians as my babs ~Amber
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Profaned Soul");
            // Description.SetDefault("The Profaned Babs will fight for and defend you!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.donutBabs || modPlayer.profanedCrystalBuffs)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
                player.buffTime[buffIndex] = 18000;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.Calamity().profanedCrystal && !player.Calamity().profanedCrystalBuffs)
                tip = "The Profaned Babs will accompany you!";
        }
    }
}
