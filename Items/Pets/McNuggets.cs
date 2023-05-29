using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class McNuggets : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<YharonSonPet>();
            Item.buffType = ModContent.BuffType<YharonSonBuff>();

            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ModContent.RarityType<Violet>();
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
