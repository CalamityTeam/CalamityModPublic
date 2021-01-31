using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
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
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 20;
            item.useTime = 20;
            item.noMelee = true;
            item.width = 28;
            item.height = 36;
            item.shoot = ModContent.ProjectileType<ChibiiDoggo>();
            item.buffType = ModContent.BuffType<ChibiiBuff>();
            item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Meowmere, 5);

            item.value = Item.sellPrice(gold: 7);
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().devItem = true;
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
