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
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<YharonSonPet>();
            item.buffType = ModContent.BuffType<YharonSonBuff>();

            item.value = Item.sellPrice(gold: 30);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
