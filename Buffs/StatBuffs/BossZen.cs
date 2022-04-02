using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class BossZen : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Boss Effects");
            Description.SetDefault("This tooltip is edited in the function below");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bossZen = true;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            tip = "The nearby boss is:\n" +
                "Greatly reducing enemy spawn rates\n" +
                "Disabling teleportation effects\n" +
                "Increasing Nurse healing cost by 400%\n" +
                "Disabling Target and Super Dummy hitboxes";
        }
    }
}
