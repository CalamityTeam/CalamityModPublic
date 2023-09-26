using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ProfanedCrystalBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.profanedCrystal)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.Calamity().profanedCrystalBuffs)
            {
                bool empowered = player.Calamity().pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Empowered;
                if (empowered)
                {
                    tip += "\n" + this.GetLocalizedValue("Empowered");
                }
                else
                {
                    bool offense = (Main.dayTime && !player.wet) || player.lavaWet;
                    tip += "\n" + (offense ? this.GetLocalization("Offense").Format(this.GetLocalizedValue(player.lavaWet ? "Lava" : "Day"))
                        : this.GetLocalization("Defense").Format(this.GetLocalizedValue(player.honeyWet ? "Honey" : (player.wet ? "Water" : "Night"))));

                    if (!Main.dayTime)
                        tip += "\n" + this.GetLocalizedValue("Enrage");
                }
            }
            else if (DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs)
            {
                tip = this.GetLocalizedValue("LockedSlots");
            }
            else
            {
                tip = this.GetLocalizedValue(!DownedBossSystem.downedExoMechs ? "LockedExos" : "LockedSCal");
            }
        }
    }
}
