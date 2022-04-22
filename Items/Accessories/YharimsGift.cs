using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class YharimsGift : ModItem
    {
        public int dragonTimer = 60;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Yharim's Gift");
            Tooltip.SetDefault("The power of a god pulses from within this artifact\n" +
                               "Flaming meteors rain down while invincibility is active\n" +
                               "Exploding dragon dust is left behind as you move\n" +
                               "Damage and movement speed increased by 15%");
        }

        public override void SetDefaults()
        {
            Item.defense = 30;
            Item.width = 20;
            Item.height = 22;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.rare = ItemRarityID.Purple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var source = player.GetSource_Accessory(Item);
            player.moveSpeed += 0.15f;
            player.GetDamage<GenericDamageClass>() += 0.15f;
            if (!player.StandingStill())
            {
                dragonTimer--;
                if (dragonTimer <= 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int projectile1 = Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<DragonDust>(), (int)(175 * player.AverageDamage()), 5f, player.whoAmI, 0f, 0f);
                        Main.projectile[projectile1].timeLeft = 60;
                    }
                    dragonTimer = 60;
                }
            }
            else
            {
                dragonTimer = 60;
            }
            if (player.immune)
            {
                if (player.miscCounter % 8 == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<SkyFlareFriendly>(), (int)(375 * player.AverageDamage()), 9f, player.whoAmI);
                    }
                }
            }
        }
    }
}
