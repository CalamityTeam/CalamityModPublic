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

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (CalamityConfig.Instance.BossZen)
                tip = tip.Replace(":", ":\n" + this.GetLocalizedValue("ZenDescription"));
        }
    }
}
