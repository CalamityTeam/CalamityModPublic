using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class PineapplePet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pineapple");
            Tooltip.SetDefault("Summons a pineapple");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useTime = item.useAnimation = 20;
            item.shoot = ModContent.ProjectileType<PineapplePetProj>();
            item.buffType = ModContent.BuffType<PineappleBuff>();

            item.width = 32;
            item.height = 34;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item2;

            item.value = Item.buyPrice(gold: 4);
            item.rare = ItemRarityID.Orange;
            item.Calamity().donorItem = true;
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
