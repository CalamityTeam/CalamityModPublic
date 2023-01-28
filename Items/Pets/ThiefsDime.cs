using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Pets
{
    public class ThiefsDime : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Thief's Dime");
            Tooltip.SetDefault("Once worthless treasure, now a relic of a thief's companionship\n" +
            "Summons Goldie the coin to light your way and collects nearby coins\n" +
            "Provides a small amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.UseSound = SoundID.CoinPickup;
            Item.shoot = ModContent.ProjectileType<GoldiePet>();
            Item.buffType = ModContent.BuffType<GoldieBuff>();

            Item.value = Item.sellPrice(gold: 20); //Buy price of 1 Platinum
            Item.rare = ItemRarityID.Pink;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(Item.buffType, 3600, true);
        }
    }
}
