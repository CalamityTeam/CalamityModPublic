using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class CosmicPlushie : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Plushie");
            Tooltip.SetDefault("Summons the devourer of the cosmos...?\nSharp objects possibly included\nSuppresses friendly red devils");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useStyle = 1;
            item.useAnimation = 20;
            item.useTime = 20;
            item.noMelee = true;
            item.width = 28;
            item.height = 36;
            item.value = Item.sellPrice(0, 7, 0, 0);
            item.shoot = ModContent.ProjectileType<ChibiiDoggo>();
            item.buffType = ModContent.BuffType<ChibiiBuff>();
            item.rare = 10;
            item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Meowmere, 5);
            item.Calamity().postMoonLordRarity = 14;
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
