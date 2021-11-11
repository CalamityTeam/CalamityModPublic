using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class LittleLight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Little Light");
            Tooltip.SetDefault("It's been looking for you for a long time.\n" +
                "Summons a small construct that follows you and provides a great amount of light\n" +
                "Provides a moderate amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.UseSound = SoundID.Item83;
            item.shoot = ModContent.ProjectileType<LittleLightProj>();
            item.buffType = ModContent.BuffType<LittleLightBuff>();

            item.value = Item.sellPrice(gold: 15);
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().donorItem = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(item.buffType, 3600, true);
        }
    }
}
