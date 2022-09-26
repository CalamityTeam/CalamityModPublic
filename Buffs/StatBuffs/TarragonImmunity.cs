using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class TarragonImmunity : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Immunity");
            Description.SetDefault("You are immune");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().tarragonImmunity = true;
        }
    }
}
