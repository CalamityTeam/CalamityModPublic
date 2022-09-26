using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AdrenalineMode : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adrenaline Mode");
            Description.SetDefault("200% damage boost. Can be boosted by other items up to 245%.");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().adrenalineModeActive = true;
        }
    }
}
