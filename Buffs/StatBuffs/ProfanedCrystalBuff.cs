using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ProfanedCrystalBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devotion");
            Description.SetDefault("");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.Calamity().profanedCrystalBuffs)
            {
                if (player.Calamity().endoCooper)
                {
                    tip = "An ascended ice construct is suppressing your true potential..";
                }
                else if (player.Calamity().magicHat)
                {
                    tip = "A magical hat overwhelms your senses, squandering your true potential..";
                }
                else
                {
                    bool offense = (Main.dayTime && !player.wet) || player.lavaWet;
                    bool enrage = player.statLife <= (int)(player.statLifeMax2 * 0.5);
                    tip = "You are an emissary of the profaned goddess now!\n" +
                        (offense ? "The " + (Main.dayTime ? "light of the sun" : "heat of the lava") + " empowers your offensive capabilities" :
                        "The " + (player.wet ? (player.honeyWet ? "honey cools" : "water douses") : "darkness of night cools") + " your flames, empowering your defensive capabilities") +
                        (enrage ? "\nYour weakened life force fuels your desperate attacks" : "");
                }
            }
            else if (DownedBossSystem.downedSCal && DownedBossSystem.downedExoMechs)
            {
                tip = "Your profaned soul is constrained by your insufficient summoning powers";
            }
            else
            {
                tip = !DownedBossSystem.downedExoMechs ? "The soul within this crystal has been defiled by overwhelming energy waves from dangerous mechanations" : "The profaned soul within has been defiled by the powerful magic of a supreme witch";
            }
        }
    }
}
