using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class ExoskeletonCannons : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ares' Cannons");
            Description.SetDefault("A purple cannon is missing from the set");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (AresExoskeleton.ArmExists(player))
                modPlayer.AresCannons = true;

            if (modPlayer.AresCannons)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
