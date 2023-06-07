using CalamityMod.Buffs.Potions;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Bloodfin : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public static int BuffType = ModContent.BuffType<BloodfinBoost>();
        public static int BuffDuration = 600;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 36;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.healLife = 240;
            Item.potion = true;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffType, BuffDuration);
        }
    }
}
