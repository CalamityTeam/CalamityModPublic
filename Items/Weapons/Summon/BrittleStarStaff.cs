using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("SeaboundStaff")]
    public class BrittleStarStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public float Knockback = 2f;
        public static int DefenseBoostPerBuffStar = 3;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenseBoostPerBuffStar);
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 64;
            Item.damage = 10;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = Knockback;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item44 with { Pitch = 0.5f };
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<BrittleStar>();
            Item.shoot = ModContent.ProjectileType<BrittleStarMinion>();
            Item.DamageType = DamageClass.Summon;
        }
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Item.noUseGraphic = false;
                int SummonNumber = player.ownedProjectileCounts[type];
                var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI, 0, 0, SummonNumber);
                minion.originalDamage = Item.damage;
            }
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = true;
            }
            int bladeIndex = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == type && p.owner == player.whoAmI)
                {
                    p.ModProjectile<BrittleStarMinion>().StarIndex = bladeIndex++;
                    p.ModProjectile<BrittleStarMinion>().AITimer = 0f;
                    p.netUpdate = true;
                }
            }
            return false;
        }
    }
}
