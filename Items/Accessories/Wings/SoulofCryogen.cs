using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class SoulofCryogen : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 3));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(120, 6.25f, 1f);
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float lightOffset = (float)Main.rand.Next(90, 111) * 0.01f;
            lightOffset *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0f * lightOffset, 0.3f * lightOffset, 0.3f * lightOffset);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.cryogenSoul = true;
            player.GetDamage<GenericDamageClass>() += 0.07f;
            player.noFallDmg = true;
            if (modPlayer.icicleCooldown <= 0)
            {
                if (player.controlJump && player.jump == 0 && player.velocity.Y != 0f && !player.mount.Active && !player.mount.Cart)
                {
                    var source = player.GetSource_Accessory(Item);
                    int damage = (int)player.GetBestClassDamage().ApplyTo(25);
                    damage = player.ApplyArmorAccDamageBonusesTo(damage);

                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, player.velocity.X * 0f, 2f, ModContent.ProjectileType<FrostShardFriendly>(), damage, 3f, player.whoAmI, 1f);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].DamageType = DamageClass.Generic;
                        Main.projectile[p].frame = Main.rand.Next(5);
                    }
                    modPlayer.icicleCooldown = 10;
                }
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
