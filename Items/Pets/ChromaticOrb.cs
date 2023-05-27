using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class ChromaticOrb : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.shoot = ModContent.ProjectileType<BendyPet>();
            Item.buffType = ModContent.BuffType<Dreamfog>();

            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Cyan;
            Item.Calamity().donorItem = true;
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
