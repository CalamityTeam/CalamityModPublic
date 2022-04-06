using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class FoxDrive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fox Drive");
            Tooltip.SetDefault("'It contains 1 file on it'\n'Fox.cs'");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<FoxPet>();
            Item.buffType = ModContent.BuffType<Fox>();
            Item.expert = true;

            Item.value = Item.sellPrice(gold: 30);
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().devItem = true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}
