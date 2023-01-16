using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class AbyssShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Abyss Shell Fossil");
            Tooltip.SetDefault("A prehistoric shell, once belonging to a goofy aquatic creature"
            + "\nSummons an Escargidolon Snail");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<EidolonSnail>();
            Item.buffType = ModContent.BuffType<EidolonSnailBuff>();
            Item.UseSound = SoundID.Zombie53;
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
