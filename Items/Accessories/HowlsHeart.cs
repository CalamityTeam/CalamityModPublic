using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class HowlsHeart : ModItem
    {
        public const int HowlDamage = 45;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Howl's Heart");
            Tooltip.SetDefault("Summons Howl to fight for you, Calcifer to light your way, and Turnip-Head to follow you around\n" +
            "Placing this accessory in vanity slots will summon the trio without the combat or exploration utilities");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.accessory = true;

            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().donorItem = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().howlsHeart;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.howlsHeart = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(BuffType<HowlTrio>()) == -1)
                {
                    player.AddBuff(BuffType<HowlTrio>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartHowl>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartHowl>(), (int)(HowlDamage * player.MinionDamage()), 1f, player.whoAmI, 0f, 1f);
                }
                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartCalcifer>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartCalcifer>(), 0, 0f, player.whoAmI, 0f, 0f);
                }
                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartTurnipHead>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartTurnipHead>(), 0, 0f, player.whoAmI, 0f, 0f);
                }
            }
        }
    }
}
