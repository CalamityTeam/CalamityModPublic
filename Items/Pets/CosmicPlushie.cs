using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class CosmicPlushie : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Cosmic Plushie");
            Tooltip.SetDefault("Summons the devourer of the cosmos...?\nSharp objects possibly included\nSuppresses friendly red devils");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.width = 28;
            Item.height = 36;
            Item.shoot = ModContent.ProjectileType<ChibiiDoggo>();
            Item.buffType = ModContent.BuffType<ChibiiDoGBuff>();
            Item.UseSound = SoundID.Meowmere;

            Item.value = Item.sellPrice(gold: 7);
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().devItem = true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true);
            }
        }
    }
}
