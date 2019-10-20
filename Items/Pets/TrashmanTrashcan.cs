using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Buffs.Pets;

namespace CalamityMod.Items.Pets
{
    public class TrashmanTrashcan : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trash Can");
            Tooltip.SetDefault("Summons the trash man");
        }
        public override void SetDefaults()
        {
            item.damage = 0;
            item.useStyle = 1;
            item.useAnimation = 20;
            item.useTime = 20;
            item.noMelee = true;
            item.width = 30;
            item.height = 30;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.shoot = ModContent.ProjectileType<DannyDevitoPet>();
            item.buffType = ModContent.BuffType<DannyDevito>();
            item.rare = 5;
            item.UseSound = SoundID.NPCDeath13;
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
