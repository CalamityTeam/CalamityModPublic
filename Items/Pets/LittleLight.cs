using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class LittleLight : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Little Light");
            Tooltip.SetDefault("It's been looking for you for a long time.\n" +
                "Summons a small construct that follows you and provides a great amount of light\n" +
                "Provides a large amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.UseSound = SoundID.Item83;
            Item.shoot = ModContent.ProjectileType<LittleLightProj>();
            Item.buffType = ModContent.BuffType<LittleLightBuff>();

            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.Calamity().donorItem = true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(Item.buffType, 3600, true);
        }
    }
}
