using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class SoulofCryogen : BaseWings
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 3));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(160, 7.5f, 1f);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.expert = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float lightOffset = (float)Main.rand.Next(90, 111) * 0.01f;
            lightOffset *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0f * lightOffset, 0.3f * lightOffset, 0.3f * lightOffset);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.cryogenSoul = true;
            if (modPlayer.wingProjectileCooldown <= 0)
            {
                if (player.controlJump && player.jump == 0 && player.velocity.Y != 0f && !player.mount.Active && !player.mount.Cart)
                {
                    var source = player.GetSource_Accessory(Item);
                    int damage = (int)player.GetBestClassDamage().ApplyTo(32);

                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, player.velocity.X * 0f, 2f, ModContent.ProjectileType<FrostShardFriendly>(), damage, 3f, player.whoAmI, 1f);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].DamageType = DamageClass.Generic;
                        Main.projectile[p].frame = Main.rand.Next(5);
                    }
                    modPlayer.wingProjectileCooldown = 7;
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
