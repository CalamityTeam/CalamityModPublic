using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Magic;

namespace CalamityMod.Buffs.StatBuffs
{
    public class CoralSymbiosis : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Symbiosis");
            Description.SetDefault($"Coral Spout charges up faster and deals {CoralSpout.SymbiosisDamageBuff} extra damage");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CoralSpoutPlayer>().Symbiosis = true;
        }
    }
}
