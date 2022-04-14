using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Pets
{
    public class JoyfulHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Joyful Heart");
            Tooltip.SetDefault("It's oddly warm. Attracts the forbidden one.");
        }
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.width = 30;
            Item.height = 30;

            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<LadShark>();
            Item.buffType = ModContent.BuffType<LadBuff>();
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
