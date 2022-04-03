using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class SirenBobs : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Elemental");
            Description.SetDefault("You are a water elemental now");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.sirenBoobsPrevious)
            {
                modPlayer.sirenBoobsPower = true;
                player.ignoreWater = NPC.downedBoss3;
                player.accFlipper = true;
                if (player.breath <= player.breathMax + 2 && !modPlayer.ZoneAbyss && NPC.downedBoss3)
                {
                    player.breath = player.breathMax + 3;
                }
                if (Main.myPlayer == player.whoAmI && player.wet && NPC.downedBoss3)
                {
                    player.AddBuff(ModContent.BuffType<SirenWaterSpeed>(), 360);
                }
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
