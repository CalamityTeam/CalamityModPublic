using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class JoyfulHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Joyful Heart");
            Tooltip.SetDefault("It's oddly warm. Attracts the forbidden one.");
        }
        public override void SetDefaults()
        {
            item.damage = 0;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 20;
            item.useTime = 20;
            item.noMelee = true;
            item.width = 30;
            item.height = 30;

            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Pink;
            item.Calamity().devItem = true;

            item.shoot = ModContent.ProjectileType<LadShark>();
            item.buffType = ModContent.BuffType<LadBuff>();
            item.UseSound = SoundID.Item2;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 15, true);
            }
        }
    }
}
