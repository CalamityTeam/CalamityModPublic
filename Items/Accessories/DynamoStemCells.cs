using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class DynamoStemCells : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Dynamo Stem Cells");
            Tooltip.SetDefault(@"10% increased movement speed
Ranged weapons have a chance to fire mini swarmers
Grants immunity to Dragon Fire and Electrified");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().dynamoStemCells = true;
            player.moveSpeed += 0.1f;
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[ModContent.BuffType<LethalLavaBurn>()] = true;
        }
    }
}
