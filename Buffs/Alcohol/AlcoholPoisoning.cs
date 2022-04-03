using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class AlcoholPoisoning : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alcohol Poisoning");
            Description.SetDefault("You drank too much and you are now dying");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().alcoholPoisoning = true;
        }
    }
}
