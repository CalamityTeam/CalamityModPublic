using CalamityMod.DataStructures;
using CalamityMod.Items.Potions.Alcohol;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class TequilaBuff : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            AlcoholLevel = 1
        };
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var cplayer = player.Calamity();
            cplayer.tequila = true;
            cplayer.ElectricDebuffMultiplier += Tequila.DebuffBoost;
            cplayer.ColdDebuffMultiplier -= Tequila.DebuffLoss;
        }
    }
}
