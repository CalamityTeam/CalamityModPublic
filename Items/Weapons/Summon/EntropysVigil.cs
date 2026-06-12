using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("BlightedEyeStaff")]
    public class EntropysVigil : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 2f;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.damage = 42;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<EntropysVigilBuff>();
            Item.shoot = ModContent.ProjectileType<Calamitamini>();
            Item.knockBack = 2f;

            Item.useAnimation = Item.useTime = 36;
            Item.mana = 10;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item82;

            // This doesn't do anything, it's just so the item is held like a staff.
            Item.shootSpeed = 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            Vector2 mouse = player.ClampedMouseWorld();
            float randomAngleOffset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 3; i++)
            {
                float angle = MathHelper.TwoPi / 3f * i + randomAngleOffset;
                Vector2 spawnVelocity = angle.ToRotationVector2() * 5f;

                switch (i)
                {
                    case 0:
                        type = ModContent.ProjectileType<Calamitamini>();
                        break;
                    case 1:
                        type = ModContent.ProjectileType<Catastromini>();
                        break;
                    case 2:
                        type = ModContent.ProjectileType<Cataclymini>();
                        break;
                }

                var minion = Projectile.NewProjectileDirect(source, mouse, spawnVelocity, type, damage, knockback, player.whoAmI);
                minion.originalDamage = Item.damage;
            }

            return false;
        }
    }
}
