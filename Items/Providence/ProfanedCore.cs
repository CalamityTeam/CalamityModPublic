using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Providence
{
    public class ProfanedCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Core");
            Tooltip.SetDefault("The core of the unholy flame\n" +
                "Summons Providence\n" +
                "Can only be used during daytime");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = true;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(mod.NPCType("Providence")) && Main.dayTime && (player.ZoneHoly || player.ZoneUnderworldHeight) && CalamityWorld.downedBossAny;
        }

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Providence"));
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
    }
}
