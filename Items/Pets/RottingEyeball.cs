using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class RottingEyeball : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotting Eyeball");
            Tooltip.SetDefault("Summons a corrupted conglomeration");
        }

        public override void SetDefaults()
        {
            /*item.DefaultToVanitypet(ModContent.ProjectileType<MiniHiveMind>(), ModContent.BuffType<HiveMindPet>());
            item.rare = -13;*/
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.damage = 0;
            item.useTime = item.useAnimation = 20;
            item.useStyle = 1;
            item.noMelee = true;
            item.width = 16;
            item.height = 30;
            item.UseSound = SoundID.NPCHit2;
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<MiniHiveMind>();
            item.buffType = ModContent.BuffType<HiveMindPet>();
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
