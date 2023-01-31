using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("OldLordOathsword")]
    public class OldLordClaymore : ModItem
    {
        public bool RMBchannel = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Lord Claymore");
            Tooltip.SetDefault("A relic of the ancient underworld\n" +
                "Holding right click rapidly absorbs energy into the blade until it is sufficiently charged\n" +
                "Left clicking will either swing the blade as usual or cause you to fly in the direction of the cursor, depending on if the blade was fully charged\n" +
                "After flying the amount of charge the blade has is reduced to zero again");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.width = 70;
            Item.height = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 34;
            Item.useTime = 34;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? CanHitNPC(Player player, NPC target) => false;

        public override bool CanHitPvp(Player player, Player target) => false;
    }
}
