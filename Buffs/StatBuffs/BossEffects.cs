using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class BossEffects : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boss Effects");
            Description.SetDefault("This tooltip is edited in the function below");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // TODO -- This bool does nothing except enable boss zen config. All other effects are hardcoded.
            // Is this intended?
            player.Calamity().isNearbyBoss = true;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            StringBuilder sb = new StringBuilder(512);
            sb.Append("The nearby boss is:\n");
            if (CalamityConfig.Instance.BossZen)
                sb.Append("Greatly reducing enemy spawn rates\n");
            sb.Append("Increasing Nurse healing cost by 400%\n");
            sb.Append("Disabling Target and Super Dummy hitboxes");

            tip = sb.ToString();
        }
    }
}
