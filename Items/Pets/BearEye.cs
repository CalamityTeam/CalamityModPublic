using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class BearEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bear's Eye");
            Tooltip.SetDefault("Summons a pet guardian angel");
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

            item.value = Item.sellPrice(platinum: 1);
            item.rare = ItemRarityID.Pink;
            item.Calamity().devItem = true;

            item.shoot = ModContent.ProjectileType<Bear>();
            item.buffType = ModContent.BuffType<BearBuff>();
            item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Meowmere, 5);
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
