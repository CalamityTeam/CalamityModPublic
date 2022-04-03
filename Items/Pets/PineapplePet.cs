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
            Item.damage = 0;
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<PineapplePetProj>();
            Item.buffType = ModContent.BuffType<PineappleBuff>();

            Item.width = 32;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item2;

            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;
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
