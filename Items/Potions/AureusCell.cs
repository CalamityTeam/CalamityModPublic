using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    [LegacyName("AstralJelly")]
    public class AureusCell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.FoodParticleColors[Type] = new Color[3] {
                new Color(187, 220, 237),
                new Color(237, 93, 83),
                new Color(123, 99, 130)
            };
        }

        public override void SetDefaults()
        {
            // Eating animation but gulp sound? Sure
            Item.DefaultToFood(22, 38, BuffID.MagicPower, CalamityUtils.MinutesToFrames(8));
            Item.UseSound = SoundID.Item3;
            Item.value = Item.sellPrice(silver: 50); // Based on material cost rather than potion cost
            Item.rare = ItemRarityID.Lime;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.MagicPower, Item.buffTime);
            player.AddBuff(BuffID.ManaRegeneration, Item.buffTime);
        }
    }
}
