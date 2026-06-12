using CalamityMod.DataStructures;
using CalamityMod.Items.Potions.Alcohol;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class ScrewdriverBuff : ModBuff
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
            player.Calamity().screwdriver = true;
            player.blockRange += 5;
            player.tileSpeed += 1;
            player.wallSpeed += 1;
            player.GetDamage(DamageClass.Generic) *= 0.75f;
        }
    }
}
