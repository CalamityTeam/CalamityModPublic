using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Pets
{
    public class StrangeOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strange Orb");
            Tooltip.SetDefault("Summons a miniature Ocean Spirit light pet\n" +
                "Provides a large amount of light while underwater");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.shoot = ModContent.ProjectileType<SirenYoung>();
            Item.buffType = ModContent.BuffType<SirenLightPetBuff>();
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
