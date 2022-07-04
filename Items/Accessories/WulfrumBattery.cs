using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Audio;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories
{
    public class WulfrumBattery : ModItem
    {
        public static readonly SoundStyle ExtraDropSound = new("CalamityMod/Sounds/Custom/WulfrumExtraDrop") { PitchVariance = 0.3f };

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Wulfrum Battery");
            Tooltip.SetDefault("7% increased summon damage\n" +
                "Attaches a spotlight on your first summon\n" +
                "50% chance to get an extra scrap when killing wulfrum robots\n" +
                "Can also be scrapped at an extractinator");

            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;

            //Needed for extractination
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.useAnimation = 10;
            Item.useTime = 2;
            Item.consumable = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<SummonDamageClass>() += 0.07f;
            player.GetModPlayer<WulfrumBatteryPlayer>().battery = true;
        }

        //Scrappable for 3-6 wulfrum scrap or a 20% chance to get an energy core
        public override void ExtractinatorUse(ref int resultType, ref int resultStack)
        {
            resultType = ModContent.ItemType<WulfrumMetalScrap>();
            resultStack = Main.rand.Next(3, 6);

            if (Main.rand.NextFloat() > 0.8f)
            {
                resultStack = 1;
                resultType = ModContent.ItemType<EnergyCore>();
            }
        }
    }

    public class WulfrumBatteryPlayer : ModPlayer
    {
        public bool battery = false;
        public override void ResetEffects() => battery = false;
        public override void UpdateDead() => battery = false;
    }
}
