using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class RustedJingleBell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusted Jingle Bell");
            Tooltip.SetDefault("Summons a baby ghost bell light pet\n" +
                "Provides a moderate amount of light while underwater");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.shoot = ModContent.ProjectileType<BabyGhostBell>();
            Item.buffType = ModContent.BuffType<BabyGhostBellBuff>();
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
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
