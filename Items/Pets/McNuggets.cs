using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class McNuggets : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("McNuggets");
            Tooltip.SetDefault("These chicken nuggets aren't for you to eat!");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<YharonSonPet>();
            Item.buffType = ModContent.BuffType<YharonSonBuff>();

            Item.value = Item.sellPrice(gold: 30);
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}
