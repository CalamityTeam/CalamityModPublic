using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

using static Terraria.ModLoader.ModContent;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Items.Accessories
{
    public class HowlsHeart : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int HowlDamage = 45;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
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
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(BuffType<HowlTrio>()) == -1)
                {
                    player.AddBuff(BuffType<HowlTrio>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartHowl>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Howls spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(HowlDamage);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    Projectile howl = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartHowl>(), damage, 1f, player.whoAmI, 0f, 1f);
                    howl.originalDamage = damage;
                }
                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartCalcifer>()] < 1)
                {
                    Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartCalcifer>(), 0, 0f, player.whoAmI, 0f, 0f);
                }
                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartTurnipHead>()] < 1)
                {
                    Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartTurnipHead>(), 0, 0f, player.whoAmI, 0f, 0f);
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            player.Calamity().howlsHeartVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(BuffType<HowlTrio>()) == -1)
                    player.AddBuff(BuffType<HowlTrio>(), 3600, true);

                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartHowl>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Howls spawned with... Hold on a second. Why the fuck are we doing damage calculations when the accessory is in vanity?!
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(HowlDamage);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    int p = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartHowl>(), damage, 1f, player.whoAmI, 0f, 1f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = HowlDamage;
                }

                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartCalcifer>()] < 1)
                    Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartCalcifer>(), 0, 0f, player.whoAmI, 0f, 0f);

                if (player.ownedProjectileCounts[ProjectileType<HowlsHeartTurnipHead>()] < 1)
                    Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ProjectileType<HowlsHeartTurnipHead>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 1f,
                drawOffset: new(0f, 0f)
            );
            return false;
        }
    }
}
