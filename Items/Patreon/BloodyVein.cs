using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Patreon
{
    public class BloodyVein : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Vein");
            Tooltip.SetDefault("Summons an amalgamated pile of flesh");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.noMelee = true;
            item.width = 24;
            item.height = 48;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.UseSound = SoundID.NPCHit9;
            item.shoot = mod.ProjectileType("PerforaMini");
            item.buffType = mod.BuffType("BloodBound");
            item.rare = 3;
            item.Calamity().postMoonLordRarity = 21;
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
