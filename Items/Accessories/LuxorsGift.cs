using CalamityMod.CalPlayer;
using CalamityMod.CustomRecipes;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class LuxorsGift : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public const int meleeAttackSpeed = 100;
        public const int rangedAttackSpeed = 25;
        public const int magicAttackSpeed = 75;
        public const int summonerAttackSpeed = 140;
        public const int rogueAttackSpeed = 36; // Rogue has a 12 frame delay between it's two shots, so it's actaully a bit slower than you'd think
        public const int classlessAttackSpeed = 50;

        public const int meleeDamage = 10;
        public const int rangedDamage = 5;
        public const int magicDamage = 13;
        public const int summonerDamage = 22;
        public const int rogueDamage = 8;
        public const int classlessDamage = 9;

        public const int luxArmorPen = 35; // Remember kids, If a Shark Tooth Necklace approaches you, run away and contact your nearest trusted adult
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 46;
            Item.value = Item.buyPrice(gold: 15); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundLuxorsGift)
            {
                RecipeUnlockHandler.HasFoundLuxorsGift = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.luxorsGift = true;

            if (player.ownedProjectileCounts[ProjectileType<Luxor>()] < 1 && !player.dead)
            {
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<Luxor>(), 0, 0f, player.whoAmI);
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.luxorsGiftVanity = true;

            if (player.ownedProjectileCounts[ProjectileType<Luxor>()] < 1 && !player.dead)
            {
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<Luxor>(), 0, 0f, player.whoAmI);
            }
        }
    }
}
