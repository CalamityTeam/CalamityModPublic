using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    class PhantomicRestorationBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantomic Regen");
            Description.SetDefault("Regenerating life");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer calPlayer = player.Calamity();
            if (calPlayer.phantomicHeartRegen <= 0)
            {
                calPlayer.phantomicHeartRegen = 1000;
            }
            player.lifeRegen += 2;
        }
    }
}
