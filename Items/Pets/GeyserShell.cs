using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class GeyserShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Geyser Shell");
            Tooltip.SetDefault("Summons a little flak hermit");
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
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.shoot = ModContent.ProjectileType<FlakPet>();
            item.buffType = ModContent.BuffType<FlakPetBuff>();
            item.rare = ItemRarityID.LightPurple;
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
