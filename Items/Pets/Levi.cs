using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class Levi : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Levi");
            Tooltip.SetDefault("Summons a baby Leviathan pet");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<LeviPet>();
            item.buffType = ModContent.BuffType<LeviBuff>();

            item.value = Item.sellPrice(gold: 40);
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.Calamity().devItem = true;
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
