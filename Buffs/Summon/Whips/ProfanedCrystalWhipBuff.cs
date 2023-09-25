using static CalamityMod.Items.Accessories.ProfanedSoulCrystal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon.Whips
{
    public class ProfanedCrystalWhipBuff : ModBuff
    {
        public override string Texture => "CalamityMod/Buffs/Summon/Whips/SentinalLash";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];
            bool empowered = player.Calamity().pscState == (int)ProfanedSoulCrystalState.Empowered;
            if (empowered)
            {
                tip += "\n" + this.GetLocalizedValue("Empowered");
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.Calamity().profanedCrystalStatePrevious < (int)ProfanedSoulCrystalState.Buffs)
            {
                player.ClearBuff(Type);
                return;
            }
                
            var whipBuffs = new int[]
            {
                BuffID.CoolWhipPlayerBuff, BuffID.ScytheWhipPlayerBuff, BuffID.SwordWhipPlayerBuff,
                BuffID.ThornWhipPlayerBuff
            };

            foreach (int buff in whipBuffs)
            {
                player.ClearBuff(buff);
            }
        }
    }
}
