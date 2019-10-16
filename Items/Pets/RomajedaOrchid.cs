using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class RomajedaOrchid : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Romajeda Orchid");
            Tooltip.SetDefault("Summons a never forgotten friend");
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
            item.value = Item.sellPrice(1, 0, 0, 0);
            item.shoot = ModContent.ProjectileType<Kendra>();
            item.buffType = ModContent.BuffType<Kendra>();
            item.rare = 5;
            item.UseSound = SoundID.Item44;
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
