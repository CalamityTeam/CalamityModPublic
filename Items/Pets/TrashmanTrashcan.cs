using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.width = 30;
            Item.height = 30;
            Item.shoot = ModContent.ProjectileType<DannyDevitoPet>();
            Item.buffType = ModContent.BuffType<DannyDevito>();
            Item.UseSound = SoundID.NPCDeath13;

            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().devItem = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true);
            }
        }
    }
}
