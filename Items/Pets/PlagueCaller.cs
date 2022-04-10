using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Pets
{
    public class PlagueCaller : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Caller");
            Tooltip.SetDefault("Summons a baby Plaguebringer pet");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<PlaguebringerBab>();
            Item.buffType = ModContent.BuffType<PlaguebringerBabBuff>();
            Item.rare = ItemRarityID.Lime;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
