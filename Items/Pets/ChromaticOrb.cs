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
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Chromatic Orb");
            Tooltip.SetDefault("It glows warmly in your hand\n" +
                "Summons an ancient dragon light pet that highlights nearby enemies and danger sources\n" +
                "Provides a small amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.shoot = ModContent.ProjectileType<BendyPet>();
            Item.buffType = ModContent.BuffType<BendyBuff>();

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
