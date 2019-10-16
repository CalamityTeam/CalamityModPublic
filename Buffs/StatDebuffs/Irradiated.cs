using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class Irradiated : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Acid Rain");
            Description.SetDefault("Your skin is burning off");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().irradiated = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().irradiated = true;
        }
    }
}
