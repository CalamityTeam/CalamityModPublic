using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class AbyssShellFossil : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Abyss Shell Fossil");
            Tooltip.SetDefault("A fossil of a prehistoric creature, long forgotten in the deep abyss"
            + "\nSummons an Escargidolon Snail pet");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 42;
            Item.height = 30;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;

            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<EidolonSnail>();
            Item.buffType = ModContent.BuffType<EidolonSnailBuff>();
            Item.UseSound = SoundID.Item2;
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
