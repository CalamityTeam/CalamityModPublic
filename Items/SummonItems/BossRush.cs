using CalamityMod.Projectiles.Typeless;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class BossRush : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terminus");
            Tooltip.SetDefault("A ritualistic artifact, thought to have brought upon The End many millennia ago\n" +
                                "Sealed away in the abyss, far from those that would seek to misuse it\n" +
                                "Activates Boss Rush Mode, using it again will deactivate Boss Rush Mode\n" +
                                "During the Boss Rush, all wires and wired devices will be disabled");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.channel = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<TerminusHoldout>();
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }
    }
}
